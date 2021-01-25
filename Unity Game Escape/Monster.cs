using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    //몬스터 능력치
    [SerializeField] float viewAngle = 120f; // 몬스터 시야각도
    [SerializeField] float viewDistance = 10f; // 몬스터 시야 거리
    [SerializeField] float attackDistance = 2f; // 공격 거리
    [SerializeField] LayerMask targetMask; //공격대상의 레이어 마스크
    NavMeshAgent m_enermy = null;// NavMesh
    [SerializeField] float speed = 3.0f; // 속도
    [SerializeField] int damage = 1; // 데미지
    [SerializeField] float attackDelay;
    [SerializeField] float beforeAttackDelay = 0.1f;
    [SerializeField] float afterAttackDelay = 0.1f;
    [SerializeField] private int startHP; // 시작 체력
    [HideInInspector] public int hp; // 현재 체력

    Transform m_target = null;// 공격 대상
    
    Vector3 startDirection;// 시작 방향


    private Vector3 direction;// 방향

    //상태변수
    private bool isRunning = false;// 달리는 중인가
    private bool isAttacking = false;// 공격중인가
    private bool isDie = false;// 죽었는가
    private bool isSwing = false;// 공격동작중인가


    //필요한 컴포넌트
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerMove thePlayer;



    [SerializeField] private GameObject DamageText; // 데미지 숫자 표시
    [SerializeField] private GameObject TextPos; // 데미지 숫자 위치

    [SerializeField] private GameObject HealthBar; // 적 체력바

    void Start()
    {
        m_enermy = GetComponent<NavMeshAgent>();
        m_enermy.speed = speed;
        startDirection = transform.eulerAngles;
        hp = startHP;
    }


    void Update()
    {
        if (!isDie)
        {
            View();// 시야 구현
            if (m_target != null)// 목표지점 갱신하기
            {
                Vector3 _direction = (m_target.position - transform.position).normalized;
                m_enermy.SetDestination(m_target.position - _direction * attackDistance);
            }
        }
		else
		{
            m_enermy.SetDestination(gameObject.transform.position);
		}
    }
    private Vector3 BoundaryAngle(float _angle)
	{
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
	}
    private void View()// 시야 구현
	{
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * -0.5f);
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * -0.5f);

        Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red);
        Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.red);

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

		for (int i = 0; i < _target.Length; i++)
		{
            Transform _targetTf = _target[i].transform;
            if(_targetTf.name == "Player")// 근처에 있는 플레이어 감지
			{
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                if(_angle < viewAngle * 0.5f)// 플레이어가 시야 안에 있는지 확인
				{
                    RaycastHit _hit;
                    if(Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance))//by은오,대상을 향해 빛 발사
                    {
                        if (_hit.transform.name == "Player")// 플레이어와의 사이에 장애물 없음
                        {
                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);
                            if (Physics.Raycast(transform.position + transform.up, _direction, out _hit, attackDistance))//by은오, 공격범위 안에 있으면 공격
                            {
                                thePlayer = _targetTf.GetComponent<PlayerMove>();
                                Attack();
                            }
                            else// 공격범위 밖이면 추격
                            {
                                isRunning = true;
                                anim.SetBool("isRunning", true);
                                SetTarget(_targetTf);
                            }
                        }
                    }
				}
			}
		}
    }
    private void Attack()// 공격 시작함수
	{
		if (!isAttacking)
		{
            StartCoroutine(AttackCoroutine());
        }
	}

    IEnumerator AttackCoroutine()// 공격 시작 코루틴
    {
        isAttacking = true; isRunning = false;
        anim.SetTrigger("Attack");
        m_enermy.speed = 0f;

        yield return new WaitForSeconds(beforeAttackDelay);
        isSwing = true;

        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(afterAttackDelay);
        isSwing = false;

        yield return new WaitForSeconds(attackDelay - beforeAttackDelay - afterAttackDelay);

        m_enermy.speed = speed;
        isAttacking = false;
    }
    IEnumerator HitCoroutine()// 공격 동작 코루틴
    {
        while(isSwing)
        { 
            isSwing = false;
            thePlayer.GetHit(damage);
            Debug.Log("플레이어 체력: " + thePlayer.hp);
        }
        yield return null;
    }
    public void SetTarget(Transform t_target)// 목표지점 설정
    {
        m_target = t_target;
    }
    public void RemoveTarget()
	{
        m_target = null;
	}

    private void Die()// 몬스터 사망
	{
        isDie = true;
        anim.SetTrigger("Die");
        Destroy(gameObject, 2f);
	}
    public void GetHit(int _damage, Transform _player)// 피격
	{
        m_target = _player;
        hp -= _damage;
        anim.SetTrigger("GetHit");
        GameObject dmgText = Instantiate(DamageText, TextPos.transform.position, Quaternion.identity);
        dmgText.GetComponent<Text>().text = damage.ToString();
        HealthBar.GetComponent<Image>().fillAmount = hp / (float)startHP; // 데미지 맞으면 시작 체력에 데미지 감소해서 현재 체력 안 맞으면 시작 체력과 현재 체력 동일
        Destroy(dmgText, 0.5f); // 데미지 숫자 표시 0.5초후에 없어짐
        Debug.Log("몬스터 체력" + hp);
        if (hp > 0)
            GetHitCoroutine();
        else
        {
            Die();
        }
	}
    IEnumerator GetHitCoroutine()// 피격시 코루틴
    {
        StopAllCoroutines();
        anim.SetTrigger("GetHit");
        m_enermy.speed = 0f;
        yield return new WaitForSeconds(1f);
        m_enermy.speed = speed;
        isRunning = true;
        anim.SetBool("isRunning", true);
    }
}
