using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour {
	//능력치
	public int MaximumReach;// 최대 사거리 
	public int SpeedofTravel;// 줄타는속도
	private float GrappleCooldownTimer = 0; // 쿨타임타이머 
	[SerializeField]
	[Header("쿨타임시간(초)")]
	private float GrappleCooldown;// 쿨타임시간 
	public float HookTravelSpeed = 15;// 밧줄타는 이동속도
	public float DismountJumpMultiplier = 1.5F;// 목표지점 도달시 점프 힘

	//프리펩
	public GameObject GrappleHookPrefab;// 갈고리프리펩
	public LineRenderer RopeRenderer;// 밧줄프리펩 
	public GameObject LineStartPoint;// 밧줄 나가는 위치 
	public PlayerMove Player;// 플레이어 스크립트
	public Transform HookSpawnPoint;// 갈고리 생성 위치
	GameObject TheHook; // 갈고리 버퍼 
	public GameObject FakeHook;// 갈고리 머리 
	private SoundManager theSoundManager;// 사운드 매니저 

	//상태 변수
	public bool HasLockedOn;// 갈고리가 발사되어 있는지 확인 
	public bool IsGrappling;// 줄타는 중인지 확인
	public bool ErrorHook;

	RaycastHit Hit;
	Vector3 HookTargetPoint;//갈고리의 목표 지점
	

	

	void Start()
	{
		HasLockedOn = false;//
		RopeRenderer.enabled = false;// 로프안보이게

		FakeHook.GetComponent<MeshRenderer>().enabled = true;
		theSoundManager = FindObjectOfType<SoundManager>();
	}


	void Update()
	{
		if (!GameManager.isPause)
		{
			// 쿨타임 구현
			if (GrappleCooldownTimer < GrappleCooldown)
				GrappleCooldownTimer += Time.deltaTime;

			ReelingSoundEffect();// 효과음 재생
			TheHookLookAt();// 갈고리방향 잡기 
			SetTheHookDaHook();// DaHook가져오기 
			ArriveAtTarget();// 목표지점 도착 
			ReelMeIn(); // 갈고리 방향으로 이동 
			// 갈고리가 생성되고 Hit에 객체가 있을때 
			if (TheHook != null && Hit.point != null)
			{
				// 갈고리와 목표지점 차이가 15미만 & 갈고리에러상태 아니고 & 소리 재생중이 아닐때 
				if (Vector3.Distance(TheHook.transform.position, Hit.point) < 15 && ErrorHook == true && !theSoundManager.IsPlaying("Grapple_Hook"))
					theSoundManager.PlaySE("Grapple_Hook");// 팅겨져 나오는 소리 
				// 갈고리와 목표지점 차이가 15미만 & 갈고리에러상태 아닐때 
				if (Vector3.Distance(TheHook.transform.position, Hit.point) < 10 && ErrorHook == true)
				{
					FakeHook.GetComponent<MeshRenderer>().enabled = true;
					IsGrappling = false;
					HasLockedOn = false;
					ErrorHook = false;
					RopeRenderer.enabled = false;
					Player.CanPlayerMove = true;
					Destroy(GameObject.Find("DaHook"));
				}
			}
			// 릴 로프의 시작을 올바른 위치로 설정 
			if (RopeRenderer.enabled == true && TheHook != null)
			{
				RopeRenderer.SetPosition(0, LineStartPoint.transform.position);
				RopeRenderer.SetPosition(1, TheHook.transform.position);
			}




			//마우스 오른쪽 버튼을 놓으면 멈춤 
			if (Input.GetMouseButtonUp(1) && HasLockedOn)
			{
				FakeHook.GetComponent<MeshRenderer>().enabled = true;
				IsGrappling = false;
				HasLockedOn = false;
				RopeRenderer.enabled = false;
				Player.CanPlayerMove = true;
				ErrorHook = false;
				Destroy(GameObject.Find("DaHook"));
				Player.VerticalVelocity = Player.JumpForce * DismountJumpMultiplier;
				
			}

			// 마우스 왼쪽 버튼을 누르면 훅을 쏨 
			if (Input.GetMouseButtonDown(0))
			{
				ActuallyShootHook();
			}

			// 후크에 타겟이 있으면 후크를 쏨
			if (HasLockedOn == true || ErrorHook == true)
			{
				MoveTheHookTowardsTarget();
			}
		}
	}

	// 감는 사운드효과
	void ReelingSoundEffect()
	{
		if (IsGrappling && !theSoundManager.IsPlaying("Grapple_Reel_3"))
		{
			theSoundManager.PlaySE("Grapple_Reel_3");//by은오, 받줄 감는 소리 2020.10.06
		}
        else if (!IsGrappling && theSoundManager.IsPlaying("Grapple_Reel_3"))
        {
            theSoundManager.StopSE("Grapple_Reel_3");//by은오, 받줄 감는 소리 2020.10.06
        }
	}
	// 갈고리 방향 잡기
	void TheHookLookAt()
	{
		if (TheHook != null)
		{
			TheHook.transform.LookAt(Camera.main.transform.position);
		}
	}
	// TheHook에 DaHook넣기
	void SetTheHookDaHook()
	{
		if (GameObject.Find("DaHook") != null)
		{
			TheHook = GameObject.Find("DaHook");
		}
	}
	// 목표지점 도착
	void ArriveAtTarget()
	{
		if (TheHook != null && Hit.point != null)
		{
			if (Vector3.Distance(transform.position, Hit.point) < 1 && IsGrappling)
			{
				FakeHook.GetComponent<MeshRenderer>().enabled = true;
				IsGrappling = false;
				HasLockedOn = false;
				ErrorHook = false;
				RopeRenderer.enabled = false;
				Player.CanPlayerMove = true;
				Destroy(GameObject.Find("DaHook"));
				Player.VerticalVelocity = Player.JumpForce * DismountJumpMultiplier;
			}
		}
	}
	// 갈고리 목표지점으로 이동 

	void MoveTheHookTowardsTarget()
	{
		if(TheHook != null)TheHook.transform.position = Vector3.Lerp(TheHook.transform.position, HookTargetPoint, HookTravelSpeed * 1F * Time.deltaTime);
	}
	// 실제로 갈고리 발사
	void ActuallyShootHook()
	{
		//사거리 안에 물체가 존재하는지 확인
		if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out Hit, MaximumReach))
		{
			// 갈고리를 쏘고, 설정하고, 밧줄을 활성화하고 bool 설정 
			if (GrappleCooldownTimer >= GrappleCooldown && Hit.transform.gameObject != null)
			{
				if(GameObject.Find("DaHook") != null)
					Destroy(GameObject.Find("DaHook"));
				HookTargetPoint = Hit.point;
				theSoundManager.PlaySE("Grapple_Throw");
				GameObject DaHook = Instantiate(GrappleHookPrefab) as GameObject;
				FakeHook.GetComponent<MeshRenderer>().enabled = false;
				RopeRenderer.enabled = true;
				RopeRenderer.SetPosition(0, LineStartPoint.transform.position);
				RopeRenderer.SetPosition(1, DaHook.transform.position);
				DaHook.name = "DaHook";
				DaHook.transform.position = HookSpawnPoint.transform.position;
				if (Hit.transform.gameObject.tag == "CanGrapple") HasLockedOn = true;
				else HasLockedOn = false;
				if (Hit.transform.gameObject.tag != "CanGrapple") ErrorHook = true;
				else ErrorHook = false;
				GrappleCooldownTimer = 0;
			}
		}
		else
		{
			FakeHook.GetComponent<MeshRenderer>().enabled = true;
			IsGrappling = false;
			HasLockedOn = false;
			ErrorHook = false;
			RopeRenderer.enabled = false;
			Player.CanPlayerMove = true;
			Destroy(GameObject.Find("DaHook"));
		}
	}

	// 갈고리가 부착되어 있고 마우스 오른쪽 버튼을 누르면 플레이어를
	void ReelMeIn()
	{
		if (Input.GetMouseButton(1) && HasLockedOn)
		{
			// 로프 위치를 업데이트하는 동안 플레이어를 후크 지점으로 이동
			HasLockedOn = true;
			RopeRenderer.SetPosition(0, LineStartPoint.transform.position);
			RopeRenderer.SetPosition(1, Hit.point);
			Player.CanPlayerMove = false;
			transform.position = Vector3.Lerp(gameObject.transform.position, Hit.point, SpeedofTravel * Time.deltaTime / Vector3.Distance(gameObject.transform.position, Hit.point));
			IsGrappling = true;
		}
	}
}
