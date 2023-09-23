using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    [SerializeField] public GameObject _gameStart;
    [SerializeField] public GameObject _option;
    [SerializeField] public GameObject _gameExit;

    private void Start()
    {
        //게임오버 되었을때 마우스 락과 안보임 버그 해결하기 위해 작성
        Cursor.lockState = CursorLockMode.None;// 마우스 커서 풀림
        Cursor.visible = true;//마우스 커서 보이게
    }
    public void SceneChange()
    {
        if (EventSystem.current.currentSelectedGameObject == _gameStart)
        {
            SceneManager.LoadScene("TownScene");
        }
        else if (EventSystem.current.currentSelectedGameObject == _option)
        {
            // 옵션창을 띄우는 로직을 여기에 추가
        }
        else if (EventSystem.current.currentSelectedGameObject == _gameExit)
        {
            //유니티에서와 실제게임어플에서의 종료는 다르다...
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}







