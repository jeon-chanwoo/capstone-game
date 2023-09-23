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

    public Transform target;//플레이어 캐릭터
    
    public float count = 0.0f;// 체력재생 트리거로 사용할 변수

    NavMeshAgent agent;//몬스터 자신

    public Animator anim;
    private bool isAttacking = false;
    public BackGroundMusic backGroundMusicScript;// 백그라운드 뮤직 스크립트

    private AudioSource audioSource; //오디오 클립을 담을 소스
    public AudioClip basicAttack; //배경 오디오 사운드 
    public AudioClip _die; //죽었을때 오디오 사운드
    public AudioClip _run; //달릴때 오디오 사운드


    enum State // 몬스터AI
    {
        Idle, //기본상태
        Tracking, //추적상태
        Attack, //공격상태
        GetHit, //피격상태
        Die, //죽음
    }

    State state; // 상태 생성

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetHealth(200.0f);//현재체력과 최대체력 설정
        state = State.Idle; // 몬스터 생성되면 초기에는 기본상태로 생성
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent의 컴포넌트를 가지고 온다.
        backGroundMusicScript = GameObject.Find("Controller Camera").GetComponent<BackGroundMusic>();// backGroundMusic을 재생하는 카메라의 컴포넌트를 가지고 온다.
    }
    private void Update()
    {
        //이전 상태가 끝나지 않았는데 다음 상태가 재생되는걸 방지하는 함수
        if (((anim.GetCurrentAnimatorStateInfo(0).IsName("Run") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) ||
            (anim.GetCurrentAnimatorStateInfo(0).IsName("Get Hit") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) ||
            (anim.GetCurrentAnimatorStateInfo(0).IsName("Basic Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) ||
            (anim.GetCurrentAnimatorStateInfo(0).IsName("Claw Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)))
        {
            agent.isStopped = false; // 에이전트가 멈출것인가?->아니요
            agent.ResetPath();//경로를 초기화를 시켜줌으로써 이전에 설정된 경로때문에 캐릭터를 밀고가는 현상 방지
            agent.updatePosition = true; //몬스터가 이동하면서 공격하는것 방지
            agent.updateRotation = true; //몬스터가 회전하면서 공격하는것 방지
        }

        //몬스터의 상태를 감지하여 변화시켜주는 함수
        if (state == State.Idle)
        {
            UpdateIdle();//현재 상태 기본
        }
        else if (state == State.Tracking)
        {
            UpdateTracking();//현재 상태 추격
        }
        else if (state == State.Attack)
        {
            UpdateAttack();//현재 상태 공격
        }
        else if (state == State.GetHit)
        {
            UpdateGetHit();//현재 상태 피격
        }
        else if (state == State.Die)
        {
            UpdateDie();//현재 상태 죽음
        }
        IncreaseHealth();//자동 체력 회복
    }

    private void UpdateIdle()
    {
        agent.speed = moveSpeed;
        target = GameObject.Find("Controller").transform;
        float distance = Vector3.Distance(transform.position, target.transform.position);//플레이어와의 거리계산

        if (distance > transAttack)
        {

            if (target != null)
            {
                state = State.Tracking;//추격상태
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
            anim.ResetTrigger("run");//달리는 애니메이션 즉시 중단 (안할 시 공격애니메이션이 늦게 재생 됨)
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
            ChooseRandomAttack();//공격을 랜덤으로 하는 함수
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
        StartCoroutine(DestroyAfterDelay(3.0f));//보스 시체 제거
        backGroundMusicScript.StopBossMusic();//보스 음악 중지
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
        anim.ResetTrigger("run");//공격후 달리기 애니메이션이 재생되는 버그 제거위해 작성


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
                        player.TakeDamage(2.0f);//플레이어 컴포넌트에 직접 데미지 계산
                        isAttacking = true;
                        StartCoroutine(ResetAttack(1.43f));//애니메이션 공격시간만큼 시간을 주어 공격이 여러번 들어가는 버그 제거
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
                        player.TakeDamage(1.0f);//플레이어 컴포넌트에 직접 데미지 계산
                        isAttacking = true;
                        StartCoroutine(ResetAttack(1.1f));//애니메이션 공격시간만큼 시간을 주어 공격이 여러번 들어가는 버그 제거
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
            isAttacking = false; //공격이 들어가게 해주는 불 함수
        }
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        maxHealth = health;
    }
    public void IncreaseHealth() // 체력이 소수점으로 증가하는 현상 제거하기 위해 제작한 함수
    {
        if (currentHealth > 0 && currentHealth < maxHealth)
        {
            count += healthIncreaseRate * Time.deltaTime; //카운트를 세는 함수

            //카운트 달성시 체력 회복 및 카운트 초기화
            if (count > 1.5f)
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

    public void Sound(string animationName)// 사운드 설정
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