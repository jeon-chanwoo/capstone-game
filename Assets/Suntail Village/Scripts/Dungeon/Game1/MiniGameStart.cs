using System.Collections;
using UnityEngine;

public class MiniGameStart : MonoBehaviour
{
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            boxCollider.isTrigger = false;
        }
    }
}