using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DMinimapPosition : MonoBehaviour
{
    [SerializeField] public GameObject _character;
    [SerializeField] public GameObject _characterSpot;
    [SerializeField] private Camera _miniMapCamera;
    [SerializeField] private float scrollSensitivity;
    [SerializeField] private float scaleSensitivity;
    private float minZoom = 10.0f;
    private float maxZoom = 25.0f;
    private float zMinScale = 2.5f;
    private float zMaxScale = 5.5f;
    private float xMinScale = 4.0f;
    private float xMaxScale = 8.8f;

    private bool isLeftControlPressed = false;
    private Vector3 vec = new Vector3(0.0f, -5.0f, 0.0f);

    void Start()
    {
        StartCameraPosition();

    }

    void Update()
    {
        CheckInput();
        CameraPosition();
        SizeADJ();
        
    }
    private void StartCameraPosition()
    {
        Vector3 newPosition = new Vector3(_character.transform.position.x, _character.transform.position.y + 40.0f, _character.transform.position.z);
        transform.position = newPosition;
        _miniMapCamera.orthographicSize = 10.0f;
        _characterSpot.transform.position = newPosition + vec;
        _characterSpot.transform.localScale = new Vector3(4.0f,0.01f,2.5f);



    }

    private void CameraPosition()
    {
        Vector3 newPosition = new Vector3(_character.transform.position.x, transform.position.y, _character.transform.position.z);
        transform.position = newPosition;
        _characterSpot.transform.position = newPosition + vec;
    }

    private void CheckInput()
    {
        // 좌측 컨트롤 키를 누르는 상태 확인
        isLeftControlPressed = Input.GetKey(KeyCode.LeftControl);
    }
    private void SizeADJ()
    {
        if (isLeftControlPressed)
        {
            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
            _miniMapCamera.orthographicSize += -mouseWheel * scrollSensitivity;
            _miniMapCamera.orthographicSize = Mathf.Clamp(_miniMapCamera.orthographicSize, minZoom, maxZoom);

            Vector3 newScale = _characterSpot.transform.localScale;
            newScale.x += -mouseWheel * scaleSensitivity* 1.6f;
            newScale.x = Mathf.Clamp(newScale.x, xMinScale,xMaxScale);

            newScale.z += -mouseWheel * scaleSensitivity;
            newScale.z = Mathf.Clamp(newScale.z, zMinScale, zMaxScale);

            _characterSpot.transform.localScale = newScale;
        }
    }
}
