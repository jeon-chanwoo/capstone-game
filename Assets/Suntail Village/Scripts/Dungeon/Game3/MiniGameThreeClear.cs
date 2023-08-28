using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameThreeClear : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Transform gameClearTextTransform = Camera.main.transform.Find("UI/MiniGameClear");
            Text _text = gameClearTextTransform.GetComponent<Text>();
            _text.CrossFadeAlpha(1, 0, false);
            _text.CrossFadeAlpha(0,5.0f, false);
        }
    }
}
