using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiOn : MonoBehaviour
{
    public Slider slider;
    public float fadeInDuration = 2.0f; // 페이드 인 지속 시간

    [SerializeField] private Image background;
    [SerializeField] private Image fill;
    [SerializeField] private Text fillText;

    private void Start()
    {
        StartCoroutine(FadeInAfterDelay());
    }

    private IEnumerator FadeInAfterDelay()
    {
        // 배경, 채우기 이미지 및 텍스트 오브젝트의 색상을 초기화하고 알파 값을 0으로 설정하여 숨깁니다.
        Color startColorBackground = background.color;
        startColorBackground.a = 0.0f;
        background.color = startColorBackground;

        Color startColorFill = fill.color;
        startColorFill.a = 0.0f;
        fill.color = startColorFill;

        Color startColorText = fillText.color;
        startColorText.a = 0.0f;
        fillText.color = startColorText;

        yield return new WaitForSeconds(4.0f); // 4초 딜레이
        StartCoroutine(FadeInSlider());
    }

    private IEnumerator FadeInSlider()
    {
        Color originalColorBackground = background.color;
        Color targetColorBackground = originalColorBackground;
        targetColorBackground.a = 1.0f; // 알파 값 1로 설정하여 완전히 보이도록 함

        Color originalColorFill = fill.color;
        Color targetColorFill = originalColorFill;
        targetColorFill.a = 1.0f; // 알파 값 1로 설정하여 완전히 보이도록 함

        Color originalColorText = fillText.color;
        Color targetColorText = originalColorText;
        targetColorText.a = 1.0f; // 알파 값 1로 설정하여 완전히 보이도록 함

        float elapsedTime = 0.0f;

        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0.0f, 1.0f, elapsedTime / fadeInDuration);

            Color newColorBackground = background.color;
            newColorBackground.a = alpha;
            background.color = newColorBackground;

            Color newColorFill = fill.color;
            newColorFill.a = alpha;
            fill.color = newColorFill;

            Color newColorText = fillText.color;
            newColorText.a = alpha;
            fillText.color = newColorText;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 페이드 인이 완료되었을 때의 처리
    }
}