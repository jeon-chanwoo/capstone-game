using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOne : MonoBehaviour
{
    public string scaffoldTag = "Scaffold"; // Scaffold의 태그
    public string stepGameLayerName = "StepGame"; // StepGame 레이어 이름
    public int requiredScaffoldCount = 25; // 필요한 Scaffold 개수

    public int scaffoldConversionCount = 3; // 탈락 시 바꿀 Scaffold 개수

    private int stepGameScaffoldCount = 0;
    private bool isGameCleared = false;
    private bool hasDecreasedScaffoldCount = false;

    private void LateUpdate()
    {
        if (!isGameCleared)
        {
            CheckScaffoldStates();
        }
    }

    private void CheckScaffoldStates()
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
            Debug.Log("게임 클리어!");
            DeactivateBrokeColliders();
            isGameCleared = true;
        }
        else if (currentStepGameScaffoldCount < stepGameScaffoldCount && !hasDecreasedScaffoldCount)
        {
            Debug.Log("게임 탈락 - 발판 수 감소");
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
    private void DeactivateBrokeColliders()
    {
        GameObject[] brokeObjects = GameObject.FindGameObjectsWithTag("broke");

        foreach (GameObject brokeObject in brokeObjects)
        {
            BoxCollider boxCollider = brokeObject.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }
        }
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

    public void IncreaseStepGameScaffoldCount()
    {
        stepGameScaffoldCount++;
    }
}