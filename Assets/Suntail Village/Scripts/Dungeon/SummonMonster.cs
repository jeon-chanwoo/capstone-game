using UnityEngine;

public class SummonMonster : MonoBehaviour
{
    public GameObject monsterPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(monsterPrefab != null)
            {
                Instantiate(monsterPrefab,new Vector3(2.1f,0.1f,0f), Quaternion.identity);
            }
            // �ٸ� ��ü�� Animator ������Ʈ�� Ȯ���ϰ� �ִϸ��̼� ���
        }
    }
}