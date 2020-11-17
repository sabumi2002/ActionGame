using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;

    void OnCollisionEnter(Collision collision)  //충돌 이벤트
    {
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);  //3초뒤에 사라짐
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.tag == "Wall") {   //근접공격 범위가 파괴되지않도록 조건추가(BoxCollider)
            Destroy(gameObject);
        }
    }
}
