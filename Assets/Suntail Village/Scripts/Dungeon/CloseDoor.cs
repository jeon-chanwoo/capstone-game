using UnityEngine;

public class StageStart : MonoBehaviour
{
    public Animator otherObjectAnimator; // �ٸ� ��ü�� Animator ������Ʈ

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            

            // �ٸ� ��ü�� Animator ������Ʈ�� Ȯ���ϰ� �ִϸ��̼� ���
            if (otherObjectAnimator != null)
            {
                otherObjectAnimator.SetTrigger("close"); // "PlayAnimation"�� �ִϸ��̼� Ʈ���� �̸��Դϴ�.
            }
        }
    }
}