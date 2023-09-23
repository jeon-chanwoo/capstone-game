using Suntail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOne : MonoBehaviour
{
    public string scaffoldTag = "Scaffold"; // Scaffold�� �±�
    public string stepGameLayerName = "StepGame"; // StepGame ���̾� �̸�
    public int requiredScaffoldCount = 25; // �ʿ��� Scaffold ����
    public int scaffoldConversionCount = 3; // Ż�� �� �ٲ� Scaffold ����
    private int stepGameScaffoldCount = 0; //���� Scaffold����
    public bool isGameCleared = false; //Ŭ���� ����
    private bool hasDecreasedScaffoldCount = false; // Ż���� ���� Ȯ��
    private GameObject player;
    private CharacterController playerController;
    private Vector3 gameOneStartPosition;
    private GameObject _wall;
    
    private void Start()
    {
        _wall = GameObject.Find("MiniGame(Clone)/Game1/Base/Cube (3)");
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<CharacterController>();
        gameOneStartPosition = new Vector3(2.8f, -4.0f, 62.8f);//�̴ϰ��� ���� ��ġ
    }
    private void LateUpdate()
    {
        CheckScaffoldStates(); //��������� Scaffold ����üũ
    }

    private void CheckScaffoldStates()
    {
        if (!isGameCleared)
        {
            //Scaffold�� �迭�� �� ������ �´�.
            GameObject[] scaffolds = GameObject.FindGameObjectsWithTag(scaffoldTag);

            int currentStepGameScaffoldCount = 0; //Scaffold�� ������ Ȯ���� ����

            foreach (GameObject scaffold in scaffolds) //�迭 �� Ȯ��
            {
                if (scaffold.layer == LayerMask.NameToLayer(stepGameLayerName))//���̾��� ������ŭ ī��Ʈ ����
                {
                    currentStepGameScaffoldCount++;
                }
            }

            if (currentStepGameScaffoldCount == requiredScaffoldCount)//���� Ŭ���� ���ǰ� ��ġ������
            {
                Transform gameClearTextTransform = Camera.main.transform.Find("UI/MiniGameClear");
                Text _text = gameClearTextTransform.GetComponent<Text>();
                _text.gameObject.SetActive(true);
                _text.CrossFadeAlpha(0, 5f, false);
                DeactivateBrokeColliders();//������ �������� ���� ���� ���̻� �������ʰ� �ϴ� �Լ�
                isGameCleared = true;
            }
            else if (currentStepGameScaffoldCount < stepGameScaffoldCount && !hasDecreasedScaffoldCount) //����Ŭ���� ����:��Ҵ� ������ �ߺ��ؼ� �������, ���� ������ �پ��� ��
            {
                ForceMove(); //���� ������ġ�� �����̵�
                SetAllScaffoldsDefault(); //Scaffold�� ���¸� �⺻���·� �ǵ����°�
                ConvertRandomScaffolds(); //���� �ʱ�ȭ�� �������� ���� ����
                hasDecreasedScaffoldCount = true; // Ż���� ���·� ǥ��
            }
            else
            {
                hasDecreasedScaffoldCount = false; // Ż�� ���·� �����ְ� ���ִ� ���Լ�
            }

            stepGameScaffoldCount = currentStepGameScaffoldCount;//���� ���ǰ��� ����
        }
    }
    private void DeactivateBrokeColliders()
        //���� Ŭ����� ������ �����ϰ�, ���� �� ������ ���̻� ���� �ʰ� �ϴ� �Լ�
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
        //���� ���н� ĳ���� �����̵�
    {
        BoxCollider boxCollider = _wall.GetComponent<BoxCollider>();
        playerController.enabled = false;
        player.transform.position = gameOneStartPosition;
        playerController.enabled = true;
        boxCollider.isTrigger = true;//�̴ϰ��� ������� Ʈ���Ű� �ߵ��Ҽ� �ְ� ����

    }
    public void SetAllScaffoldsDefault()
    {
        //���Ǽ��� ���� �ʱ�ȭ ��Ű�� �Լ�
        GameObject[] scaffolds = GameObject.FindGameObjectsWithTag(scaffoldTag);

        foreach (GameObject scaffold in scaffolds)
        {
            scaffold.layer = LayerMask.NameToLayer("Default");
        }

        stepGameScaffoldCount = 0; // ��� Scaffold�� Default�� ������ �� ī��Ʈ �ʱ�ȭ
        isGameCleared = false; // ���� Ŭ���� ���� �ʱ�ȭ
    }

    public void ConvertRandomScaffolds()
        //���� ���н� �������� 3���� ������ ���� ���·� ���ϴ� �Լ�
    {
        GameObject[] scaffolds = GameObject.FindGameObjectsWithTag(scaffoldTag);//�ʱ�ȭ ���Ķ� ��� ������ ������´�.
        List<GameObject> scaffoldList = new List<GameObject>(scaffolds);//����Ʈ�� �ִ´�.
        int scaffoldsToConvert = Mathf.Min(scaffoldConversionCount, scaffoldList.Count);//���� �� ���� �Լ� ��ȯ ->3��ȯ�Ѵ�.

        for (int i = 0; i < scaffoldsToConvert; i++)
        {
            int randomIndex = Random.Range(0, scaffoldList.Count);
            GameObject randomScaffold = scaffoldList[randomIndex];//�������� �ϳ� ����

            randomScaffold.layer = LayerMask.NameToLayer(stepGameLayerName);//���õ� ���̾� ����->���� ���·� ����
            scaffoldList.RemoveAt(randomIndex);//�̹� ���õ� ������ �� ���õ��� �ʵ��� ������
        }
    }
}