using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;

    bool isJump;
    bool isDodge;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    private void Awake()
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
        Dodge();
    }
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");        //input manager에 버튼 생성후 사용(Walk)
        jDown = Input.GetButtonDown("Jump");    

    }
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //어떤값이든 1로 보정

        if (isDodge)
            moveVec = dodgeVec; //회피중에는 움직임벡터 -> 회피방향 벡터로 바뀌도록 구현

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
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge) {  //제자리에서 점프할때 점프
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }
    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge)    //움직이면서 점프할때 회피모션
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

    void OnCollisionEnter(Collision collision)  //이 함수로 착지 구현 (충돌시)
    {
        if (collision.gameObject.tag == "Floor") {  //태그를 활용하여 바닥에 대해서만 작동하도록!
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

}
