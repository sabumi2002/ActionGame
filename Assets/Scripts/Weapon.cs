using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }   //무기타입, 데미지, 공속, 범위, 효과 변수생성
    public Type type;   //무기타입
    public int damage;  //데미지
    public float rate;  //공속
    public int maxAmmo; //최대 탄창
    public int curAmmo; //실제 가지고있는 탄창

    public BoxCollider meleeArea;    //공격 범위가 될 콜라이더의 위치, 크기 조정(해머, 건 구분)
    public TrailRenderer trailEffect;   //효과(이펙트)
    public Transform bulletPos; //프리팹을 생성할 위치
    public GameObject bullet;   //프리팹을 저장할 변수
    public Transform bulletCasePos; //프리팹을 생성할 위치
    public GameObject bulletCase;   //프리팹을 저장할 변수


    public void Use()   //플레이어가 무기를 사용한다
    {
        if (type == Type.Melee)
        {   //무기타입이 근접공격이다
            StopCoroutine("Swing");     //같은 코루틴을 새로 시작하기위해 지금 동작하고있는 코루틴을 중지해서 로직이 꼬이지않게함
                                        //현재 쉬고있는 코루틴도 정지 시킴
            StartCoroutine("Swing");    //코루틴 실행함수   ""사이에 코루틴이름을 써주면됨

        }
        else if (type == Type.Range && curAmmo > 0) //총을 가지고 있고 총알이 0이상일때
        {
            curAmmo--;
            
            StartCoroutine("Shot");
        }


    }

    IEnumerator Swing() //코루틴함수 쓰는법 IEnumerator 단, 반드시 yield을 사용해야함.
    {

        /*1 번 지역에서 로직이 실행되고
        yield return null;  // 여기서 1프레임 대기 (yield키워드를 여러개 사용하여 시간차 로직 작성가능)
        //2 번 구역을 다시 실행
        yield return null;  // 여기서 1프레임 대기
        //3 번 구역을 다시 실행
        yield return null;  // 여기서 1프레임 대기

        yield return new WaitForSeconds(0.1f);  //주어진 수치만큼 기다리는 함수(0.1초 대기)
        yield break;     //이걸로 코루틴 탈출 가능*/

        yield return new WaitForSeconds(0.1f);  //trailRenderer와 BoxCollider를 시간차로 활성화 컨트롤
        meleeArea.enabled = true;   //BoxCollider 활성화
        trailEffect.enabled = true;     //trailRenderer 활성화
        yield return new WaitForSeconds(0.4f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }
    //주어진 수치만큼 기다리는 함수
    //Use() 메인루틴 -> Swing() 서브루틴 -> Use() 메인루틴      교차실행을한다.
    //Use() 메인루틴 +Swing()  = 코루틴 (동시실행하는것을 말한다  Co-Routine)
    IEnumerator Shot() //코루틴함수 쓰는법 IEnumerator 단, 반드시 yield을 사용해야함.
    {
        //#1. 총알발사
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);   //총알 인스턴스화 하기
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;  //만들어둔 bulletPos위치의 z축(파란축)쪽으로 * 50   (속도가 붙는다)

        yield return null;
        //#2. 탄피배출
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);   //총알 인스턴스화 하기
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);//탄피배출을 파란축 반대방향, 위쪽으로 랜덤하게 배출
        caseRigid.AddForce(caseVec, ForceMode.Impulse);  //탄피에 만들어둔 caseVec 힘 가하기 , 즉발(impulse)
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);   //회전함수(addTorque) 탄피에 회전주기
    }
}