using System.Collections;
using UnityEngine;

namespace Suntail
{
    //현재는 쓰지않는 코드 마을안에서 물체를 들고 내릴수 있는 코드인데
    //사용하지 않음
    //나중에 물체를 이용한 기믹을 사용할때 사용할 예정
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    public class PhysicsObject : MonoBehaviour
    {
        [Tooltip("Waiting time for an item to be picked up")]
        [SerializeField] private float waitOnPickup = 0.2f;

        [Tooltip("The force by which an object is pulled away from the parent")]
        [SerializeField] private float breakForce = 25f;

        [Tooltip("Array drop sounds")]
        [SerializeField] private AudioClip[] dropClips;
        [HideInInspector] public bool pickedUp = false;
        [HideInInspector] public bool wasPickedUp = false;
        [HideInInspector] public PlayerInteractions playerInteraction;
        private AudioSource _objectAudioSource;

        private void Awake()
        {
            _objectAudioSource = gameObject.GetComponent<AudioSource>();
        }

        public IEnumerator PickUp()
        {
            yield return new WaitForSeconds(waitOnPickup);
            pickedUp = true;
            wasPickedUp = true;
        }
        private void PlayDropSound()
        {
            _objectAudioSource.clip = dropClips[Random.Range(0, dropClips.Length)];
            _objectAudioSource.Play();
        }
    }
}