using UnityEngine;
using UnityEngine.Audio;

namespace Suntail
{
    public class AudioZoneControl : MonoBehaviour
    {
        //야외
        [Tooltip("Snapshot for outdoor enviroment")]
        [SerializeField] private AudioMixerSnapshot outdoorSnapshot;
        //실내
        [Tooltip("Snapshot for indoor enviroment")]
        [SerializeField] private AudioMixerSnapshot indoorSnapshot;
        //전환시간
        [Tooltip("Transition time between snapshots")]
        [SerializeField] private float crossfadeTime = 0.5f;

        //오디오 업데이트를 위한 트리거 설정
        [Tooltip("Trigger tag for updating audio zones")]
        [SerializeField] private string triggerTag = "Player";

        //Private variables
        //오디오 영역 카운트
        private int zoneCount;

        //Trigger, changing snapshot when player enter zone
        //플레이어가 영역에 진입할때 스냅샷 변경
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == triggerTag)
            {
                zoneCount += 1;
                UpdateAudioZoneSnapshot();
            }
        }

        //Trigger, changing snapshot when player leave zone
        //플레이어가 나갈때 스냅샷 변경
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == triggerTag)
            {
                zoneCount -= 1;
                UpdateAudioZoneSnapshot();
            }
        }

        //Update snapshot, depending on the location
        //스냡샷 변경을 위한 코드
        private void UpdateAudioZoneSnapshot()
        {
            if (zoneCount > 0)
            {
                indoorSnapshot.TransitionTo(crossfadeTime);
            }
            else
            {
                outdoorSnapshot.TransitionTo(crossfadeTime);
            }
        }
    }
}