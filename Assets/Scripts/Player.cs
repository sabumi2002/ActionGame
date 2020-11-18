using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public GameManagerLogic manager;
    public Enemy EnemyManager;
    public int itemCount;   //item == enemy
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades; //수륙탄(필살기)
    public GameObject grenadeObj;   //수륙탄 저장할 오브젝트
    public Camera followCamera;

    public int ammo;    //탄약 변수 생성
    public int coin;        //동전
    public int health;     //체력
    

    public int maxAmmo;    //최대치
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool fDown; //공격 키다운    fireDown
    bool gDown;
    bool rDown;
    bool iDown;
    bool sDown1;    //스왑1 (무기)
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true;   //공격준비
    bool isBorder;
    bool isDamage;

    

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs;   //Player 피격 색깔 넣기위해 사용

    GameObject nearObject;  //최근에 어떤 무기오브젝트를 실행했는지 알기위해 사용
    Weapon equipWeapon; //장착중인 웨폰은 어떤것입니까?  무기가 여러개 겹치게 들지않기위해 사용
    int equipWeaponIndex = -1;
    float fireDelay;    //공격딜레이


    void Awake()    //초기화
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
    }
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Interation();
        Swap();

    }
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");        //input manager에 버튼 생성후 사용(Walk)
        jDown = Input.GetButtonDown("Jump");    //getButtonDown은 한번만 입력
        fDown = Input.GetButton("Fire1");   //getButton은 꾹 누르고있으면 계속 입력
        gDown = Input.GetButton("Fire2");
        rDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //어떤값이든 1로 보정

        if (isDodge)
            moveVec = dodgeVec; //회피중에는 움직임벡터 -> 회피방향 벡터로 바뀌도록 구현

        if (isSwap || !isFireReady || isReload ) //스왑하거나 공격할때 이동x
            moveVec = Vector3.zero;
        if(!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;



        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        //#1. 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec); //플레이어를 방향키 움직이는대로 회전시켜줌
        //#2. 마우스에 의한 회전
        /*
        if (fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;  //정보를 저장할 변수 추가
            if (Physics.Raycast(ray, out rayHit, 100))
            {    //out: return처럼 반환값을 주어진 변수에 저장하는 키워드
                 //ray가 어느 오브젝트에 닿았으면 rayHit에 저장해줌
                Vector3 nextVec = rayHit.point - transform.position;    //rayCasthit의 마우스 클릭 위치 활용하여 회전을 구현
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
        */
        
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap) {  //제자리에서 점프할때 점프
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Grenade()
    {
        if (hasGrenades == 0)
            return;
        if (gDown && !isReload && !isSwap)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;  //정보를 저장할 변수 추가
            if (Physics.Raycast(ray, out rayHit, 100))
            {    //out: return처럼 반환값을 주어진 변수에 저장하는 키워드
                 //ray가 어느 오브젝트에 닿았으면 rayHit에 저장해줌
                Vector3 nextVec = rayHit.point - transform.position;    //rayCasthit의 마우스 클릭 위치 활용하여 회전을 구현
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false); //0번째 수륙탄이면 비활성화
            }
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;
        fireDelay += Time.deltaTime;    //매프레임 소비한 시간을 더해줌
        isFireReady = equipWeapon.rate < fireDelay;  //공격딜레이에 시간을 더해주고 공격가능 여부를 확인

        if (fDown && isFireReady && !isDodge && !isSwap) {
            equipWeapon.Use();  //조건이 충족되면 무기에 있는 함수 실행
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot"); //무기타입에따라 다른 트리거 실행
            fireDelay = 0;  //공격딜레이를 0으로 돌려서 다음공격까지 기다리도록 작성
        }
    }

    void Reload() 
    {
        if (equipWeapon == null)
            return;
        if (equipWeapon.type == Weapon.Type.Melee)
            return;
        if (ammo == 0)
            return;
        if (rDown && !isDodge && !isJump && !isSwap && isFireReady) {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 3f);
        }
    }
    void ReloadOut() 
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }
    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)    //움직이면서 점프할때 회피모션
        {
            dodgeVec = moveVec; //dodge 사용중일때 moveVec 복사 ( Move()에서 사용됨 )
            speed *= 2;             //스피드 2배 상승
            anim.SetTrigger("doDodge");         //애니메이션 doDodge 실행
            isDodge = true;         //Dodge가 실행중이다. 라고 알려주는 역할

            Invoke("DodgeOut", 0.5f);   //시간차 함수 호출(0.5초)   (회피기 딜레이)
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;      //스피드 1/2
        isDodge = false;
    }

    void Swap()
    {
        if(sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge) {
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }
    void Interation()
    {
        if (iDown && nearObject != null && !isJump && !isDodge) {
            if (nearObject.tag == "Weapon") {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);    //먹은 아이템 사라지게만듬
            }
        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }
    void StopToWall()   //벽충돌방지
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);  //DrawRay(시작위치, 쏘는방향, ray의 길이, 색깔 )
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall")); //(시작위치, 쏘는방향, 길이, LayerMask))
        //Raycast(): Ray룰 쏘아 닿는 오브젝트를 감지하는 함수
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void OnCollisionEnter(Collision collision)  //이 함수로 착지 구현 (충돌시)
    {
        if (collision.gameObject.tag == "Floor") {  //태그를 활용하여 바닥에 대해서만 작동하도록!
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);  // 삭제
        }
        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                MyBullet enemyBullet = other.GetComponent<MyBullet>();
                health -= enemyBullet.damage;

                bool isBossAtk = other.name == "Boss melee Area";

                StartCoroutine(OnDamage(isBossAtk));
            }
            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);
        }
        else if (other.tag == "FinishPoint")
        {
            if (manager.enemyCount == manager.TotalItemCount)
            {
                //game clear!
                if (manager.stage == 2)
                    SceneManager.LoadScene(0);
                else
                    SceneManager.LoadScene(manager.stage + 1);

            }
            else
            {
                //restart..
                SceneManager.LoadScene(manager.stage);

            }
        }
    }
    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true;
        foreach (MeshRenderer mesh in meshs) {
            mesh.material.color = Color.yellow; //player 피격 색 변경
        }

        if (isBossAtk)  //보스공격이면
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse); //넉백

        yield return new WaitForSeconds(1f);    //무적타임조정
        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;  //player 피격 색 변경
        }

        if (isBossAtk)  //보스공격이면
            rigid.velocity = Vector3.zero;  //넉백종료
    }
    void OnTriggerStay(Collider other)      //d 
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;

        
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
