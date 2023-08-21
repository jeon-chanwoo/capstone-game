using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScaffoldChange : MonoBehaviour
{
    public string playerTag = "Player"; // 플레이어의 태그
    public string newLayerName = "StepGame"; // 변경할 레이어의 이름
    public string defaultLayerName = "Default"; // 원래 레이어의 이름


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