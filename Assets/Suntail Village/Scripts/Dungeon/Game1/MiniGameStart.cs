using System.Collections;
using UnityEngine;

public class MiniGameStart : MonoBehaviour
{
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerExit(Collider other) //캐릭터가 콜리더에서 나갔을때
    {
        if (other.CompareTag("Player"))
        {
            boxCollider.isTrigger = false;//트리거를 꺼줌으로써 객체를 통과 못하게 한다.
        }
    }
}