using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackGroundMusic : MonoBehaviour
{
    public AudioClip enterSound;
    public AudioSource enterSoundSource;
    public AudioClip backGroundMusic;
    public AudioSource backGroundMusicSource;
    public AudioClip bossMusic;

    private bool hasEntered = false;
    private bool isBossMusicPlaying = false;//���� ������ ��������� Ȯ��

    void Start()
    {
        enterSoundSource = gameObject.AddComponent<AudioSource>();
        enterSoundSource.clip = enterSound;
        enterSoundSource.loop = false;
        enterSoundSource.playOnAwake = false;

        backGroundMusicSource = gameObject.AddComponent<AudioSource>();
        backGroundMusicSource.clip =  backGroundMusic;
        backGroundMusicSource.loop = true;
        backGroundMusicSource.playOnAwake = false;

        PlayEnterSound();
        Invoke("PlayBackGroundMusic", 3.0f);
    }

    void PlayEnterSound()
    {
        if (!hasEntered)
        {
            enterSoundSource.Play();
            hasEntered = true;
        }
    }

    void PlayBackGroundMusic()
    {
        if(!isBossMusicPlaying)
        {
            backGroundMusicSource.Play();
        }
    }

    public void PlayBossMusic()
    {
        isBossMusicPlaying = true;
        backGroundMusicSource.Stop(); // ���� ��׶��� ���� ����
        backGroundMusicSource.clip = bossMusic; // ���� �������� Ŭ�� ����
        backGroundMusicSource.Play();
    }
    public void StopBossMusic()
    {
        isBossMusicPlaying = false;
        backGroundMusicSource.Stop(); // ���� ���� ����
        backGroundMusicSource.clip = backGroundMusic; // ���� ��׶��� �������� Ŭ�� ����
        backGroundMusicSource.Play();
    }
    public void StartEnterMusic()
    {
        backGroundMusicSource.Stop();
        backGroundMusicSource.clip = enterSound;
        backGroundMusicSource.Play();
        StartCoroutine(EnterSoundTime(3.2f));
    }

    private IEnumerator EnterSoundTime(float time)
    {
        yield return new WaitForSeconds(time);
        backGroundMusicSource.clip = backGroundMusic;
        backGroundMusicSource.Play();
    }
}
