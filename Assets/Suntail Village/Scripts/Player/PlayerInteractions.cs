using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

//Interacting with objects and doors
namespace Suntail
{
    public class PlayerInteractions : MonoBehaviour
    {
        #region 정의부
        [Header("Interaction variables")]
        [Tooltip("Layer mask for interactive objects")]
        [SerializeField] private LayerMask interactionLayer;
        [Tooltip("Maximum distance from player to object of interaction")]
        [SerializeField] private float interactionDistance = 2f;
        [Tooltip("Tag for door object")]
        [SerializeField] private string doorTag = "Door";
        [Tooltip("Tag for pickable object")]
        [SerializeField] private string itemTag = "Item";
        [SerializeField] private string castleDoorTag = "CastleDoor";
        [Tooltip("The player's main camera")]
        [SerializeField] private Camera mainCamera;
        [Tooltip("Parent object where the object to be lifted becomes")]
        [SerializeField] private Transform pickupParent;

        [Header("Keybinds")]
        [Tooltip("Interaction key")]
        [SerializeField] private KeyCode interactionKey = KeyCode.E;

        [Header("Object Following")]
        [Tooltip("Minimum speed of the lifted object")]
        [SerializeField] private float minSpeed = 0;
        [Tooltip("Maximum speed of the lifted object")]
        [SerializeField] private float maxSpeed = 3000f;

        [Header("UI")]
        [Tooltip("Background object for text")]
        [SerializeField] private Image uiPanel;
        [Tooltip("Text holder")]
        [SerializeField] private Text panelText;
        [Tooltip("Text when an object can be lifted")]
        [SerializeField] private string itemPickUpText;
        [Tooltip("Text when an object can be drop")]
        [SerializeField] private string itemDropText;
        [Tooltip("Text when the door can be opened")]
        [SerializeField] private string doorOpenText;
        [Tooltip("Text when the door can be closed")]
        [SerializeField] private string doorCloseText;
        [SerializeField] private string castleDoorOpenText;
        [Tooltip("Text when the minigame Two")]
        [SerializeField] private string rotationText;
        [SerializeField] private string miniGameTwoTag = "MiniGameTwo";
        //Private variables.
        private PhysicsObject _physicsObject;
        private PhysicsObject _currentlyPickedUpObject;
        private PhysicsObject _lookObject;
        private Quaternion _lookRotation;
        private Vector3 _raycastPosition;
        private Rigidbody _pickupRigidBody;
        private Door _lookDoor;
        private float _currentSpeed = 0f;
        private float _currentDistance = 0f;
        private CharacterController _characterController;
        [SerializeField] public GameObject _character;
        private CastleDoor _lookCastleDoor;
        private MiniGameTwo _lookMiniGameTwo;
        



        #endregion
        private void Start()
        {
            mainCamera = Camera.main;
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Interactions();
            //LegCheck();
        }

        //Determine which object we are now looking at, depending on the tag and component
        private void Interactions()
        {

            Vector3 boxCastOrigin = _character.transform.position + new Vector3(0, 1.0f, 0);
            if (Physics.BoxCast(boxCastOrigin, _character.transform.lossyScale * 0.5f, _character.transform.forward, out RaycastHit interactionHit, _character.transform.rotation, interactionDistance, interactionLayer))
            {
                if (interactionHit.collider.CompareTag(doorTag))
                {
                    _lookDoor = interactionHit.collider.gameObject.GetComponentInChildren<Door>();
                    ShowDoorUI();
                    if (Input.GetKeyDown(interactionKey))
                    {
                        _lookDoor.PlayDoorAnimation();
                    }
                }
                else if (interactionHit.collider.CompareTag(castleDoorTag))
                {
                    _lookCastleDoor = interactionHit.collider.gameObject.GetComponentInChildren<CastleDoor>();
                    ShowCastleDoorUI();
                    if (Input.GetKeyDown(interactionKey))
                    {
                        _lookCastleDoor.PlayCastleDoorAnimation();
                    }
                }
                else if (interactionHit.collider.CompareTag(miniGameTwoTag))
                {
                    _lookMiniGameTwo = interactionHit.collider.gameObject.GetComponentInChildren<MiniGameTwo>();
                    ShowMiniGameTwoUI();
                    if (Input.GetKeyDown(interactionKey))
                    {
                        _lookMiniGameTwo.PlayMiniGameTwo();
                    }
                }
            }
            else
            {
                _lookCastleDoor = null;
                _lookDoor = null;
                _lookObject = null;
                _lookMiniGameTwo = null;
                uiPanel.gameObject.SetActive(false);
            }

        }
        //Show interface elements when hovering over an object
        private void ShowDoorUI()
        {
            uiPanel.gameObject.SetActive(true);

            if (_lookDoor.doorOpen)
            {
                panelText.text = doorCloseText;
            }
            else
            {
                panelText.text = doorOpenText;
            }
        }
        private void ShowCastleDoorUI()
        {
            uiPanel.gameObject.SetActive(true);

            if (!_lookCastleDoor.castleDoorOpen)
            {
                panelText.text = castleDoorOpenText;
            }
        }
        private void ShowMiniGameTwoUI()
        {
            uiPanel.gameObject.SetActive(true);

            if (!_lookMiniGameTwo.gameClear)
            {
                panelText.text = rotationText;
            }
        }
    }
}