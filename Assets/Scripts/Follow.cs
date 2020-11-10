using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;    //이카메라가 따라가야할 타겟
    public Vector3 offset;  //따라갈 목표와 위치 오프셋을 public 변수로 선언


    private void Update()
    {
        transform.position = target.position + offset;
    }
}
