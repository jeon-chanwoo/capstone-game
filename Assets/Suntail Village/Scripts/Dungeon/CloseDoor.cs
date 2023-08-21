using UnityEngine;

public class StageStart : MonoBehaviour
{
    public Animator otherObjectAnimator1; // 다른 객체의 Animator 컴포넌트
    public Animator otherObjectAnimator2;
    public Animator otherObjectAnimator3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // stage count 넣기

            // 다른 객체의 Animator 컴포넌트를 확인하고 애니메이션 재생
            if (otherObjectAnimator1 != null && otherObjectAnimator2 != null && otherObjectAnimator3 != null)
            {
                otherObjectAnimator1.SetTrigger("close"); // "PlayAnimation"은 애니메이션 트리거 이름입니다.
                otherObjectAnimator2.SetTrigger("close");
                otherObjectAnimator3.SetTrigger("close");

            }
        }
    }
}