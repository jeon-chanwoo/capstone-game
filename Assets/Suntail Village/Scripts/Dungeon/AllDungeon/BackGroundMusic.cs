using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackGroundMusic : MonoBehaviour
{
    public AudioClip enterSound;//입장시 효과음
    public AudioSource enterSoundSource;//효과음의 사운드 소스
    public AudioClip backGroundMusic;//기본 배경음악
    public AudioSource backGroundMusicSource;//기본 배경음악 소스
    public AudioClip bossMusic;//보스와의 전투 개시시 재생될 음악

    private bool hasEntered = false; //보스 문에 입장했는지 확인
    private bool isBossMusicPlaying = false;//보스 음악이 재생중인지 확인

    void Start()
    {
        //기본설정
        enterSoundSource = gameObject.AddComponent<AudioSource>();
        enterSoundSource.clip = enterSound;
        enterSoundSource.loop = false;
        enterSoundSource.playOnAwake = false;//씬이 실행 되자마자 실행할 건지 유무

        backGroundMusicSource = gameObject.AddComponent<AudioSource>();
        backGroundMusicSource.clip =  backGroundMusic;
        backGroundMusicSource.loop = true;
        backGroundMusicSource.playOnAwake = false;

        PlayEnterSound();
        Invoke("PlayBackGroundMusic", 3.0f);//3초의 대기시간후 실행
    }

    void PlayEnterSound()//최초 입장사운드
    {
        if (!hasEntered)
        {
            enterSoundSource.Play();
            hasEntered = true;
        }
    }

    void PlayBackGroundMusic()//던전 기본 배경음악
    {
        if(!isBossMusicPlaying)
        {
            backGroundMusicSource.Play();
        }
    }

    public void PlayBossMusic()//보스와의 전투시 배경음악
    {
        isBossMusicPlaying = true;
        backGroundMusicSource.Stop(); // 기존 백그라운드 뮤직 정지
        backGroundMusicSource.clip = bossMusic; // 보스 음악으로 클립 변경
        backGroundMusicSource.Play();
    }
    public void StopBossMusic()//보스처치 후 원래의 배경음악
    {
        isBossMusicPlaying = false;
        backGroundMusicSource.Stop(); // 보스 음악 정지
        backGroundMusicSource.clip = backGroundMusic; // 원래 백그라운드 뮤직으로 클립 변경
        backGroundMusicSource.Play();
    }
    public void StartEnterMusic()//다음스테이지 입장시 나오는 사운드
    {
        backGroundMusicSource.Stop();
        backGroundMusicSource.clip = enterSound;
        backGroundMusicSource.Play();
        StartCoroutine(EnterSoundTime(3.2f));
    }

    private IEnumerator EnterSoundTime(float time)//딜레이 주는 함수
    {
        yield return new WaitForSeconds(time);
        backGroundMusicSource.clip = backGroundMusic;
        backGroundMusicSource.Play();
    }
}
