using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;  //물리적속도를 모두 Vector3.zero로 초기화
        rigid.angularVelocity = Vector3.zero;   //회전속도
        meshObj.SetActive(false);   //수류탄 매쉬는 비활성화, 폭발은 활성화 하기
        effectObj.SetActive(true);

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, 
                                                                            LayerMask.GetMask("Enemy"));
                                                                            //구체모양의 레이캐스팅(모든오브젝트)를 가져옴
        foreach (RaycastHit hitObj in rayHits) { //raycastHit 데이터를 한개한개 가져옴 (수류탄범위 적들의 피격함수 호출)
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5); //수류탄은 파티클이 사라지는 시간을 고려하여 Destroy() 호출
    }
}
