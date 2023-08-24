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
        Cursor.lockState = CursorLockMode.None;// ���콺 Ŀ�� Ǯ��
        Cursor.visible = true;//���콺 Ŀ�� ���̰�
    }
    public void SceneChange()
    {
        if (EventSystem.current.currentSelectedGameObject == _gameStart)
        {
            SceneManager.LoadScene("TownScene");
        }
        else if (EventSystem.current.currentSelectedGameObject == _option)
        {
            // �ɼ�â�� ���� ������ ���⿡ �߰�
        }
        else if (EventSystem.current.currentSelectedGameObject == _gameExit)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}







