using UnityEngine;

public class StageStart : MonoBehaviour
{
    public Animator otherObjectAnimator1; // �ٸ� ��ü�� Animator ������Ʈ
    public Animator otherObjectAnimator2;
    public Animator otherObjectAnimator3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // stage count �ֱ�

            // �ٸ� ��ü�� Animator ������Ʈ�� Ȯ���ϰ� �ִϸ��̼� ���
            if (otherObjectAnimator1 != null && otherObjectAnimator2 != null && otherObjectAnimator3 != null)
            {
                otherObjectAnimator1.SetTrigger("close"); // "PlayAnimation"�� �ִϸ��̼� Ʈ���� �̸��Դϴ�.
                otherObjectAnimator2.SetTrigger("close");
                otherObjectAnimator3.SetTrigger("close");

            }
        }
    }
}