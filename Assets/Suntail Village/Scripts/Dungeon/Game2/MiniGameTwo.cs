using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameTwo : MonoBehaviour
{
    [Tooltip("Rotation sound")]
    [SerializeField] private AudioClip rotationSound;

    public bool gameClear = false;
    private AudioSource _rotationAudioSource;
    private GameObject leftCube;
    private GameObject centerCube;
    private GameObject rightCube;
    private Vector3 rotationAmount = new Vector3(0f, 0f, 90.0f); // ȸ���� (Y�� ����)
    private bool isRotating = false;
    private bool endGame = false;
    private GameObject _wall;

    private void Awake()
    {
        _wall = GameObject.Find("GameTwoWall");
        _rotationAudioSource = gameObject.GetComponent<AudioSource>();
        leftCube = GameObject.Find("LeftCube");
        centerCube = GameObject.Find("CenterCube");
        rightCube = GameObject.Find("RightCube");
    }
    public void Update()
    {
        GameClear();
    }
    public void GameClear()
    {
        //����Ƽ���� ȸ���� �ҽ� �ʿ��ִ� ��� ������ �ޱ� ������ ��Ȯ�� 0�� ���ü� ���(ex)0.02)
        //������ �ֱ����� �Լ�(���� �Լ�) ��� ������ 0�϶� Ŭ����
        if ((Mathf.Floor(leftCube.transform.rotation.eulerAngles.y) == 0) &&
           (Mathf.Floor(rightCube.transform.rotation.eulerAngles.y) == 0) &&
          (Mathf.Floor(centerCube.transform.rotation.eulerAngles.y) == 0) && !endGame)
        {

            gameClear = true;
            endGame = true;
            Transform gameClearTextTransform = Camera.main.transform.Find("UI/MiniGameClear");
            Text _text = gameClearTextTransform.GetComponent<Text>();
            _text.CrossFadeAlpha(1, 0, false);
            _text.CrossFadeAlpha(0, 5f, false);

            BoxCollider boxCollider = _wall.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }
        }
    }
    public void PlayMiniGameTwo()
    {
        //�÷��̾ ��ȣ�ۿ� �� �� ����(����+�߾�)/�߾�(����+�߾�+������)/������(�߾�+������)�� ȸ���ϰ� �Ѵ�.
        if (!gameClear && !isRotating)
        {
            isRotating = true;

            if (_rotationAudioSource != null && rotationSound != null)
            {
                _rotationAudioSource.PlayOneShot(rotationSound); //�������
            }

            if (gameObject.name == "LeftCube")
            {
                StartCoroutine(RotateCubes(leftCube, centerCube));
            }
            else if (gameObject.name == "CenterCube")
            {
                StartCoroutine(RotateCubes(leftCube, centerCube, rightCube));
            }
            else if (gameObject.name == "RightCube")
            {
                StartCoroutine(RotateCubes(centerCube, rightCube));
            }
        }
    }

    private IEnumerator RotateCubes(params GameObject[] cubes)//ť�긦 �迭�� �ִ´�.
    {
        Quaternion[] startRotations = new Quaternion[cubes.Length];// �� ť���� ȸ�� �� ����
        Quaternion[] endRotations = new Quaternion[cubes.Length];//�� ť���� ȸ�� �� ����

        for (int i = 0; i < cubes.Length; i++)
        {
            startRotations[i] = cubes[i].transform.rotation;
            endRotations[i] = cubes[i].transform.rotation * Quaternion.Euler(rotationAmount);//ȸ��
        }

        float elapsedTime = 0f;
        float rotationTime = 1f; // ȸ���ϴµ� �ɸ��� �ð� (��)

        while (elapsedTime < rotationTime)
        {
            for (int i = 0; i < cubes.Length; i++)
            {
                cubes[i].transform.rotation = Quaternion.Slerp(startRotations[i], endRotations[i], elapsedTime / rotationTime);//Slerp�� ���� �ε巴�� ȸ��
            }

            elapsedTime += Time.deltaTime;//���ӳ����� �帣�� �ð��� �޾ƿ´�.
            yield return null;
        }

        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].transform.rotation = endRotations[i];
        }

        isRotating = false;

    }
}