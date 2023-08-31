using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using TMPro;
using TMPro.Examples;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        #region state
        [Header("Stats")]
        [SerializeField] public float _hp; //현재체력
        [SerializeField] public float _maxHP;//최대체력
        [SerializeField] public float _mp;//현제마나
        [SerializeField] public float _maxMP;//최대마나
        [SerializeField] public float _atkPower;//공격력
        [SerializeField] public float _defense;//방어력
        [SerializeField] public float walkSpeed;//이속
        [SerializeField] private float runMultiplier;//대쉬속도
        [SerializeField] private float jumpForce;//점프력
        [SerializeField] private Weapon currentWeapon;//장착하고있는 무기
        private int attackCount = 0;//연속공격횟수
        private float attackCoolDown = 0.4f;//공격간 쿨다운
        private bool isAttackInputDisabled = false;//공격가능여부

        [Header("Skill One")]
        private bool _skillOneIsClicked =true;
        [SerializeField] private float _skillOnePower = 6.0f;
        [SerializeField] private float _skillOneLeft = 0.0f;
        [SerializeField] private float _skillOneCool = 5.0f;
        [SerializeField] private float _skillOneMp = 15.0f;
        [SerializeField] private Button _skillOneButton;
        [SerializeField] private Image _skillOneImage;
        [SerializeField] private Text _skillOneText;
        private bool _isSkillOneCoolDown = false;

        [Header("Skill Two")]
        private bool _skillTwoIsClicked = true;
        [SerializeField] private float _skillTwoDefend = 100.0f;
        [SerializeField] private float _skillTwoLeft = 0.0f;
        [SerializeField] private float _skillTwoCool = 10.0f;
        [SerializeField] private float _skillTwoMp = 40.0f;
        [SerializeField] private Button _skillTwoButton;
        [SerializeField] private Image _skillTwoImage;
        [SerializeField] private Text _skillTwoText;
        public AudioClip _defend;
        [SerializeField] private AudioSource defendSound;
        public bool _isDefendSound = false;

        [Header("Skill Three")]
        private bool _skillThreeIsClicked = true;
        [SerializeField] private float _skillThreeHeal = 30.0f;
        [SerializeField] private float _skillThreeLeft = 0.0f;
        [SerializeField] private float _skillThreeCool = 10.0f;
        [SerializeField] private float _skillThreeMp = 40.0f;
        [SerializeField] private Button _skillThreeButton;
        [SerializeField] private Image _skillThreeImage;
        [SerializeField] private Text _skillThreeText;
        private bool _isSkillThreeCoolDown = false;

        [Header("Skill Four")]
        private bool _skillFourIsClicked = true;
        [SerializeField] private float _skillFourHp = 20.0f;
        [SerializeField] private float _skillFourMp = 30.0f;
        [SerializeField] private float _skillFourLeft = 0.0f;
        [SerializeField] private float _skillFourCool = 1.0f;
        [SerializeField] private Button _skillFourButton;
        [SerializeField] private Image _skillFourImage;
        private bool _isSkillFourCoolDown = false;

        [Header("DeBuff")]
        private bool _debuffOn = false;
        private float _debuffRemainingTime;
        [SerializeField] private float _debuff = 0.0f;
        [SerializeField] private float _debuffIncrease = 1.0f;
        [SerializeField] private float _debuffMax = 20.0f;
        private float _debuffResetTime = 5.0f;
        private float _lastDamageTime = 0.0f;
        [SerializeField] private Button _debuffButton;
        [SerializeField] private Image _debuffImage;
        [SerializeField] private Text _debuffText;
        [SerializeField] private Text _debuffStack;
        private float _debuffCount = 0.0f;

        [Header("CollSub")]
        private bool _isAnimatingSkill = false;
        private float _one = 1.0f;//쿨타임 도와주는 함수
        private float _countHP = 0.0f;
        private float _countMP = 0.0f;
        
        [Header("Sound")]        
        


        [Header("GameOver")]
        [SerializeField] private Image blackScreenImage;
        [SerializeField] private Text gameOverText;

        
        #endregion
        #region move
        [Header("Movement")]
        [SerializeField] public GameObject direction;//바라보는 방향
        [SerializeField] private float gravity = -9.81f;
        [Header("Keybinds")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

        [Header("Footsteps")]
        [Tooltip("Footstep source")]
        [SerializeField] private AudioSource footstepSource;

        [Tooltip("Distance for ground texture checker")]
        [SerializeField] private float groundCheckDistance = 1.0f;

        [Tooltip("Footsteps playing rate")]
        [SerializeField][Range(1f, 2f)] private float footstepRate = 1f;

        [Tooltip("Footstep rate when player running")]
        [SerializeField][Range(1f, 2f)] private float runningFootstepRate = 1.5f;

        [Tooltip("Add textures for this layer and add sounds to be played for this texture")]
        public List<GroundLayer> groundLayers = new List<GroundLayer>();

        //idle update
        public float animationSwitchTime = 15.0f;
        private float lastInputTime;
        private float lastAnimationSwitchTime;
        private bool isPlayingAnimation = true;
        public GameObject idleSword;
        public GameObject idleshield;
        public bool isMoving = false;
        public bool isAnimation = false;

        //GameOver
        private bool isGameOver = false;


        #endregion
        #region camera
        [Header("Mouse Look")] 
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float mouseSensivity;
        [SerializeField] private float mouseVerticalClamp;
        [SerializeField] private float scrollSpeed = 2000.0f;
        private int escKeyPressCount = 0;
        private bool isLeftControlPressed = false;
        #endregion
        #region variables
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

        private GameObject previousHitObject;
        private int previousLayer;
        #endregion
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
        private void Start()
        {
            lastInputTime = Time.time;
            lastAnimationSwitchTime = Time.time;
        }

        private void Update()
        {
            if (!isGameOver)
            {
                #region Attack
                Attack();
                Skill();
                AttackReset();
                DebuffTimeCheck();
                IncreaseHealth();
                IncreaseMp();
                Die();
                #endregion
                #region move
                Movement();
                Jump();
                GroundChecker();
                IdleUpdate();
                #endregion
                #region Camera
                MouseLook();
                CheckInput();
                Zoom();
                #endregion
            }
        }
        #region attack
        private void Attack()
        {
            if (Input.GetMouseButtonDown(0) && !isAttackInputDisabled && _characterController.isGrounded && !_isAnimatingSkill && !isAnimation)
            {
                if (attackCount == 0)
                {
                    if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3") &&
                        _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f &&
                         _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f && !isAttackInputDisabled)
                    {
                        _animator.Play("Attack1");
                        CheckMonsterCollision();
                        attackCount++;
                        StartCoroutine(DisableInputForDuration(attackCoolDown));
                    }
                    else
                    {
                        _animator.Play("Attack1");
                        CheckMonsterCollision();
                        attackCount++;
                        StartCoroutine(DisableInputForDuration(attackCoolDown));
                    }

                }

                else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") &&
                         _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f &&
                         _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f && !isAttackInputDisabled)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        _animator.Play("Attack2");
                        CheckMonsterCollision();
                        attackCount++;
                        StartCoroutine(DisableInputForDuration(attackCoolDown));
                    }
                }

                else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") &&
                         _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f &&
                         _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f && !isAttackInputDisabled)
                {
                    if (Input.GetMouseButton(0))
                    {
                        _animator.Play("Attack3");
                        CheckMonsterCollision();
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
        private void CheckMonsterCollision()
        {
            Collider weaponCollider = currentWeapon.GetComponent<Collider>();

            // currentWeapon의 콜라이더가 없을 경우 처리
            if (weaponCollider == null)
            {
                Debug.LogWarning("Weapon collider not found.");
                return;
            }

            // 캐릭터 전방으로 레이캐스트 발사
            Ray ray2 = new Ray(direction.transform.position + new Vector3(0.0f, 1.0f, 0.0f), direction.transform.forward);
            Debug.DrawRay(ray2.origin, ray2.direction * currentWeapon.attackRange, Color.red);

            RaycastHit[] hits;

            // 레이캐스트로 충돌 검사
            hits = Physics.RaycastAll(ray2, currentWeapon.attackRange);

            foreach (RaycastHit hit in hits)
            {
                // 무기의 콜라이더와 몬스터의 콜라이더가 닿았을 때
                if (hit.collider.CompareTag("Monster"))
                {
                    Monster monster = hit.collider.GetComponent<Monster>();
                    MonsterTwo monsterTwo = hit.collider.GetComponent<MonsterTwo>();
                    MonsterThree monsterThree = hit.collider.GetComponent<MonsterThree>();
                    if (monster != null)
                    {
                        // 무기와 몬스터 충돌 시 몬스터의 체력 감소
                        monster.TakeDamage(_atkPower); // 무기의 공격력만큼 체력 감소시키도록 수정
                    }
                    if (monsterTwo != null)
                    {
                        // 무기와 몬스터 충돌 시 몬스터의 체력 감소
                        monsterTwo.TakeDamage(_atkPower); // 무기의 공격력만큼 체력 감소시키도록 수정
                    }
                    if (monsterThree != null)
                    {
                        // 무기와 몬스터 충돌 시 몬스터의 체력 감소
                        monsterThree.TakeDamage(_atkPower); // 무기의 공격력만큼 체력 감소시키도록 수정
                    }
                }
            }
        }
        private void AttackReset()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("WAIT"))
            {
                attackCount = 0;
            }
        }
        private void Skill()
        {
            #region 스킬1
            if (Input.GetKeyDown(KeyCode.Alpha1) && _characterController.isGrounded && _mp >= _skillOneMp && !isAnimation)
            {
                if (_skillOneIsClicked && !_isSkillOneCoolDown && !_isAnimatingSkill)
                {
                    _mp -= _skillOneMp;
                    _isAnimatingSkill = true;
                    _animator.Play("PowerAttack");
                    CheckMonsterCollision(_skillOnePower);
                    walkSpeed = 0.0f;
                    _isSkillOneCoolDown = true;
                    _skillOneLeft = _skillOneCool;
                    if (_skillOneButton != null)
                        _skillOneButton.enabled = false;
                    _skillOneIsClicked = false;

                    StartCoroutine(DisableSkillMove(0.9f, () =>
                    {
                        walkSpeed = 4.0f;
                        _isAnimatingSkill = false;
                    }));

                }

            }

            if (!_skillOneIsClicked)
            {
                _skillOneLeft -= Time.deltaTime * _one;
                _skillOneText.gameObject.SetActive(true);
                _skillOneText.text = ((int)_skillOneLeft).ToString();
                if (_skillOneLeft <= 0.0f)
                {
                    _skillOneText.gameObject.SetActive(false);
                    _skillOneLeft = 0.0f;
                    if (_skillOneButton != null)
                        _skillOneButton.enabled = true;
                    _isSkillOneCoolDown = false;
                    _skillOneIsClicked = true;
                }
                else
                {
                    float ratio = 1.0f - (_skillOneLeft / _skillOneCool);
                    if (ratio > 0.99f) ratio = 1.0f;
                    if (_skillOneImage != null)
                        _skillOneImage.fillAmount = ratio;
                }
            }
            #endregion
            #region 스킬2
            if (Input.GetKey(KeyCode.Alpha2) && _characterController.isGrounded && _mp >= _skillTwoMp && !isAnimation)
            {
                if (!_isDefendSound)
                {
                    _isDefendSound = true;
                    defendSound.PlayOneShot(_defend);
                }
                if (_skillTwoIsClicked && !_isAnimatingSkill)
                {
                    _mp -= _skillTwoMp;
                    _isAnimatingSkill = true;
                    _isJumping = true;
                    walkSpeed = 0.0f;
                    _animator.Play("Defend");
                    _defense = _skillTwoDefend;
                }

            }
            if (Input.GetKeyUp(KeyCode.Alpha2) && _skillTwoLeft == 0 && _animator.GetCurrentAnimatorStateInfo(0).IsName("Defend"))
            {
                _isDefendSound = false;
                walkSpeed = 4.0f;
                _isAnimatingSkill = false;
                _isJumping = false;
                _animator.Play("WAIT");
                _skillTwoLeft = _skillTwoCool;
                _defense = _one;
                _skillTwoIsClicked = false;
                if (_skillTwoButton != null)
                    _skillTwoButton.enabled = false;
            }

            if (!_skillTwoIsClicked)
            {
                _skillTwoLeft -= Time.deltaTime * _one;
                _skillTwoText.gameObject.SetActive(true);
                _skillTwoText.text = ((int)_skillTwoLeft).ToString();
                if (_skillTwoLeft <= 0.0)
                {
                    _skillTwoText.gameObject.SetActive(false);
                    _skillTwoLeft = 0.0f;
                    if (_skillTwoButton != null)
                        _skillTwoButton.enabled = true;
                    _skillTwoIsClicked = true;
                }
                else
                {
                    float ratio = 1.0f - (_skillTwoLeft / _skillTwoCool);
                    if (ratio > 0.99f) ratio = 1.0f;
                    if (_skillTwoImage != null)
                        _skillTwoImage.fillAmount = ratio;
                }
            }
            #endregion
            #region 스킬3
            if (Input.GetKeyDown(KeyCode.Alpha3) && _characterController.isGrounded && _mp >= _skillThreeMp && !isAnimation)
            {
                if (_skillThreeIsClicked && !_isSkillThreeCoolDown && !_isAnimatingSkill)
                {
                    _mp -= _skillThreeMp;
                    _isAnimatingSkill = true;
                    _animator.Play("Heal");
                    _hp += _skillThreeHeal;
                    if (_hp >= _maxHP)
                    {
                        _hp = _maxHP;
                    }
                    walkSpeed = 0.0f;
                    _isSkillThreeCoolDown = true;
                    _skillThreeLeft = _skillThreeCool;
                    _skillThreeIsClicked = false;
                    if (_skillThreeButton != null)
                        _skillThreeButton.enabled = false;
                    StartCoroutine(DisableSkillMove(1.6f, () =>
                    {
                        walkSpeed = 4.0f;
                        _isAnimatingSkill = false;
                    }));


                }

            }

            if (!_skillThreeIsClicked)
            {
                _skillThreeLeft -= Time.deltaTime * _one;
                _skillThreeText.gameObject.SetActive(true);
                _skillThreeText.text = ((int)_skillThreeLeft).ToString();
                if (_skillThreeLeft <= 0.0f)
                {
                    _skillThreeText.gameObject.SetActive(false);
                    _skillThreeLeft = 0.0f;
                    if (_skillThreeButton != null)
                        _skillThreeButton.enabled = true;
                    _skillThreeIsClicked = true;
                    _isSkillThreeCoolDown = false;
                }
                else
                {
                    float ratio = 1.0f - (_skillThreeLeft / _skillThreeCool);
                    if (ratio > 0.99f) ratio = 1.0f;
                    if (_skillThreeImage != null)
                        _skillThreeImage.fillAmount = ratio;
                }
            }
            #endregion
            #region 스킬4
            if (Input.GetKeyDown(KeyCode.Alpha4) && _characterController.isGrounded && !isAnimation)
            {
                if (_skillFourIsClicked && !_isSkillFourCoolDown && !_isAnimatingSkill)
                {
                    _hp -= _skillFourHp;
                    if (_hp < 0.0f)
                    {
                        _hp = 0.0f;
                    }
                    _mp += _skillFourMp;
                    if (_mp > _maxMP)
                    {
                        _mp = _maxMP;
                    }
                    _isAnimatingSkill = true;
                    _animator.Play("Sacrifice");
                    walkSpeed = 0.0f;
                    _isSkillFourCoolDown = true;
                    _skillFourLeft = _skillFourCool;
                    if (_skillFourButton != null)
                        _skillFourButton.enabled = false;
                    _skillFourIsClicked = false;

                    StartCoroutine(DisableSkillMove(0.46f, () =>
                    {
                        walkSpeed = 4.0f;
                        _isAnimatingSkill = false;
                    }));

                }

            }

            if (!_skillFourIsClicked)
            {
                _skillFourLeft -= Time.deltaTime * _one;
                if (_skillFourLeft <= 0.00000f)
                {
                    _skillFourLeft = 0.00000f;
                    if (_skillFourButton != null)
                        _skillFourButton.enabled = true;
                    _isSkillFourCoolDown = false;
                    _skillFourIsClicked = true;
                }
                else
                {
                    float ratio = 1.0f - (_skillFourLeft / _skillFourCool);
                    if (ratio > 0.97f) ratio = 1.0f;
                    if (_skillFourImage != null)
                        _skillFourImage.fillAmount = ratio;
                }
            }
            #endregion

        }
        private IEnumerator DisableSkillMove(float delay, Action onComplete)
        {
            yield return new WaitForSeconds(delay);
            onComplete?.Invoke();
        }
        private void CheckMonsterCollision(float _skillOnePower)
        {
            Collider weaponCollider = currentWeapon.GetComponent<Collider>();

            // currentWeapon의 콜라이더가 없을 경우 처리
            if (weaponCollider == null)
            {
                Debug.LogWarning("Weapon collider not found.");
                return;
            }

            // 캐릭터 전방으로 레이캐스트 발사
            Ray ray2 = new Ray(direction.transform.position + new Vector3(0.0f, 1.0f, 0.0f), direction.transform.forward);
            Debug.DrawRay(ray2.origin, ray2.direction * currentWeapon.attackRange, Color.red);

            RaycastHit[] hits;

            // 레이캐스트로 충돌 검사
            hits = Physics.RaycastAll(ray2, currentWeapon.attackRange);

            foreach (RaycastHit hit in hits)
            {
                // 무기의 콜라이더와 몬스터의 콜라이더가 닿았을 때
                if (hit.collider.CompareTag("Monster"))
                {
                    Monster monster = hit.collider.GetComponent<Monster>();
                    MonsterTwo monsterTwo = hit.collider.GetComponent<MonsterTwo>();
                    MonsterThree monsterThree = hit.collider.GetComponent<MonsterThree>();
                    if (monster != null)
                    {
                        // 무기와 몬스터 충돌 시 몬스터의 체력 감소
                        monster.TakeDamage(_skillOnePower); // 무기의 공격력만큼 체력 감소시키도록 수정
                    }
                    if (monsterTwo != null)
                    {
                        // 무기와 몬스터 충돌 시 몬스터의 체력 감소
                        monsterTwo.TakeDamage(_skillOnePower); // 무기의 공격력만큼 체력 감소시키도록 수정
                    }
                    if (monsterThree != null)
                    {
                        // 무기와 몬스터 충돌 시 몬스터의 체력 감소
                        monsterThree.TakeDamage(_skillOnePower); // 무기의 공격력만큼 체력 감소시키도록 수정
                    }
                }
            }
        }
        public void TakeDamage(float damageAmount) 
        {
            if (_hp > 0)
            {
                if (_debuffCount < 20)
                {
                    _debuffCount++;
                }
                _debuff += _debuffIncrease;
                _debuffOn = true;
                if (_debuff > _debuffMax) //디버프 최대치
                {
                    _debuff = _debuffMax;
                }
                _lastDamageTime = Time.time;//데미지 입은 시간

                float _totalDamage = damageAmount + _debuff;
                Debug.Log(_totalDamage);
                if (_defense >= _totalDamage)
                {
                    return;
                }
                else
                {
                    _hp -= _totalDamage - _defense;
                }
            }
        }
        private void DebuffTimeCheck()
        {
            if ((Time.time - _lastDamageTime >= _debuffResetTime) && _debuffOn)
            //현재시간-  데미지 입은시간차이가   5이상
            {
                _debuffText.gameObject.SetActive(false);
                _debuffOn = false;
                _debuff = 0.0f;
                _debuffCount = 0.0f;
                _debuffButton.gameObject.SetActive(false);
                _debuffImage.gameObject.SetActive(false);
                _debuffStack.gameObject.SetActive(false);
            }
            else if (Time.time - _lastDamageTime < _debuffResetTime && _debuffOn)
            {
                _debuffRemainingTime = _debuffResetTime - (Time.time - _lastDamageTime);
                _debuffStack.gameObject.SetActive(true);
                _debuffButton.gameObject.SetActive(true);
                _debuffImage.gameObject.SetActive(true);
                _debuffText.gameObject.SetActive(true);
                _debuffText.text = ((int)_debuffRemainingTime).ToString();
                _debuffStack.text = ("x" + (int)_debuffCount).ToString();

                float ratio = 1.0f - (_debuffRemainingTime / _debuffResetTime);
                if (_debuffImage != null)
                    _debuffImage.fillAmount = ratio;
            }
        }
        public void IncreaseHealth()
        {
            if( _hp > 0 && _hp<_maxHP)
            {
                _countHP += _one * Time.deltaTime;
                if(_countHP > 3.0f) 
                {
                    _countHP = 0;
                    _hp += _one;
                    _hp = Mathf.Min(_hp, _maxHP);
                }
            }
        }
        public void IncreaseMp()
        {
            if (_mp < _maxMP)
            {
                _countMP += _one * Time.deltaTime;
                if (_countMP > 0.7f)
                {
                    _countMP = 0;
                    _mp += _one;
                    _mp = Mathf.Min(_mp, _maxMP);
                }
            }
        }
        public void Die()
        {
            if(_hp<=0 && !isGameOver)
            {
                isGameOver = true;
                _animator.Play("Die");
                StartCoroutine(gameOver());
            }
        }
        private IEnumerator gameOver()
        {
            gameOverText.gameObject.SetActive(true);
            yield return null;
            gameOverText.CrossFadeAlpha(0,0, false);
            yield return null;
            blackScreenImage.CrossFadeAlpha(1,4.0f,false);
            gameOverText.CrossFadeAlpha(1,4.0f, false);
            yield return new WaitForSeconds(3.0f);
            gameOverText.CrossFadeAlpha(0,1.0f,false);
            yield return new WaitForSeconds(2.0f);
            isGameOver = false;
            SceneManager.LoadScene("GameStart");
        }
        #endregion
        #region move
        private void Movement()
        {
            if (!(((_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1")) || (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2")) || (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))) &&
                _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.0f) && !isAnimation)
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
                }
                else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
                {
                    direction.gameObject.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, 135, 0));
                }
                else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
                {
                    direction.gameObject.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, 225, 0));
                }
                else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
                {
                    direction.gameObject.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, 315, 0));
                }
                else if (Input.GetKey(KeyCode.W))
                {
                    direction.gameObject.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, 0, 0));
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    direction.gameObject.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, 90, 0));
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    direction.gameObject.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(0, 180, 0));
                }
                else if (Input.GetKey(KeyCode.A))
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
        }
        private void Jump()
        {
            if (!(((_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1")) ||
                        (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2")) ||
                            (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))) &&
                                _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f &&
                                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.0f) &&
                                        !_isJumping && Input.GetKeyDown(jumpKey) && _characterController.isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpForce * 2f * -gravity);
                _animator.SetBool("jump", true);
                
                StartCoroutine(JumpCoolTime(0.8f));
            }
        }
        private IEnumerator JumpCoolTime(float cool)
        {
            _isJumping = true; // 점프 중임을 설정
            yield return new WaitForSeconds(cool);

            
            _isJumping = false; // 점프 종료
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
        private void IdleUpdate()
        {
            if (Input.anyKey)
            {
                isMoving = true;
                lastAnimationSwitchTime = Time.time;
            }
            else
            {
                isMoving = false;
            }

            if (isMoving)
            {
                lastInputTime = Time.time;//키입력이 뭐라도 있으면 시간 갱신
            }

            if(Time.time - lastAnimationSwitchTime > animationSwitchTime)//마지막 입력시간과의 차이가 3초이상 나게되면
            {
                ToggleAnimation();
                PlayCurrentAnimation();
                lastAnimationSwitchTime = Time.time;
            }
        }

        private void ToggleAnimation()
        {
            isPlayingAnimation = !isPlayingAnimation;
        }
        private void PlayCurrentAnimation()
        {
            if (isPlayingAnimation)
            {
                isAnimation = true;
                _animator.Play("WAIT1");
                idleSword.SetActive(false);
                idleshield.SetActive(false);

                StartCoroutine(DisableSkillMove(5.5f, () =>
                {
                    isAnimation=false;
                    idleSword.SetActive(true);
                    idleshield.SetActive(true);
                }));
            }
            else
            {
                isAnimation = true;
                _animator.Play("WAIT2");
                idleSword.SetActive(false);
                idleshield.SetActive(false);
                StartCoroutine(DisableSkillMove(4.7f, () =>
                {
                    idleSword.SetActive(true);
                    idleshield.SetActive(true);
                    isAnimation = false;
                }));
            }
        }
        private void FixedUpdate()
        {
            if (!(_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Attack3")))
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
                        _velocity.y += (gravity + 3.0f) * Time.fixedDeltaTime;
                    }
                }
                _animator.SetFloat("speed", _currentSpeed / 4.0f);
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
        }
        #endregion
        #region camera
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
            if (!isLeftControlPressed)
            {
                if (playerCamera.transform.localPosition.y >= 0.6f && playerCamera.transform.localPosition.y <= 8.0f)
                {
                    float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
                    Vector3 cameraDirection = this.transform.localRotation * Vector3.forward + this.transform.localRotation * Vector3.down;
                    playerCamera.transform.position += cameraDirection * Time.deltaTime * scrollWheel * scrollSpeed;
                }
                else if (playerCamera.transform.localPosition.y < 0.6f)
                {
                    playerCamera.transform.localPosition = new Vector3(0.0f, 0.6f, -0.6f);
                }
                else
                {
                    playerCamera.transform.localPosition = new Vector3(0.0f, 8.0f, -8.0f);
                }
            }
        }
        #endregion
        #region other
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
            footstepSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);

            AudioClip selectedClip = clips[UnityEngine.Random.Range(0, clips.Length)];

            while (selectedClip == _previousClip && attempts > 0)
            {
                selectedClip = clips[UnityEngine.Random.Range(0, clips.Length)];

                attempts--;
            }
            _previousClip = selectedClip;
            return selectedClip;
        }
        #endregion
    }
}
