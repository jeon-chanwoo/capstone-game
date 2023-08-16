using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOne : MonoBehaviour
{
    public string scaffoldTag = "Scaffold"; // Scaffold�� �±�
    public string stepGameLayerName = "StepGame"; // StepGame ���̾� �̸�
    public int requiredScaffoldCount = 25; // �ʿ��� Scaffold ����

    public int scaffoldConversionCount = 3; // Ż�� �� �ٲ� Scaffold ����

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
            Debug.Log("���� Ŭ����!");
            DeactivateBrokeColliders();
            isGameCleared = true;
        }
        else if (currentStepGameScaffoldCount < stepGameScaffoldCount && !hasDecreasedScaffoldCount)
        {
            Debug.Log("���� Ż�� - ���� �� ����");
            SetAllScaffoldsDefault();
            ConvertRandomScaffolds();
            hasDecreasedScaffoldCount = true; // Ż���� ���·� ǥ��
        }
        else
        {
            hasDecreasedScaffoldCount = false; // ���� �� ���� ���� �ʱ�ȭ
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

        stepGameScaffoldCount = 0; // ��� Scaffold�� Default�� ������ �� ī��Ʈ �ʱ�ȭ
        isGameCleared = false; // ���� Ŭ���� ���� �ʱ�ȭ
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