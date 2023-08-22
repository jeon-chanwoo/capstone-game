using Suntail;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Monster : MonoBehaviour
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
        SetHealth(200.0f);
        state = State.Idle;
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if (((anim.GetCurrentAnimatorStateInfo(0).IsName("Run") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) ||
            (anim.GetCurrentAnimatorStateInfo(0).IsName("Get Hit") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) ||
            (anim.GetCurrentAnimatorStateInfo(0).IsName("Basic Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) ||
            (anim.GetCurrentAnimatorStateInfo(0).IsName("Claw Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)))
        {
            agent.isStopped = false;
            agent.ResetPath();
            agent.updatePosition = true;
            agent.updateRotation = true;
        }

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
        IncreaseHealth();
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
            ChooseRandomAttack();
        }
    }

    private void UpdateTracking()
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= transAttack)
        {
            state = State.Attack;
            ChooseRandomAttack();
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

            ChooseRandomAttack();
        }
    }
    private void UpdateGetHit()
    {

        if (currentHealth % 10 == 0)
            anim.Play("Get Hit");

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= transAttack)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
            {
                state = State.Attack;
                ChooseRandomAttack();
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
        _text.CrossFadeAlpha(0, 5f, false);
        StartCoroutine(DestroyAfterDelay(5.0f));
        OpenDoor();
    }

    //공격할때 2개의 공격중 1개 랜덤공격
    private void ChooseRandomAttack()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.updatePosition = false;
        agent.updateRotation = false;
        int randomAttack = Random.Range(1, 3); // 1, 2 중에 하나 랜덤 선택
        switch (randomAttack)
        {
            case 1:
                anim.SetTrigger("attack1");
                Attack1();
                break;
            case 2:
                anim.SetTrigger("attack2");
                Attack2();
                break;
            default:
                break;
        }
        anim.ResetTrigger("run");


    }
    public void Attack1()
    {
        if (state == State.Attack && !isAttacking && anim.GetCurrentAnimatorStateInfo(0).IsName("Claw Attack"))
        {
            if (target != null)
            {
                PlayerController player = target.GetComponent<PlayerController>();
                if ((player != null))
                    if (!isAttacking)
                    {
                        player.TakeDamage(6.0f);
                        isAttacking = true;
                        StartCoroutine(ResetAttack(1.43f));
                    }

            }
        }
    }
    public void Attack2()
    {
        if (state == State.Attack && !isAttacking && anim.GetCurrentAnimatorStateInfo(0).IsName("Basic Attack"))
        {
            if (target != null)
            {
                PlayerController player = target.GetComponent<PlayerController>();
                if (player != null)
                {
                    if (!isAttacking)
                    {
                        player.TakeDamage(4.0f);
                        isAttacking = true;
                        StartCoroutine(ResetAttack(1.1f));
                    }
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
            if (count > 0.8f)
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
        Destroy(agent.gameObject);
    }
    // 데미지를 입었을 때 호출되는 메서드
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log(currentHealth + "/" + maxHealth);
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

}