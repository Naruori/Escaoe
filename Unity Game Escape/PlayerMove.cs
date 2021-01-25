using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour {
	
	public float WalkSpeed = 20F;   // 앞뒤 이동속도
	public float StrafeSpeed = 15F;   // 좌우이동속도
	public float JumpForce = 10F;   // 점프시 상승속도
	private float HorizontalSensitivity = 4F;  // 카메라 좌우 민감도
	private float VerticalSensitivity = 2F; // 카메라 상하 민감도
	private float currentCameraRotationX = 0;   // 카메라 방향
	public bool CanPlayerMove = true;   // 플레이어 움직임 가능여부 확인
	public float AirTime;   // 공중에 떠있는 시간
	public float VerticalVelocity;  // Y축 방향 이동속도
	public float Gravity = 25F; // 중력
	private float startNum = 0; // 시작시 대기시간
	[SerializeField] private int startHP; // 시작 체력
	[HideInInspector] public int hp; // 현재 체력

	//필요한 컴포넌트
	private CharacterController player;  // 플레이어 컨트롤러
	private GameObject Camera;  // 플레이어 시점 카메라
	private SoundManager theSoundManager;// 사운드 매니저

	[SerializeField] private GameObject PlayerHpBar; // 플레이어 체력바


	void Start ()
	{
		//필요한 오브젝트 가져오기
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
		Camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject;
		theSoundManager = GameObject.FindObjectOfType<SoundManager>();
		hp = startHP;
	}





	void Update ()
	{
		if (hp <= 0)
			Die();
		// 플레이어의 저장위치 대입할때까지 대기시간
		if (startNum<0.1)
			startNum += Time.deltaTime;
		// 메뉴 비활성화상태
		if (!GameManager.isPause)
		{
			RotationCamera();//카메라 회전
		}

		ChangeSensitivity();//민감도 조절
		if (startNum >= 0.1)
		{
			if (CanPlayerMove == true)
			{
				MovePlayer();//플레이어 움직임
				VerticalMovement();//플레이어 y축 방향 이동
				Jump();//점프
			}
		}
	}
	//플레이어 사망
	private void Die()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	//민감도 조절
	private void ChangeSensitivity()
	{
		//"["키 입력시 마우스 민감도 감소
		if (Input.GetKeyDown(KeyCode.LeftBracket))
		{
			HorizontalSensitivity -= 0.2f;
			VerticalSensitivity -= 0.1f;
			// 민감도가 음수면 0으로 초기화 
			if (HorizontalSensitivity < 0 && VerticalSensitivity < 0)
			{
				HorizontalSensitivity = 0f;
				VerticalSensitivity = 0f;
			}
		}
		//"]"키 입력시 마우스 민감도 증가
		if (Input.GetKeyDown(KeyCode.RightBracket))
		{
			HorizontalSensitivity += 0.2f;
			VerticalSensitivity += 0.1f;
		}

	}
	//카메라 회전
	private void RotationCamera()
	{
		float RotX = Input.GetAxisRaw("Mouse X") * HorizontalSensitivity;//마우스X 값 받기
		float RotY = Input.GetAxisRaw("Mouse Y") * VerticalSensitivity;//마우스y 값 받기


		//카메라 회전
		currentCameraRotationX -= RotY;
		currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -90, 90);
		Camera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
		transform.Rotate(0, RotX, 0);
	}
	//플레이어 위아래 움직임
	public void VerticalMovement()
	{
		//Y축 방향 이동
		Vector3 MoveVector = new Vector3(0, VerticalVelocity, 0);
		player.Move(MoveVector * Time.deltaTime);
		//중력 적용
		if(!player.isGrounded)
		{
			VerticalVelocity -= Gravity * 1F * Time.deltaTime;
		}
		else
		{
			VerticalVelocity = 0;
		}
		//공중에 떠있는 시간
		if(!player.isGrounded)
			AirTime++;
		else
			AirTime = 0;
	}
	//점프
	void Jump()
	{
		if (player.isGrounded && Input.GetKeyDown(KeyCode.Space))
		{
			VerticalVelocity = JumpForce;
			theSoundManager.PlaySE("Jump");
		}
	}

	//플레이어 앞뒤좌우움직임
	void MovePlayer()
	{
		float MoveForward = Input.GetAxis("Vertical") * WalkSpeed;//앞뒤 이동속도 받기
		float MoveStrafe = Input.GetAxis("Horizontal") * StrafeSpeed;//좌우 이동속도 받기

		//플레이어 움직임 벡터 생성
		Vector3 FBMovement = new Vector3 (0, 0, MoveForward);
		FBMovement = transform.rotation * FBMovement;

		Vector3 RLMovement = new Vector3 (MoveStrafe, 0, 0);
		RLMovement = transform.rotation * RLMovement;

		player.Move(FBMovement * Time.deltaTime);//앞뒤로 움직이기
		player.Move(RLMovement * Time.deltaTime);//좌우로 움직이기
	}

	public void GetHit(int _damage)
	{
		hp -= _damage;
		PlayerHpBar.GetComponent<Image>().fillAmount = hp / (float)startHP; // 데미지 맞으면 시작 체력에 데미지 감소해서 현재 체력 안 맞으면 시작 체력과 현재 체력 동일
	}
}
