using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private float currentHealth;
    public float moveSpeed = 5.0f;
    public float transAttack = 3.0f;
    public Transform target;
    NavMeshAgent agent;
    public Animator anim;
 


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
        SetHealth(50.0f);
        state = State.Idle;
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if (((anim.GetCurrentAnimatorStateInfo(0).IsName("Run")&& 
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
        else
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
        else
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
            state = State.Attack;
            ChooseRandomAttack();
        }
        else if (distance > transAttack)
        {
           
            state = State.Tracking;
            anim.SetTrigger("run");
        }
    }

    private void UpdateDie()
    {
        StartCoroutine(UpdateDieWithDelay());
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
                break;
            case 2:
                anim.SetTrigger("attack2");
                break;
            default:
                break;
        }
    }
    // 초기화 메서드
    public void SetHealth(float health)
    {
        currentHealth = health;
    }
    //죽었을때 죽은모션 후 디스트로이
    private IEnumerator UpdateDieWithDelay()
    {
        anim.Play("Die");
        yield return new WaitForSeconds(5.0f);
        Destroy(agent);
    }
    // 데미지를 입었을 때 호출되는 메서드
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            state = State.Die;
            Debug.Log("주금");
        }
        else
        {
            state = State.GetHit;
        }
    }



}