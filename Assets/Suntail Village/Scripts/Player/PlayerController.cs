using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;

/*Simple player movement controller, based on character controller component, 
with footstep system based on check the current texture of the component*/
namespace Suntail
{
    public class PlayerController : MonoBehaviour
    {
        #region 정의부
        //Variables for footstep system list
        [System.Serializable]
        public class GroundLayer
        {
            public string layerName;
            public Texture2D[] groundTextures;
            public AudioClip[] footstepSounds;
        }

        [Header("Stats")]
        [SerializeField] public float _hp; //현재체력
        [SerializeField] public float _maxHP;//최대체력
        [SerializeField] public float _mp;//현제마나
        [SerializeField] public float _maxMP;//최대마나
        [SerializeField] public float _atkPower;//공격력
        [SerializeField] public float _defense;//방어력
        [SerializeField] public float walkSpeed;//이속
        [SerializeField] private float runMultiplier;
        [SerializeField] private float jumpForce;
        private int attackCount = 0;
        private float attackCoolDown = 0.4f;
        private bool isAttackInputDisabled = false;

        [Header("Movement")]
        [SerializeField] public GameObject direction;//바라보는 방향

        [SerializeField] private float gravity = -9.81f;

        [Header("Mouse Look")] 
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float mouseSensivity;
        [SerializeField] private float mouseVerticalClamp;
        [SerializeField] private float scrollSpeed = 2000.0f;
        private int escKeyPressCount = 0;
        private bool isLeftControlPressed = false;
        
        [Header("Keybinds")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

        [Header("Footsteps")]
        [Tooltip("Footstep source")]
        [SerializeField] private AudioSource footstepSource;

        [Tooltip("Distance for ground texture checker")]
        [SerializeField] private float groundCheckDistance = 1.0f;

        [Tooltip("Footsteps playing rate")]
        [SerializeField] [Range(1f, 2f)] private float footstepRate = 1f;

        [Tooltip("Footstep rate when player running")]
        [SerializeField] [Range(1f, 2f)] private float runningFootstepRate = 1.5f;

        [Tooltip("Add textures for this layer and add sounds to be played for this texture")]
        public List<GroundLayer> groundLayers = new List<GroundLayer>();

        //Private movement variables
        private float _horizontalMovement;
        private float _verticalMovement;
        private float _currentSpeed;
        private Vector3 _moveDirection;
        private Vector3 _velocity;
        private CharacterController _characterController;
        private bool _isRunning;
        private bool _isJumping = false;
        private Animator _animator;


        //Private mouselook variables
        private float _verticalRotation;
        private float _yAxis;
        private float _xAxis;
        private bool _activeRotation;

        //Private footstep system variables
        private Terrain _terrain;
        private TerrainData _terrainData;
        private TerrainLayer[] _terrainLayers;
        private AudioClip _previousClip;
        private Texture2D _currentTexture;
        private RaycastHit _groundHit;
        private float _nextFootstep;

        #endregion
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _characterController = GetComponent<CharacterController>();
            GetTerrainData();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //Getting all terrain data for footstep system
        private void GetTerrainData()
        {
            if (Terrain.activeTerrain)
            {
                _terrain = Terrain.activeTerrain;
                _terrainData = _terrain.terrainData;
                _terrainLayers = _terrain.terrainData.terrainLayers;
            }
        }

        private void Update()
        {
            if (_characterController.isGrounded)
            {
                Attack();
            }
            if (!(((_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1")) || 
                (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2")) ||
                (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))) && 
                _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f &&
                _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.0f
                ))
            {
                Jump();
                Movement();
            }
            MouseLook();
            CheckInput();
            if (!isLeftControlPressed)
                Zoom();
            GroundChecker();
            AttackReset();
            
        }
        private void AttackReset()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("WAIT"))
            {
                attackCount = 0;
            }
        }
        private void Attack()
        {
            if (Input.GetMouseButton(0) && !isAttackInputDisabled)
            {
                
                if (attackCount == 0)
                {
                    if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
                    {
                        _animator.Play("Attack1");
                        attackCount++;
                        StartCoroutine(DisableInputForDuration(attackCoolDown));
                    }
                    else
                    {
                        _animator.Play("Attack1");
                        attackCount++;
                        StartCoroutine(DisableInputForDuration(attackCoolDown));
                    }
                   
                }

                else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") &&
                         _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f &&
                         _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f && !isAttackInputDisabled)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        _animator.Play("Attack2");
                        attackCount++;
                        StartCoroutine(DisableInputForDuration(attackCoolDown));
                    }
                }

