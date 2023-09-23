using System.Collections;
using UnityEngine;

public class MiniGameStart : MonoBehaviour
{
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerExit(Collider other) //ĳ���Ͱ� �ݸ������� ��������
    {
        if (other.CompareTag("Player"))
        {
            boxCollider.isTrigger = false;//Ʈ���Ÿ� �������ν� ��ü�� ��� ���ϰ� �Ѵ�.
        }
    }
}