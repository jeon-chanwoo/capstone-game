using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapOn : MonoBehaviour
{
    public RawImage miniMapImage;
    public float fadeInDuration = 2.0f; // 페이드 인 지속 시간

    private void Start()
    {
        StartCoroutine(FadeInAfterDelay());
    }

    private IEnumerator FadeInAfterDelay()
    {
       

        // 미니맵 오브젝트를 초기화하고 알파 값을 0으로 설정하여 숨깁니다.
        Color startColor = miniMapImage.color;
        startColor.a = 0.0f;
        miniMapImage.color = startColor;
        yield return new WaitForSeconds(4.0f); // 5초 딜레이
        StartCoroutine(FadeInMiniMap());
    }

    private IEnumerator FadeInMiniMap()
    {
        Color originalColor = miniMapImage.color;
        Color targetColor = originalColor;
        targetColor.a = 1.0f; // 알파 값 1로 설정하여 완전히 보이도록 함

        float elapsedTime = 0.0f;

        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0.0f, 1.0f, elapsedTime / fadeInDuration);
            Color newColor = miniMapImage.color;
            newColor.a = alpha;
            miniMapImage.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 페이드 인이 완료되었을 때의 처리
    }
}