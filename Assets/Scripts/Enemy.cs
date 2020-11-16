﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;    //material은 meshRenderer컴포터넌트에서 접근 가능!

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;//넉백 만들기 (현재 위치에 피격 위치를 빼서 반작용 방향 구하기)
            StartCoroutine(OnDamage(reactVec));

            Debug.Log("Melee : " + curHealth);
        }
        else if (other.tag == "Bullet") 
        {
            MyBullet bullet = other.GetComponent<MyBullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject); //총알의경우 적과 닿았을때 삭제
            StartCoroutine(OnDamage(reactVec)); 

            Debug.Log("Range: " + curHealth);
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        mat.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            mat.color = Color.white;
        }
        else {
            mat.color = Color.gray;
            gameObject.layer = 14;  //layer를 14번 layer로 바꿔준다. (Enemy -> EnemyDead)

            reactVec = reactVec.normalized;
            reactVec += Vector3.up*10;
            rigid.AddForce(reactVec *1, ForceMode.Impulse);
            Destroy(gameObject, 4); //4초뒤에 사라짐
        }
    }
}
