using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type {  Melee, Range }   //무기타입, 데미지, 공속, 범위, 효과 변수생성
    public Type type;   //무기타입
    public int damage;  //데미지
    public float rate;  //공속
    public BoxCollider meleeArea;    //공격 범위가 될 콜라이더의 위치, 크기 조정(해머, 건 구분)
    public TrailRenderer trailEffect;   //효과(이펙트)

    public void Use()   //플레이어가 무기를 사용한다
    {
        if (type == Type.Melee) {   //무기타입이 근접공격이다
            StopCoroutine("Swing");     //같은 코루틴을 새로 시작하기위해 지금 동작하고있는 코루틴을 중지해서 로직이 꼬이지않게함
                                                       //현재 쉬고있는 코루틴도 정지 시킴
            StartCoroutine("Swing");    //코루틴 실행함수   ""사이에 코루틴이름을 써주면됨
            
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
            yield return new WaitForSeconds(0.3f);
            meleeArea.enabled = false;
            yield return new WaitForSeconds(0.3f);
            trailEffect.enabled = false;
        }
        //주어진 수치만큼 기다리는 함수
        //Use() 메인루틴 -> Swing() 서브루틴 -> Use() 메인루틴      교차실행을한다.
        //Use() 메인루틴 +Swing()  = 코루틴 (동시실행하는것을 말한다  Co-Routine)
}
