using UnityEngine;

public class CreateGame1 : MonoBehaviour
{
    

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            // 다른 객체의 Animator 컴포넌트를 확인하고 애니메이션 재생
        }
    }
}