using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScaffoldChange : MonoBehaviour
{
    public string playerTag = "Player"; // �÷��̾��� �±�
    public string newLayerName = "StepGame"; // ������ ���̾��� �̸�
    public string defaultLayerName = "Default"; // ���� ���̾��� �̸�


    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.layer == LayerMask.NameToLayer("Default") && other.CompareTag(playerTag))
        {
            gameObject.layer = LayerMask.NameToLayer(newLayerName);
        }
        else if ((gameObject.layer == LayerMask.NameToLayer("StepGame") && other.CompareTag(playerTag)))
        {
            gameObject.layer = LayerMask.NameToLayer(defaultLayerName);
        }
    }
}