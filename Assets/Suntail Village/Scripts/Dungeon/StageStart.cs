using UnityEngine;

public class StageStart : MonoBehaviour
{
    public Animator otherObjectAnimator; // �ٸ� ��ü�� Animator ������Ʈ

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("1�� ����");

            // �ٸ� ��ü�� Animator ������Ʈ�� Ȯ���ϰ� �ִϸ��̼� ���
            if (otherObjectAnimator != null)
            {
                otherObjectAnimator.SetTrigger("close"); // "PlayAnimation"�� �ִϸ��̼� Ʈ���� �̸��Դϴ�.
            }
        }
    }
}