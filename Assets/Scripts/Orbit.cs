using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;
    public float orbitSpeed;
    Vector3 offset;

    void Start()
    {
        offset = transform.position - target.position;  //지금 현재 위치에서 타겟 위치를 뺀다. (플레이어와 수륙탄과의 차이값이 나옴)
    }

    void Update()
    {
        transform.position = target.position + offset;  //타겟 위치 재설정
        transform.RotateAround(target.position, 
                                                Vector3.up, 
                                                orbitSpeed * Time.deltaTime);
        offset = transform.position - target.position;  //RotateAround()후의 위치를 가지고 목표와의 거리를 유지
    }
}
