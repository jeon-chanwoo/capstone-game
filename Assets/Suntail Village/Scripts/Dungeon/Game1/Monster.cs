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

    public Transform target;//�÷��̾� ĳ����
    
    public float count = 0.0f;// ü����� Ʈ���ŷ� ����� ����

    NavMeshAgent agent;//���� �ڽ�

    public Animator anim;
    private bool isAttacking = false;
    public BackGroundMusic backGroundMusicScript;// ��׶��� ���� ��ũ��Ʈ

    private AudioSource audioSource; //����� Ŭ���� ���� �ҽ�
    public AudioClip basicAttack; //��� ����� ���� 
    public AudioClip _die; //�׾����� ����� ����
    public AudioClip _run; //�޸��� ����� ����


    enum State // ����AI
    {
        Idle, //�⺻����
        Tracking, //��������
        Attack, //���ݻ���
        GetHit, //�ǰݻ���
        Die, //����
    }

    State state; // ���� ����

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetHealth(200.0f);//����ü�°� �ִ�ü�� ����
        state = State.Idle; // ���� �����Ǹ� �ʱ⿡�� �⺻���·� ����
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent�� ������Ʈ�� ������ �´�.
        backGroundMusicScript = GameObject.Find("Controller Camera").GetComponent<BackGroundMusic>();// backGroundMusic�� ����ϴ� ī�޶��� ������Ʈ�� ������ �´�.
    }
    private void Update()
    {
        //���� ���°� ������ �ʾҴµ� ���� ���°� ����Ǵ°� �����ϴ� �Լ�
        if (((anim.GetCurrentAnimatorStateInfo(0).IsName("Run") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) ||
            (anim.GetCurrentAnimatorStateInfo(0).IsName("Get Hit") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) ||
            (anim.GetCurrentAnimatorStateInfo(0).IsName("Basic Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) ||
            (anim.GetCurrentAnimatorStateInfo(0).IsName("Claw Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)))
        {
            agent.isStopped = false; // ������Ʈ�� ������ΰ�?->�ƴϿ�
            agent.ResetPath();//��θ� �ʱ�ȭ�� ���������ν� ������ ������ ��ζ����� ĳ���͸� �а��� ���� ����
            agent.updatePosition = true; //���Ͱ� �̵��ϸ鼭 �����ϴ°� ����
            agent.updateRotation = true; //���Ͱ� ȸ���ϸ鼭 �����ϴ°� ����
        }

        //������ ���¸� �����Ͽ� ��ȭ�����ִ� �Լ�
        if (state == State.Idle)
        {
            UpdateIdle();//���� ���� �⺻
        }
        else if (state == State.Tracking)
        {
            UpdateTracking();//���� ���� �߰�
        }
        else if (state == State.Attack)
        {
            UpdateAttack();//���� ���� ����
        }
        else if (state == State.GetHit)
        {
            UpdateGetHit();//���� ���� �ǰ�
        }
        else if (state == State.Die)
        {
            UpdateDie();//���� ���� ����
        }
        IncreaseHealth();//�ڵ� ü�� ȸ��
    }

    private void UpdateIdle()
    {
        agent.speed = moveSpeed;
        target = GameObject.Find("Controller").transform;
        float distance = Vector3.Distance(transform.position, target.transform.position);//�÷��̾���� �Ÿ����

        if (distance > transAttack)
        {

            if (target != null)
            {
                state = State.Tracking;//�߰ݻ���
                anim.SetTrigger("run");
            }
        }
        else if (distance <= transAttack)
        {
            anim.ResetTrigger("run");
            state = State.Attack;
            ChooseRandomAttack();
        }
    }

    private void UpdateTracking()
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= transAttack)
        {
            anim.ResetTrigger("run");//�޸��� �ִϸ��̼� ��� �ߴ� (���� �� ���ݾִϸ��̼��� �ʰ� ��� ��)
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
            anim.ResetTrigger("run");
            ChooseRandomAttack();//������ �������� �ϴ� �Լ�
        }
    }
    private void UpdateGetHit()
    {
        anim.ResetTrigger("run");

        if (currentHealth % 11 == 0)
        {
            anim.Play("Get Hit");
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= transAttack)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
            {
                anim.ResetTrigger("run");
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
        anim.ResetTrigger("run");
        anim.Play("Die");
        Transform stageClearTextTransform = Camera.main.transform.Find("UI/StageClear");
        Text _text = stageClearTextTransform.GetComponent<Text>();
        _text.gameObject.SetActive(true);
        _text.CrossFadeAlpha(0, 5f, false);
        StartCoroutine(DestroyAfterDelay(3.0f));//���� ��ü ����
        backGroundMusicScript.StopBossMusic();//���� ���� ����
    }
    //�����Ҷ� 2���� ������ 1�� ��������
    private void ChooseRandomAttack()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.updatePosition = false;
        agent.updateRotation = false;
        int randomAttack = Random.Range(1, 3); // 1, 2 �߿� �ϳ� ���� ����
        switch (randomAttack)
        {
            case 1:
                anim.ResetTrigger("run");
                anim.SetTrigger("attack1");
                Attack1();
                break;
            case 2:
                anim.ResetTrigger("run");
                anim.SetTrigger("attack2");
                Attack2();
                break;
            default:
                break;
        }
        anim.ResetTrigger("run");//������ �޸��� �ִϸ��̼��� ����Ǵ� ���� �������� �ۼ�


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
                        player.TakeDamage(2.0f);//�÷��̾� ������Ʈ�� ���� ������ ���
                        isAttacking = true;
                        StartCoroutine(ResetAttack(1.43f));//�ִϸ��̼� ���ݽð���ŭ �ð��� �־� ������ ������ ���� ���� ����
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
                        player.TakeDamage(1.0f);//�÷��̾� ������Ʈ�� ���� ������ ���
                        isAttacking = true;
                        StartCoroutine(ResetAttack(1.1f));//�ִϸ��̼� ���ݽð���ŭ �ð��� �־� ������ ������ ���� ���� ����
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
            isAttacking = false; //������ ���� ���ִ� �� �Լ�
        }
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        maxHealth = health;
    }
    public void IncreaseHealth() // ü���� �Ҽ������� �����ϴ� ���� �����ϱ� ���� ������ �Լ�
    {
        if (currentHealth > 0 && currentHealth < maxHealth)
        {
            count += healthIncreaseRate * Time.deltaTime; //ī��Ʈ�� ���� �Լ�

            //ī��Ʈ �޼��� ü�� ȸ�� �� ī��Ʈ �ʱ�ȭ
            if (count > 1.5f)
            {
                count = 0;
                currentHealth += healthIncreaseRate;
                currentHealth = Mathf.Min(currentHealth, maxHealth); // �ִ밪 �ʰ� ����
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
    // �������� �Ծ��� �� ȣ��Ǵ� �޼���
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

    public void Sound(string animationName)// ���� ����
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
    }
}