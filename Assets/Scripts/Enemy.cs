using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameManagerLogic manager;
    public int enemyDead;
    public enum Type { A, B, C , D};   //몬스터마다 구분할수있게 함
    public Type enemyType;  //enum 구분한걸 저장할수있음
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public bool isChase;
    public bool isAttack;
    public bool isDead;
    

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[]  meshs;  //피격시 모든색상을 바꾸기위해 배열로 다 받아옴
    public NavMeshAgent nav;   //nav관련클래스는 UnityEngine.AI 네임스페이스 사용
    public Animator anim;

    void Awake()    //초기화
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();    //material은 meshRenderer컴포터넌트에서 접근 가능!
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if(enemyType != Type.D)
            Invoke("ChaseStart", 2);
    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
        if (nav.enabled && enemyType != Type.D) {
            nav.SetDestination(target.position);    //도착할 목표위치 지정함수 (목표추적)
            nav.isStopped = !isChase;   //isStopped를 사용화여 완벽하게 멈추도록 작성
        }
    }

    void FreezeVelocity()   //부딪쳐도 물리 오류 x (회전하는 둥)
    {
        if (isChase) {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    void Targeting() 
    {
        if (!isDead && enemyType != Type.D)
        {
            float targetRadius = 0;
            float targetRange = 0;

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 1.5f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targetRange = 10f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }

            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
                                                                                targetRadius,
                                                                                transform.forward, targetRange,
                                                                                LayerMask.GetMask("Player"));
            //몬스터 앞쪽으로 레이캐스트를 쏠것임
            if (rayHits.Length > 0 && !isAttack)
            {//rayHit 변수에 데이터가 들어오면 공격 코루틴 실행
                StartCoroutine(Attack());
            }
        }
    }
    IEnumerator Attack()
    {//먼저 정지를 한 다음, 애니메이션과 함께 공격범위 활성화
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.4f);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }



        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }
    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;//넉백 만들기 (현재 위치에 피격 위치를 빼서 반작용 방향 구하기)
            StartCoroutine(OnDamage(reactVec, false));

            Debug.Log("Melee : " + curHealth);
        }
        else if (other.tag == "Bullet") 
        {
            MyBullet bullet = other.GetComponent<MyBullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject); //총알의경우 적과 닿았을때 삭제
            StartCoroutine(OnDamage(reactVec, false)); 

            Debug.Log("Range: " + curHealth);
        }
    }
    public void HitByGrenade(Vector3 explosionPos) //수류탄 피격
    {       //피격로직이랑 비슷
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;

        

        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 14;  //layer를 14번 layer로 바꿔준다. (Enemy -> EnemyDead)
            isDead = true;
            isChase = false;    //적이 죽는 시점에서도 애니메이션과 플래그 셋팅
            nav.enabled = false;    //사망리액션을 유지하기위해 비활성화
            anim.SetTrigger("doDie");
            
            /////////////
            if (isGrenade)//수류탄 사망리액션은 큰힘과 회전을 추가
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 7;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
                
            }
            else {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                
            }   

            enemyDead++;
            if (enemyDead==1)
                manager.enemyCount++;

            if(enemyType != Type.D)
                Destroy(gameObject, 0.01f); //4초뒤에 사라짐
            
        }
    }
}
