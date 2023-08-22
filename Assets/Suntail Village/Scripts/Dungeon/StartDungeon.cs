using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartDungeon : MonoBehaviour
{
    [SerializeField] private Image blackScreenImage;
    [SerializeField] private Text blackScreenText1;
    [SerializeField] private float blackScreenDuration = 4f;
    [SerializeField] private float TextDuration = 3f;

    private bool screenTimerIsActive = true;
    void Start()
    {
        blackScreenImage.gameObject.SetActive(true);
        blackScreenText1.gameObject.SetActive(true);
    }

    void Update()
    {
        if (screenTimerIsActive)
        {
            blackScreenDuration -= Time.deltaTime;
            if(blackScreenDuration < 0)
            {
                screenTimerIsActive = false;
                blackScreenImage.CrossFadeAlpha(0, TextDuration, false);
                blackScreenText1.CrossFadeAlpha(0, TextDuration, false);
            }
            
        }
    }
}
