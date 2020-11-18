using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : MyBullet
{
    public Transform target;
    NavMeshAgent nav;
    void Awake()    //초기화
    {
        nav = GetComponent<NavMeshAgent>();
    }

    
    void Update()
    {
        nav.SetDestination(target.position);    //추적
    }
}
