using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    Vector3 lookVec;    //player 움직임 예측 벡터 변수 생성
    Vector3 tauntVec;
    public bool isLook;    //player를 바라보는 플래그 bool 변수 추가

    void Awake()    //상속받을 때 Awake 함수는 자식 스크립트만 단독실행
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();    //material은 meshRenderer컴포터넌트에서 접근 가능!
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;
        StartCoroutine(Think());

    }

    
    void Update()
    {
        if (isDead) {
            StopAllCoroutines();
            return;
        }
        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");   //플레이어가 입력한 방향을 알기위해
            float v = Input.GetAxisRaw("Vertical");        //플레이어가 입력한 방향을 알기위해
            lookVec = new Vector3(h, 0, v) * 5f;        //5초마다 예측
            transform.LookAt(target.position + lookVec);
        }
        else
            nav.SetDestination(tauntVec);   //점프공격할 떄 목표지점으로 이동하도록 로직 추가
    }
    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int ranAction = Random.Range(0, 5); //행동 패턴을 만들기 위해 Random.Range() 호출 (0, 1, 2, 3, 4)
        switch (ranAction) {
            case 0:
            case 1:
                //미사일 발사 패턴
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                //돌 굴러가는 패턴
                StartCoroutine(RockShot());
                break;
            case 4:
                //점프공격 패턴
                StartCoroutine(Taunt());
                break;
        }

    }
    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);    //미사일 생성
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;


        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);    //미사일 생성
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f);
        StartCoroutine(Think());
    }
    IEnumerator RockShot()
    {
        isLook = false;      //기모을때는 바라보기 중지
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);


        yield return new WaitForSeconds(3f);
        isLook = true;
        StartCoroutine(Think());
    }
    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;
        

        StartCoroutine(Think());
    }
}
