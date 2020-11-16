using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision)  //충돌 이벤트
    {
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);  //3초뒤에 사라짐
        }
        else if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject);
        }
    }
}
