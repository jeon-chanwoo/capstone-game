using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapOn : MonoBehaviour
{
    public RawImage miniMapImage;
    public float fadeInDuration = 2.0f; // ���̵� �� ���� �ð�

    private void Start()
    {
        StartCoroutine(FadeInAfterDelay());
    }

    private IEnumerator FadeInAfterDelay()
    {
       

        // �̴ϸ� ������Ʈ�� �ʱ�ȭ�ϰ� ���� ���� 0���� �����Ͽ� ����ϴ�.
        Color startColor = miniMapImage.color;
        startColor.a = 0.0f;
        miniMapImage.color = startColor;
        yield return new WaitForSeconds(4.0f); // 5�� ������
        StartCoroutine(FadeInMiniMap());
    }

    private IEnumerator FadeInMiniMap()
    {
        Color originalColor = miniMapImage.color;
        Color targetColor = originalColor;
        targetColor.a = 1.0f; // ���� �� 1�� �����Ͽ� ������ ���̵��� ��

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

        // ���̵� ���� �Ϸ�Ǿ��� ���� ó��
    }
}