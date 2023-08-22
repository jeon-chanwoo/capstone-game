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
    private GameObject game1;
    private CharacterController playerController;
    private Vector3 stageStartPosition;
    [SerializeField] private GameOne gameOne;
    [SerializeField] private float blackScreenDuration = 4f;
    [SerializeField] private float TextDuration = 3f;
    public Animator otherObjectAnimator1; //입장문
    public Animator otherObjectAnimator2; //보스문
    public Animator otherObjectAnimator3; //보스문

    public int stageCount = 0;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<CharacterController>();
        stageStartPosition = new Vector3(2.5f, -8.5f, 85.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stageCount++;
            blackScreenImage.CrossFadeAlpha(1, 0, false);//다시보이게
            blackScreenText.gameObject.SetActive(true);//올라가는중
            StartCoroutine(TransitionAnimation());
            ForceMove();
            if (otherObjectAnimator1 != null && otherObjectAnimator2 != null && otherObjectAnimator3 != null)
            {
                otherObjectAnimator1.SetTrigger("open");
                otherObjectAnimator2.SetTrigger("open");
                otherObjectAnimator3.SetTrigger("open");
            }


            #region 1stage
            if (stageCount == 1)
            {
                game1 = GameObject.Find("MiniGame(Clone)");
                Destroy(game1.gameObject);
            }
            #endregion
        }

    }
    private IEnumerator TransitionAnimation()
    {
        blackScreenImage.CrossFadeAlpha(0, TextDuration, false);
        yield return new WaitForSeconds(0.1f);
        blackScreenText.CrossFadeAlpha(0, TextDuration, false);
    }
    private void ForceMove()
    {
        playerController.enabled = false;
        player.transform.position = stageStartPosition;
        playerController.enabled = true;
    }
}
