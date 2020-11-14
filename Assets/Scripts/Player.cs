using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades; //수륙탄(필살기)

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
    bool iDown;
    bool sDown1;    //스왑1 (무기)
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady = true;   //공격준비
    

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;  //최근에 어떤 무기오브젝트를 실행했는지 알기위해 사용
    Weapon equipWeapon; //장착중인 웨폰은 어떤것입니까?  무기가 여러개 겹치게 들지않기위해 사용
    int equipWeaponIndex = -1;
    float fireDelay;    //공격딜레이


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Dodge();
        Interation();
        Swap();
    }
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");        //input manager에 버튼 생성후 사용(Walk)
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButtonDown("Fire1");
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

        if (isSwap || !isFireReady)
            moveVec = Vector3.zero;

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;


        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec); //플레이어를 방향키 움직이는대로 회전시켜줌
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

    void Attack()
    {
        if (equipWeapon == null)
            return;
        fireDelay += Time.deltaTime;    //매프레임 소비한 시간을 더해줌
        isFireReady = equipWeapon.rate < fireDelay;  //공격딜레이에 시간을 더해주고 공격가능 여부를 확인

        if (fDown && isFireReady && !isDodge && !isSwap) {
            equipWeapon.Use();  //조건이 충족되면 무기에 있는 함수 실행
            anim.SetTrigger("doSwing"); //애니메이션은 모두 트리거로 사용
            fireDelay = 0;  //공격딜레이를 0으로 돌려서 다음공격까지 기다리도록 작성
        }
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


    void OnCollisionEnter(Collision collision)  //이 함수로 착지 구현 (충돌시)
    {
        if (collision.gameObject.tag == "Floor") {  //태그를 활용하여 바닥에 대해서만 작동하도록!
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item") {
            Item item = other.GetComponent<Item>();
            switch (item.type) {
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
