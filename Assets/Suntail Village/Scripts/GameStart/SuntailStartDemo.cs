using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

//This script is only used to start the Suntail demo scene
namespace Suntail
{
    public class SuntailStartDemo : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Image blackScreenImage;
        [SerializeField] private Text blackScreenText1;
        [SerializeField] private Text blackScreenText2;
        [SerializeField] private Text hintText1;
        [SerializeField] private Text hintText2;
        [SerializeField] private float blackScreenDuration = 4f;
        [SerializeField] private float hintDuration = 17f;
        [SerializeField] private float fadingDuration = 3f;
        
        //Private variables
        private bool screenTimerIsActive = true;
        private bool hintTimerIsActive = true;
        private bool isHintText1 = true;
        private bool isHintText2 = true;

        private void Start()
        {
            blackScreenImage.gameObject.SetActive(true);
            blackScreenText1.gameObject.SetActive(true);
            blackScreenText2.gameObject.SetActive(true);
            hintText1.gameObject.SetActive(true);
            hintText1.CrossFadeAlpha(0, 0, false);
            hintText2.gameObject.SetActive(true);
            hintText2.CrossFadeAlpha(0, 0, false);
            _audioMixer.SetFloat("soundsVolume", -80f);
        }

        private void Update()
        {
            //Black screen timer
            if (screenTimerIsActive) 
            {
                blackScreenDuration -= Time.deltaTime; //까만화면+제목 4초동안 먼저보임
                if (blackScreenDuration < 0)
                {
                    screenTimerIsActive = false;
                    blackScreenImage.CrossFadeAlpha(0, fadingDuration, false);//각화면은 3초동안 천천히 사라짐 총7초
                    blackScreenText1.CrossFadeAlpha(0, fadingDuration, false);
                    blackScreenText2.CrossFadeAlpha(0, fadingDuration, false);
                    StartCoroutine(StartAudioFade(_audioMixer, "soundsVolume", fadingDuration, 1f));
                }
            }

            //Hint text timer
            if (hintTimerIsActive)//17-7=10초동안 보임
            {
                hintDuration -= Time.deltaTime;//
                if(hintDuration < 9.0f && isHintText1)
                {
                    isHintText1= false;
                    hintText1.CrossFadeAlpha(1, fadingDuration, false);
                }
                if (hintDuration < 0)
                {
                    hintTimerIsActive = false;
                    hintText1.CrossFadeAlpha(0, fadingDuration, false);
                }
            }
            if(!hintTimerIsActive && isHintText2)
            {
                isHintText2 = false;
                StartCoroutine(HintText2Fade(hintText2,fadingDuration, 4.0f));
            }
        }
        public static IEnumerator HintText2Fade(Text text,float duration, float wait)
        {
            yield return new WaitForSeconds(wait);
            text.CrossFadeAlpha(1, duration, false);
            yield return new WaitForSeconds(wait+4.0f);
            text.CrossFadeAlpha(0, duration, false);
        }
        //Sound fading
        public static IEnumerator StartAudioFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
        {
            float currentTime = 0;
            float currentVol;
            audioMixer.GetFloat(exposedParam, out currentVol);
            currentVol = Mathf.Pow(10, currentVol / 20);
            float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
                audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
                yield return null;
            }
            yield break;
        }
    }
}