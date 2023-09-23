using Suntail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOne : MonoBehaviour
{
    public string scaffoldTag = "Scaffold"; // Scaffold의 태그
    public string stepGameLayerName = "StepGame"; // StepGame 레이어 이름
    public int requiredScaffoldCount = 25; // 필요한 Scaffold 개수
    public int scaffoldConversionCount = 3; // 탈락 시 바꿀 Scaffold 개수
    private int stepGameScaffoldCount = 0; //현재 Scaffold개수
    public bool isGameCleared = false; //클리어 여부
    private bool hasDecreasedScaffoldCount = false; // 탈락한 상태 확인
    private GameObject player;
    private CharacterController playerController;
    private Vector3 gameOneStartPosition;
    private GameObject _wall;
    
    private void Start()
    {
        _wall = GameObject.Find("MiniGame(Clone)/Game1/Base/Cube (3)");
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<CharacterController>();
        gameOneStartPosition = new Vector3(2.8f, -4.0f, 62.8f);//미니게임 생성 위치
    }
    private void LateUpdate()
    {
        CheckScaffoldStates(); //상시적으로 Scaffold 상태체크
    }

    private void CheckScaffoldStates()
    {
        if (!isGameCleared)
        {
            //Scaffold를 배열로 다 가지고 온다.
            GameObject[] scaffolds = GameObject.FindGameObjectsWithTag(scaffoldTag);

            int currentStepGameScaffoldCount = 0; //Scaffold의 개수를 확인할 변수

            foreach (GameObject scaffold in scaffolds) //배열 내 확인
            {
                if (scaffold.layer == LayerMask.NameToLayer(stepGameLayerName))//레이어의 개수만큼 카운트 증가
                {
                    currentStepGameScaffoldCount++;
                }
            }

            if (currentStepGameScaffoldCount == requiredScaffoldCount)//게임 클리어 조건과 일치했을때
            {
                Transform gameClearTextTransform = Camera.main.transform.Find("UI/MiniGameClear");
                Text _text = gameClearTextTransform.GetComponent<Text>();
                _text.gameObject.SetActive(true);
                _text.CrossFadeAlpha(0, 5f, false);
                DeactivateBrokeColliders();//게임이 끝났을때 발판 색이 더이상 변하지않게 하는 함수
                isGameCleared = true;
            }
            else if (currentStepGameScaffoldCount < stepGameScaffoldCount && !hasDecreasedScaffoldCount) //게임클리어 실패:밟았던 발판을 중복해서 밟았을때, 발판 개수가 줄었을 때
            {
                ForceMove(); //게임 시작위치로 강제이동
                SetAllScaffoldsDefault(); //Scaffold의 상태를 기본상태로 되돌리는것
                ConvertRandomScaffolds(); //게임 초기화시 랜덤으로 발판 생성
                hasDecreasedScaffoldCount = true; // 탈락한 상태로 표시
            }
            else
            {
                hasDecreasedScaffoldCount = false; // 탈락 상태로 갈수있게 해주는 불함수
            }

            stepGameScaffoldCount = currentStepGameScaffoldCount;//현재 발판개수 변동
        }
    }
    private void DeactivateBrokeColliders()
        //게임 클리어시 투명벽을 제거하고, 발판 색 변경을 더이상 하지 않게 하는 함수
    {
        GameObject[] brokeObjects = GameObject.FindGameObjectsWithTag("broke");
        GameObject[] scaffolds = GameObject.FindGameObjectsWithTag("Scaffold");
        foreach (GameObject brokeObject in brokeObjects)
        {
            BoxCollider boxCollider = brokeObject.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }
        }
        foreach (GameObject scaffold in scaffolds)
        {
            BoxCollider boxCollider = scaffold.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }
        }

    }
    private void ForceMove()
        //게임 실패시 캐릭터 강제이동
    {
        BoxCollider boxCollider = _wall.GetComponent<BoxCollider>();
        playerController.enabled = false;
        player.transform.position = gameOneStartPosition;
        playerController.enabled = true;
        boxCollider.isTrigger = true;//미니게임 재입장시 트리거가 발동할수 있게 해줌

    }
    public void SetAllScaffoldsDefault()
    {
        //발판수를 완전 초기화 시키는 함수
        GameObject[] scaffolds = GameObject.FindGameObjectsWithTag(scaffoldTag);

        foreach (GameObject scaffold in scaffolds)
        {
            scaffold.layer = LayerMask.NameToLayer("Default");
        }

        stepGameScaffoldCount = 0; // 모든 Scaffold를 Default로 변경할 때 카운트 초기화
        isGameCleared = false; // 게임 클리어 상태 초기화
    }

    public void ConvertRandomScaffolds()
        //게임 실패시 랜덤으로 3개의 발판이 켜진 상태로 변하는 함수
    {
        GameObject[] scaffolds = GameObject.FindGameObjectsWithTag(scaffoldTag);//초기화 이후라 모든 발판을 가지고온다.
        List<GameObject> scaffoldList = new List<GameObject>(scaffolds);//리스트에 넣는다.
        int scaffoldsToConvert = Mathf.Min(scaffoldConversionCount, scaffoldList.Count);//둘중 더 작은 함수 반환 ->3반환한다.

        for (int i = 0; i < scaffoldsToConvert; i++)
        {
            int randomIndex = Random.Range(0, scaffoldList.Count);
            GameObject randomScaffold = scaffoldList[randomIndex];//랜덤으로 하나 선택

            randomScaffold.layer = LayerMask.NameToLayer(stepGameLayerName);//선택된 레이어 변경->밟힌 상태로 변경
            scaffoldList.RemoveAt(randomIndex);//이미 선택된 발판은 또 선택되지 않도록 제거하
        }
    }
}