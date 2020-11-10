using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;

    Vector3 moveVec;

    void Start()
    {
        
    }

    Animator anim;

    private void Awake()
    {

        anim = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //어떤값이든 1로 보정

        
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);

        transform.LookAt(transform.position + moveVec); //플레이어를 방향키 움직이는대로 회전시켜줌

    }
}
