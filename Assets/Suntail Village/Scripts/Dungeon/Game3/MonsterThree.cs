using Suntail;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.VFX;

public class MonsterThree : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;
    public float healthIncreaseRate = 1.0f;
    public float count = 0.0f;
    public float healthCount = 0.0f;
    private GameObject player;
    NavMeshAgent agent;
    public BackGroundMusic backGroundMusicScript;// 백그라운드 뮤직 스크립트
    private AudioSource audioSource;
    [SerializeField ]private AudioClip _magma;
    private PlayerController playerController;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetHealth(300.0f);
        agent = GetComponent<NavMeshAgent>();
        backGroundMusicScript = GameObject.Find("Controller Camera").GetComponent<BackGroundMusic>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        audioSource.clip = _magma;
        audioSource.Play();
    }
    private void Update()
    {
        IncreaseHealth();
        DamageOverTime();
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
            count += Time.deltaTime;
            if (count > 1.0f)
            {
                count = 0;
                currentHealth += healthIncreaseRate;
                currentHealth = Mathf.Min(currentHealth, maxHealth); // 최대값 초과 방지
            }
        }
    }
    public void DamageOverTime()
    {
        if (currentHealth > 0)
        {
            healthCount += Time.deltaTime;
            if (playerController._hp >= 0.0f && healthCount > 1.0f)
            {
                healthCount = 0.0f;
                playerController._hp -= 1.0f;
            }
        }
        
    }
    private void Die()
    {
        Transform stageClearTextTransform = Camera.main.transform.Find("UI/StageClear");
        Text _text = stageClearTextTransform.GetComponent<Text>();
        _text.gameObject.SetActive(true);
        _text.CrossFadeAlpha(1, 0, false);
        _text.CrossFadeAlpha(0, 5f, false);
        StartCoroutine(DestroyAfterDelay(1.0f));
        backGroundMusicScript.StopBossMusic();
        audioSource.Stop();
        playerController._maxHP = 100.0f;
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OpenDoor();
        Destroy(agent.gameObject);
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

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }
}