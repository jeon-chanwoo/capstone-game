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
            // 다른 객체의 Animator 컴포넌트를 확인하고 애니메이션 재생
        }
    }
}