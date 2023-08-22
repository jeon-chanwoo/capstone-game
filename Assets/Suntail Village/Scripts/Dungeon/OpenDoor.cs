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
    public Animator otherObjectAnimator1; //���幮
    public Animator otherObjectAnimator2; //������
    public Animator otherObjectAnimator3; //������

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

            
            
            //ĳ���� �����̵�
            //���幮+������ 2�� ����
        }
        #region 1stage
        if (stageCount == 1)
        {
            //1�������� �̴ϰ��� �����
        }
        #endregion
    }
}