                else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") &&
                         _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f &&
                         _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f && !isAttackInputDisabled)
                {
                    if (Input.GetMouseButton(0))
                    {
                        _animator.Play("Attack3");
                        attackCount = 0;
                        isAttackInputDisabled = true;
                        StartCoroutine(DisableInputForDuration(attackCoolDown));
                    }
                }
                return;
            }
        }

        private IEnumerator DisableInputForDuration(float delay)
        {
            
            yield return new WaitForSeconds(delay);
            isAttackInputDisabled = false;
            

        }


        private void Jump()
        {
            if (!_isJumping && Input.GetKeyDown(jumpKey) && _characterController.isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpForce * 2f * -gravity);
                _animator.SetBool("jump", true);
                
                StartCoroutine(JumpCoolTime(0.8f));
            }
        }

        //쿨타임계산
        private IEnumerator JumpCoolTime(float cool)
        {
            _isJumping = true; // 점프 중임을 설정
            yield return new WaitForSeconds(cool);

            
            _isJumping = false; // 점프 종료
        }


        private void Movement()
        {
            if (_characterController.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            _horizontalMovement = Input.GetAxis("Horizontal");
            _verticalMovement = Input.GetAxis("Vertical");
            #region 이동방향
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            {
                direction.gameObject.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, 45, 0));
            }else if(Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                direction.gameObject.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, 135, 0));
            }else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            {
                direction.gameObject.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, 225, 0));
            }else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            {
                direction.gameObject.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, 315, 0));
            }else if (Input.GetKey(KeyCode.W))
            {
                direction.gameObject.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, 0, 0));
            }else if (Input.GetKey(KeyCode.D))
            {
                direction.gameObject.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, 90, 0));
            }else if (Input.GetKey(KeyCode.S))
            {
                direction.gameObject.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, 180, 0));
            }else if (Input.GetKey(KeyCode.A))
            {
                direction.gameObject.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, 270, 0));
            }
            else
            {
                return;
            }

            #endregion


            _moveDirection = transform.forward * _verticalMovement + transform.right * _horizontalMovement;

            _isRunning = Input.GetKey(runKey);
            _currentSpeed = walkSpeed * (_isRunning ? runMultiplier : 1f);
            
            _characterController.Move(_moveDirection * _currentSpeed * Time.deltaTime);

            _velocity.y += gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);

        }

        private void MouseLook()
        {   
            _xAxis = Input.GetAxis("Mouse X"); 
            _yAxis = Input.GetAxis("Mouse Y");

            if (Input.GetKeyDown(KeyCode.Escape)) {
                escKeyPressCount++;
                if (escKeyPressCount % 2 == 1)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    mouseSensivity = 0;
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    mouseSensivity = 1;
                }
            }
            else
            {
                _verticalRotation += -_yAxis * mouseSensivity;
                _verticalRotation = Mathf.Clamp(_verticalRotation, -mouseVerticalClamp, mouseVerticalClamp);
                playerCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
                transform.rotation *= Quaternion.Euler(0, _xAxis * mouseSensivity, 0);
            }
            
        }
        private void CheckInput()
        {
            isLeftControlPressed = Input.GetKey(KeyCode.LeftControl);
        }
        private void Zoom()
        {
            if (playerCamera.transform.localPosition.y >= 0.6f && playerCamera.transform.localPosition.y <= 8.0f)
            {
                float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
                Vector3 cameraDirection = this.transform.localRotation * Vector3.forward + this.transform.localRotation * Vector3.down;
                playerCamera.transform.position += cameraDirection * Time.deltaTime * scrollWheel * scrollSpeed;

            }
            else if(playerCamera.transform.localPosition.y < 0.6f)
            {
                playerCamera.transform.localPosition = new Vector3(0.0f, 0.6f, -0.6f);
            }
            else
            {
                playerCamera.transform.localPosition = new Vector3(0.0f, 8.0f, -8.0f);
            }
        }

        
        private void FixedUpdate()
        {
            if (_characterController.isGrounded)
            {
                _isJumping = false;
                _velocity.y += gravity * Time.fixedDeltaTime;
            }
            else
            {
                if (!_isJumping)
                {
                    _velocity.y += (gravity+3.0f) * Time.fixedDeltaTime;
                }
            }
            _animator.SetFloat("speed", _currentSpeed/4.0f);
            if (_characterController.isGrounded && (_horizontalMovement != 0 || _verticalMovement != 0))
            {
                float currentFootstepRate = (_isRunning ? runningFootstepRate : footstepRate);

                if (_nextFootstep >= 100f)
                {
                    {
                        PlayFootstep();
                        _nextFootstep = 0;
                    }
                }
                _nextFootstep += (currentFootstepRate * walkSpeed);
            }
            else
            {
                _animator.SetFloat("speed", 0);
            }

            if (_characterController.isGrounded && _velocity.y < 0)
            {
                _animator.SetBool("jump", false);
            }
            _characterController.Move(_velocity * Time.fixedDeltaTime);
        }

        
        private void GroundChecker()
        {
            Ray checkerRay = new Ray(transform.position + (Vector3.up * 0.1f), Vector3.down);

            if (Physics.Raycast(checkerRay, out _groundHit, groundCheckDistance))
            {
                if (_groundHit.collider.GetComponent<Terrain>())
                {
                    _currentTexture = _terrainLayers[GetTerrainTexture(transform.position)].diffuseTexture;
                }
                if (_groundHit.collider.GetComponent<Renderer>())
                {
                    _currentTexture = GetRendererTexture();
                }
            }
        }

        
        private void PlayFootstep()
        {
            for (int i = 0; i < groundLayers.Count; i++)
            {
                for (int k = 0; k < groundLayers[i].groundTextures.Length; k++)
                {
                    if (_currentTexture == groundLayers[i].groundTextures[k])
                    {
                        footstepSource.PlayOneShot(RandomClip(groundLayers[i].footstepSounds));
                    }
                }
            }
        }

        private float[] GetTerrainTexturesArray(Vector3 controllerPosition)
        {
            _terrain = Terrain.activeTerrain;
            _terrainData = _terrain.terrainData;
            Vector3 terrainPosition = _terrain.transform.position;

            int positionX = (int)(((controllerPosition.x - terrainPosition.x) / _terrainData.size.x) * _terrainData.alphamapWidth);
            int positionZ = (int)(((controllerPosition.z - terrainPosition.z) / _terrainData.size.z) * _terrainData.alphamapHeight);

            float[,,] layerData = _terrainData.GetAlphamaps(positionX, positionZ, 1, 1);

            float[] texturesArray = new float[layerData.GetUpperBound(2) + 1];
            for (int n = 0; n < texturesArray.Length; ++n)
            {
                texturesArray[n] = layerData[0, 0, n];
            }
            return texturesArray;
        }

        
        private int GetTerrainTexture(Vector3 controllerPosition)
        {
            float[] array = GetTerrainTexturesArray(controllerPosition);
            float maxArray = 0;
            int maxArrayIndex = 0;

            for (int n = 0; n < array.Length; ++n)
            {

                if (array[n] > maxArray)
                {
                    maxArrayIndex = n;
                    maxArray = array[n];
                }
            }
            return maxArrayIndex;
        }

        private Texture2D GetRendererTexture()
        {
            Texture2D texture;
            texture = (Texture2D)_groundHit.collider.gameObject.GetComponent<Renderer>().material.mainTexture;
            return texture;
        }

        private AudioClip RandomClip(AudioClip[] clips)
        {
            int attempts = 2;
            footstepSource.pitch = Random.Range(0.9f, 1.1f);

            AudioClip selectedClip = clips[Random.Range(0, clips.Length)];

            while (selectedClip == _previousClip && attempts > 0)
            {
                selectedClip = clips[Random.Range(0, clips.Length)];

                attempts--;
            }
            _previousClip = selectedClip;
            return selectedClip;
        }
    }
}
