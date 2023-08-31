using Suntail;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.VFX;

public class MonsterTwo : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;
    public float moveSpeed = 5.0f;
    public float transAttack = 5.0f;
    public float healthIncreaseRate = 4.0f;
    public Transform target;
    public float count = 0.0f;
    NavMeshAgent agent;
    public Animator anim;
    private bool isAttacking = false;
    public BackGroundMusic backGroundMusicScript;// 백그라운드 뮤직 스크립트
    private AudioSource audioSource;
    public AudioClip basicAttack;
    public AudioClip _die;
    public AudioClip _run;
    public AudioClip _scream;
    public bool isDeathAttack = false;
    public bool firstDeathAttack = false;
    public bool secondDeathAttack = false;
    public bool thirdDeathAttack = false;
    public bool fourthDeathAttack = false;
    public ParticleSystem _meteor;
    float desireVolum = 0.5f;
    enum State
    {
        Idle,
        Tracking,
        Attack,
        GetHit,
        Die,
    }

    State state;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetHealth(300.0f);
        state = State.Idle;
        agent = GetComponent<NavMeshAgent>();
        backGroundMusicScript = GameObject.Find("Controller Camera").GetComponent<BackGroundMusic>();
    }
    private void Update()
    {
        if (((anim.GetCurrentAnimatorStateInfo(0).IsName("Run") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) ||
            (anim.GetCurrentAnimatorStateInfo(0).IsName("Get Hit") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) ||
            (anim.GetCurrentAnimatorStateInfo(0).IsName("Basic Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) ||
            (anim.GetCurrentAnimatorStateInfo(0).IsName("Scream") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)))
        {
            agent.isStopped = false;
            agent.ResetPath();
            agent.updatePosition = true;
            agent.updateRotation = true;
        }


        if (!isDeathAttack)
        {
            if (state == State.Idle)
            {
                UpdateIdle();
            }
            else if (state == State.Tracking)
            {
                UpdateTracking();
            }
            else if (state == State.Attack)
            {
                UpdateAttack();
            }
            else if (state == State.GetHit)
            {
                UpdateGetHit();
            }
            else if (state == State.Die)
            {
                UpdateDie();
            }
        }
        IncreaseHealth();
        InstantDeathAttack();
    }

    private void UpdateIdle()
    {
        agent.speed = moveSpeed;
        target = GameObject.Find("Controller").transform;
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > transAttack)
        {

            if (target != null)
            {
                state = State.Tracking;
                anim.SetTrigger("run");
            }
        }
        else if (distance <= transAttack)
        {
            state = State.Attack;
            anim.SetTrigger("attack1");
            Attack1();
        }
    }
    private void UpdateTracking()
    {
        
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= transAttack)
            {
                state = State.Attack;
                anim.SetTrigger("attack1");
                Attack1();
            }
            else if (distance > transAttack)
            {
                anim.SetTrigger("run");
                agent.speed = moveSpeed;
                agent.destination = target.transform.position;
            }
        
    }
    private void UpdateAttack()
    {
        agent.speed = 0;
        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance > transAttack)
        {
            state = State.Tracking;
            anim.SetTrigger("run");
        }
        else if (distance <= transAttack)
        {

            anim.SetTrigger("attack1");
            Attack1();
        }
    }
    private void UpdateGetHit()
    {

        if (currentHealth % 11 == 0)
            anim.Play("Get Hit");
            

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= transAttack)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
            {
                state = State.Attack;
                anim.SetTrigger("attack1");
                Attack1();
            }
        }
        else if (distance > transAttack)
        {

            state = State.Tracking;
            anim.SetTrigger("run");
        }
    }
    private void UpdateDie()
    {
        anim.Play("Die");
        Transform stageClearTextTransform = Camera.main.transform.Find("UI/StageClear");
        Text _text = stageClearTextTransform.GetComponent<Text>();
        _text.gameObject.SetActive(true);
        _text.CrossFadeAlpha(1, 0, false);
        _text.CrossFadeAlpha(0, 5f, false);
        StartCoroutine(DestroyAfterDelay(3.0f));
        
        backGroundMusicScript.StopBossMusic();
    }
    public void Attack1()
    {
        if (state == State.Attack && !isAttacking && anim.GetCurrentAnimatorStateInfo(0).IsName("Basic Attack"))
        {
            if (target != null)
            {
                PlayerController player = target.GetComponent<PlayerController>();
                if (player != null && !isAttacking)
                {
                     player.TakeDamage(1.0f);
                     isAttacking = true;
                     StartCoroutine(ResetAttack(1.3f));
                }
            }
        }
    }
    private IEnumerator ResetAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isAttacking)
        {
            isAttacking = false;
        }
    }
    private void InstantDeathAttack()
    {
        Transform DeathAttackText = Camera.main.transform.Find("UI/DeathAttack");
        Text _text = DeathAttackText.GetComponent<Text>();
        //ParticleSystem.MainModule mainModule = _meteor.main;
        if (currentHealth<=260.0f && !firstDeathAttack)
        {
            isDeathAttack = true;
            firstDeathAttack = true;
            if(target != null)
            {
                _text.gameObject.SetActive(true);
                _text.CrossFadeAlpha(1, 0, false);
                _text.CrossFadeAlpha(0, 2f, false);
                _meteor.gameObject.SetActive(true);
                anim.Play("Scream");
                StartCoroutine(DeathAttack(4.0f));
            }
        }
        if (currentHealth <= 200.0f && !secondDeathAttack)
        {
            isDeathAttack = true;
            secondDeathAttack = true;
            if (target != null)
            {
                _text.gameObject.SetActive(true);
                _text.CrossFadeAlpha(1, 0, false);
                _text.CrossFadeAlpha(0, 2f, false);
                _meteor.gameObject.SetActive(true);
                anim.Play("Scream");
                StartCoroutine(DeathAttack(4.0f));
            }
        }
        if (currentHealth <= 140.0f && !thirdDeathAttack)
        {
            isDeathAttack = true;
            thirdDeathAttack = true;
            if (target != null)
            {
                _text.gameObject.SetActive(true);
                _text.CrossFadeAlpha(1, 0, false);
                _text.CrossFadeAlpha(0, 2f, false);
                _meteor.gameObject.SetActive(true);
                anim.Play("Scream");
                StartCoroutine(DeathAttack(4.0f));
            }
        }
        if (currentHealth <= 80.0f && !fourthDeathAttack)
        {
            isDeathAttack = true;
            fourthDeathAttack = true;
            if (target != null)
            {
                _text.gameObject.SetActive(true);
                _text.CrossFadeAlpha(1, 0, false);
                _text.CrossFadeAlpha(0, 2f, false);
                _meteor.gameObject.SetActive(true);
                anim.Play("Scream");
                StartCoroutine(DeathAttack(3.0f));
            }
        }
    }
    private IEnumerator DeathAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayerController player = target.GetComponent<PlayerController>();

        _meteor.Play();
        yield return new WaitForSeconds(1.2f);
        player.TakeDamage(60.0f);
        _meteor.Stop();

        isDeathAttack = false;
        state = State.Idle;
        anim.Play("Idle");
        _meteor.gameObject.SetActive(false);
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        maxHealth = health;
    }
    public void IncreaseHealth()
    {
        if (currentHealth > 0 && currentHealth < maxHealth)
        {
            count += healthIncreaseRate * Time.deltaTime;
            if (count > 2.0f)
            {
                count = 0;
                currentHealth += healthIncreaseRate;
                currentHealth = Mathf.Min(currentHealth, maxHealth); // 최대값 초과 방지
            }
        }
    }

    public void OpenDoor()
    {
        GameObject door = GameObject.Find("SM_Env_Wall_233 (6)");
        Animator animator = door.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("open");
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OpenDoor();
        Destroy(agent.gameObject);
    }
    // 데미지를 입었을 때 호출되는 메서드
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            state = State.Die;
        }
        else
        {
            state = State.GetHit;
        }
    }

    public void Sound(string animationName)
    {
        if (animationName == "Basic Attack")
        {
            audioSource.PlayOneShot(basicAttack);
        }
        else if (animationName == "Die")
        {
            audioSource.PlayOneShot(_die);
        }
        else if (animationName == "Run")
        {
            audioSource.PlayOneShot(_run);
        }
        else if (animationName == "Scream")
        {
            audioSource.volume = desireVolum;
            audioSource.PlayOneShot(_scream);
        }
    }
}