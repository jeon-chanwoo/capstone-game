using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioClip waitSound1;
    public AudioClip waitSound2;
    public AudioClip powerAttack;
    public AudioClip _heal;
    public AudioClip _sacrifice;
    public AudioClip _jump;
    public AudioClip _attack1;
    public AudioClip _attack2;
    public AudioClip _attack3;
    public AudioClip _die;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Sound(string animationName)
    {
        if (animationName == "WAIT1")
        {
            audioSource.PlayOneShot(waitSound1);
        }
        else if (animationName == "WAIT2")
        {
            audioSource.PlayOneShot(waitSound2);
        }
        else if (animationName == "PowerAttack")
        {
            audioSource.PlayOneShot(powerAttack);
        }
        else if (animationName == "Heal")
        {
            audioSource.PlayOneShot(_heal);
        }
        else if (animationName == "Sacrifice")
        {
            audioSource.PlayOneShot(_sacrifice);
        }
        else if (animationName == "JUMP")
        {
            audioSource.PlayOneShot(_jump);
        }
        else if (animationName == "Attack1")
        {
            audioSource.PlayOneShot(_attack1);
        }
        else if (animationName == "Attack2")
        {
            audioSource.PlayOneShot(_attack2);
        }
        else if (animationName == "Attack3")
        {
            audioSource.PlayOneShot(_attack3);
        }
        else if (animationName == "Die")
            audioSource.PlayOneShot(_die);

    }
}
