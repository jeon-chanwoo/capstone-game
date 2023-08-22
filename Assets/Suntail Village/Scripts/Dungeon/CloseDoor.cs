using UnityEngine;

public class StageStart : MonoBehaviour
{
    public Animator otherObjectAnimator1; // 다른 객체의 Animator 컴포넌트
    public Animator otherObjectAnimator2;
    public Animator otherObjectAnimator3;
    public GameObject game1Prefab;
    public GameObject monster1Prefab;
    public int stageCount=0;

    private void OnTriggerExit(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            stageCount++;
            if (otherObjectAnimator1 != null && otherObjectAnimator2 != null && otherObjectAnimator3 != null)
            {
                otherObjectAnimator1.SetTrigger("close"); // "PlayAnimation"은 애니메이션 트리거 이름입니다.
                otherObjectAnimator2.SetTrigger("close");
                otherObjectAnimator3.SetTrigger("close");
            }

            #region 1stage
            if (stageCount == 1)
            {
                if (game1Prefab != null)
                {
                    Instantiate(game1Prefab, new Vector3(9.7f, -59.1f, -4.1f), Quaternion.identity);
                }
                else if (monster1Prefab != null)
                {
                    Instantiate(monster1Prefab, new Vector3(2.1f, 0.1f, 0f), Quaternion.identity);
                }
                else return;
            }
            #endregion
        }

    }
}