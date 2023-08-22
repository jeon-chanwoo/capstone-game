using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] private Image blackScreenImage;
    [SerializeField] private Text blackScreenText;
    private GameObject player;
    private CharacterController characterController;
    private Vector3 stageStartPosition;
    [SerializeField] private float blackScreenDuration = 4f;
    [SerializeField] private float TextDuration = 3f;
    public Animator otherObjectAnimator1; //입장문
    public Animator otherObjectAnimator2; //보스문
    public Animator otherObjectAnimator3; //보스문

    public int stageCount = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stageCount++;
            blackScreenImage.CrossFadeAlpha(1, 0, false);
            blackScreenText.gameObject.SetActive(true);
            blackScreenImage.CrossFadeAlpha(0, TextDuration, false);
            blackScreenText.CrossFadeAlpha(0, TextDuration, false);

            
            
            //캐릭터 강제이동
            //입장문+보스문 2개 열기
        }
        #region 1stage
        if (stageCount == 1)
        {
            //1스테이지 미니게임 지우기
        }
        #endregion
    }
}
