using System.Collections;
using UnityEngine;


namespace Suntail
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class CastleDoor : MonoBehaviour
    {
        [Tooltip("Door opening sound")]
        [SerializeField] private AudioClip castleDooropenSound;

        [Tooltip("Additional delay in deactivating interaction, added to animation time")]
        [HideInInspector] public bool castleDoorOpen = false;

        
        private Animator _castleDoorAnimator;
        private AudioSource _castleDoorAudioSource;

        private void Awake()
        {
            _castleDoorAudioSource = gameObject.GetComponent<AudioSource>();
            _castleDoorAnimator = gameObject.GetComponent<Animator>();
        }

        public void PlayCastleDoorAnimation()
        {
            if (!castleDoorOpen)
            {
                _castleDoorAnimator.Play("OpenCastleDoor");
                _castleDoorAudioSource.volume = 0.5f;
                _castleDoorAudioSource.clip = castleDooropenSound;
                _castleDoorAudioSource.Play();
                
            }
        }
    }
}