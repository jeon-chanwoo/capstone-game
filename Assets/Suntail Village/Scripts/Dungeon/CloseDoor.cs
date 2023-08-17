using UnityEngine;

public class StageStart : MonoBehaviour
{
    public Animator otherObjectAnimator; // 다른 객체의 Animator 컴포넌트

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            

            // 다른 객체의 Animator 컴포넌트를 확인하고 애니메이션 재생
            if (otherObjectAnimator != null)
            {
                otherObjectAnimator.SetTrigger("close"); // "PlayAnimation"은 애니메이션 트리거 이름입니다.
            }
        }
    }
}