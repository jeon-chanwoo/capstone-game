using UnityEngine;

public class CreateGame1 : MonoBehaviour
{
    public GameObject game1Prefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (game1Prefab != null)
            {
                Instantiate(game1Prefab, new Vector3(9.7f, -59.1f, -4.1f), Quaternion.identity);
            }
            // �ٸ� ��ü�� Animator ������Ʈ�� Ȯ���ϰ� �ִϸ��̼� ���
        }
    }
}