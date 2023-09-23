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
    private Vector3 rotationAmount = new Vector3(0f, 0f, 90.0f); // 회전량 (Y축 기준)
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
        //유니티에서 회전을 할시 맵에있는 굴곡에 영향을 받기 때문에 정확히 0이 나올수 없어서(ex)0.02)
        //보정해 주기위한 함수(내림 함수) 모든 석상이 0일때 클리어
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
        //플레이어가 상호작용 할 시 왼쪽(왼쪽+중앙)/중앙(왼쪽+중앙+오른쪽)/오른쪽(중앙+오른쪽)이 회전하게 한다.
        if (!gameClear && !isRotating)
        {
            isRotating = true;

            if (_rotationAudioSource != null && rotationSound != null)
            {
                _rotationAudioSource.PlayOneShot(rotationSound); //공통사운드
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

    private IEnumerator RotateCubes(params GameObject[] cubes)//큐브를 배열에 넣는다.
    {
        Quaternion[] startRotations = new Quaternion[cubes.Length];// 그 큐브의 회전 전 각도
        Quaternion[] endRotations = new Quaternion[cubes.Length];//그 큐브의 회전 후 각도

        for (int i = 0; i < cubes.Length; i++)
        {
            startRotations[i] = cubes[i].transform.rotation;
            endRotations[i] = cubes[i].transform.rotation * Quaternion.Euler(rotationAmount);//회전
        }

        float elapsedTime = 0f;
        float rotationTime = 1f; // 회전하는데 걸리는 시간 (초)

        while (elapsedTime < rotationTime)
        {
            for (int i = 0; i < cubes.Length; i++)
            {
                cubes[i].transform.rotation = Quaternion.Slerp(startRotations[i], endRotations[i], elapsedTime / rotationTime);//Slerp를 통해 부드럽게 회전
            }

            elapsedTime += Time.deltaTime;//게임내에서 흐르는 시간을 받아온다.
            yield return null;
        }

        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].transform.rotation = endRotations[i];
        }

        isRotating = false;

    }
}