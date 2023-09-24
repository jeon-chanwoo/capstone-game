using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] private Image blackScreenImage;
    [SerializeField] private Text blackScreenText;
    private GameObject player;
    private GameObject game;
    private CharacterController playerController;
    private Vector3 stageStartPosition;
    [SerializeField] private float TextDuration = 3f;
    public Animator otherObjectAnimator1; //입장문
    public Animator otherObjectAnimator2; //보스문
    public Animator otherObjectAnimator3; //보스문
    private BackGroundMusic backGroundMusic;

    public int stageCount = 0;

    private void Start()
    {
        backGroundMusic = FindObjectOfType<BackGroundMusic>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<CharacterController>();
        stageStartPosition = new Vector3(2.5f, -8.5f, 85.0f);//플레이어 캐릭터 옮길 위치
    }

    private void OnTriggerEnter(Collider other)//플레이어가 콜리더에 닿았을 때
    {
        if (other.CompareTag("Player"))
        {
            stageCount++;//스테이지 카운터
            blackScreenImage.CrossFadeAlpha(1, 0, false);//다시 보이게
            blackScreenText.gameObject.SetActive(true);//올라가는 중
            backGroundMusic.StartEnterMusic();
            StartCoroutine(TransitionAnimation());
            ForceMove();//강제 이동
            //모든 문 오픈
            if (otherObjectAnimator1 != null && otherObjectAnimator2 != null && otherObjectAnimator3 != null)
            {
                otherObjectAnimator1.SetTrigger("open");
                otherObjectAnimator2.SetTrigger("open");
                otherObjectAnimator3.SetTrigger("open");
            }


            #region stage
            //스테이지에 맞게 미니게임 제거
            if (stageCount == 1)
            {
                game = GameObject.Find("MiniGame(Clone)");
                Destroy(game.gameObject);
            }
            if (stageCount == 2)
            {
                game = GameObject.Find("MiniGame2(Clone)");
                Destroy(game.gameObject);
            }
            if (stageCount == 3)
            {
                game = GameObject.Find("MiniGame3(Clone)");
                Destroy(game.gameObject);
            }
            #endregion
        }

    }
    private IEnumerator TransitionAnimation()
    {
        yield return new WaitForSeconds(4.0f);
        blackScreenImage.CrossFadeAlpha(0, TextDuration, false);
        blackScreenText.CrossFadeAlpha(0, TextDuration, false);
    }
    private void ForceMove()
        //플레이어의 컨트롤러를 껐다키는 사이에 강제이동
        //컨트롤러가 켜져있을때는 강제이동이 안된다.
    {
        playerController.enabled = false;
        player.transform.position = stageStartPosition;
        playerController.enabled = true;
    }
}
