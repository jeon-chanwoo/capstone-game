using UnityEngine;

public class CreateGame1 : MonoBehaviour
{
    

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            // �ٸ� ��ü�� Animator ������Ʈ�� Ȯ���ϰ� �ִϸ��̼� ���
        }
    }
}