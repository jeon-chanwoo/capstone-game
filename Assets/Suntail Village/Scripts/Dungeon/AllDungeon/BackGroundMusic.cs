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

    private bool hasEntered = false;

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
        backGroundMusicSource.Play();
    }
}
