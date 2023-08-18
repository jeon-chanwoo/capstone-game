using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;
    public float moveSpeed = 5.0f;
    public float transAttack = 3.0f;
    public float healthIncreaseRate = 1.0f;
    public Transform target;
    public float count = 0.0f;
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

        if (currentHealth % 5 == 0)
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
        anim.Play("Die");
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
        maxHealth = health;
    }
    public void IncreaseHealth()
    {
        if (currentHealth < maxHealth)
        {
            count += healthIncreaseRate * Time.deltaTime;
            if(count>0.8f)
            {
                count= 0;
                currentHealth += healthIncreaseRate;
                currentHealth = Mathf.Min(currentHealth, maxHealth); // 최대값 초과 방지
            }
                
                
        }
    }

    public void OpenDoor()
    {
        GameObject door = GameObject.Find("SM_Env_Wall_233 (6)");
        Animator animator = door.GetComponent<Animator>();
        if(animator != null)
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
        Debug.Log(currentHealth+"/"+maxHealth);
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