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
    private int stepGameScaffoldCount = 0;
    public bool isGameCleared = false;
    private bool hasDecreasedScaffoldCount = false;
    private GameObject player;
    private CharacterController playerController;
    private Vector3 gameOneStartPosition;
    private GameObject _wall;
    
    private void Start()
    {
        _wall = GameObject.Find("MiniGame(Clone)/Game1/Base/Cube (3)");
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<CharacterController>();
        gameOneStartPosition = new Vector3(2.8f, -4.0f, 62.8f);
    }
    private void LateUpdate()
    {
        CheckScaffoldStates();
    }

    private void CheckScaffoldStates()
    {
        if (!isGameCleared)
        {
            GameObject[] scaffolds = GameObject.FindGameObjectsWithTag(scaffoldTag);

            int currentStepGameScaffoldCount = 0;

            foreach (GameObject scaffold in scaffolds)
            {
                if (scaffold.layer == LayerMask.NameToLayer(stepGameLayerName))
                {
                    currentStepGameScaffoldCount++;
                }
            }

            if (currentStepGameScaffoldCount == requiredScaffoldCount)
            {
                Transform gameClearTextTransform = Camera.main.transform.Find("UI/MiniGameClear");
                Text _text = gameClearTextTransform.GetComponent<Text>();
                _text.gameObject.SetActive(true);
                _text.CrossFadeAlpha(0, 5f, false);
                DeactivateBrokeColliders();
                isGameCleared = true;
            }
            else if (currentStepGameScaffoldCount < stepGameScaffoldCount && !hasDecreasedScaffoldCount)
            {
                ForceMove();
                SetAllScaffoldsDefault();
                ConvertRandomScaffolds();
                hasDecreasedScaffoldCount = true; // 탈락한 상태로 표시
            }
            else
            {
                hasDecreasedScaffoldCount = false; // 발판 수 감소 상태 초기화
            }

            stepGameScaffoldCount = currentStepGameScaffoldCount;
        }
    }
    private void DeactivateBrokeColliders()
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
    {
        BoxCollider boxCollider = _wall.GetComponent<BoxCollider>();
        playerController.enabled = false;
        player.transform.position = gameOneStartPosition;
        playerController.enabled = true;
        boxCollider.isTrigger = true;

    }
    public void SetAllScaffoldsDefault()
    {
        GameObject[] scaffolds = GameObject.FindGameObjectsWithTag(scaffoldTag);

        foreach (GameObject scaffold in scaffolds)
        {
            scaffold.layer = LayerMask.NameToLayer("Default");
        }

        stepGameScaffoldCount = 0; // 모든 Scaffold를 Default로 변경할 때 카운트 초기화
        isGameCleared = false; // 게임 클리어 상태 초기화
    }

    public void ConvertRandomScaffolds()
    {
        GameObject[] scaffolds = GameObject.FindGameObjectsWithTag(scaffoldTag);
        List<GameObject> scaffoldList = new List<GameObject>(scaffolds);
        int scaffoldsToConvert = Mathf.Min(scaffoldConversionCount, scaffoldList.Count);

        for (int i = 0; i < scaffoldsToConvert; i++)
        {
            int randomIndex = Random.Range(0, scaffoldList.Count);
            GameObject randomScaffold = scaffoldList[randomIndex];

            randomScaffold.layer = LayerMask.NameToLayer(stepGameLayerName);
            scaffoldList.RemoveAt(randomIndex);
        }
    }
}