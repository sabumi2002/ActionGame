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

    Vector3 moveVec;
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
    }
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");

    }
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //어떤값이든 1로 보정


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
        if (jDown && !isJump) {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void OnCollisionEnter(Collision collision)  //이 함수로 착지 구현 (충돌시)
    {
        if (collision.gameObject.tag == "Floor") {  //태그를 활용하여 바닥에 대해서만 작동하도록!
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

}
