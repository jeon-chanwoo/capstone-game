using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiOn : MonoBehaviour
{
    public Slider slider;
    public float fadeInDuration = 2.0f; // ���̵� �� ���� �ð�

    [SerializeField] private Image background;
    [SerializeField] private Image fill;
    [SerializeField] private Text fillText;

    private void Start()
    {
        StartCoroutine(FadeInAfterDelay());
    }

    private IEnumerator FadeInAfterDelay()
    {
        // ���, ä��� �̹��� �� �ؽ�Ʈ ������Ʈ�� ������ �ʱ�ȭ�ϰ� ���� ���� 0���� �����Ͽ� ����ϴ�.
        Color startColorBackground = background.color;
        startColorBackground.a = 0.0f;
        background.color = startColorBackground;

        Color startColorFill = fill.color;
        startColorFill.a = 0.0f;
        fill.color = startColorFill;

        Color startColorText = fillText.color;
        startColorText.a = 0.0f;
        fillText.color = startColorText;

        yield return new WaitForSeconds(4.0f); // 4�� ������
        StartCoroutine(FadeInSlider());
    }

    private IEnumerator FadeInSlider()
    {
        Color originalColorBackground = background.color;
        Color targetColorBackground = originalColorBackground;
        targetColorBackground.a = 1.0f; // ���� �� 1�� �����Ͽ� ������ ���̵��� ��

        Color originalColorFill = fill.color;
        Color targetColorFill = originalColorFill;
        targetColorFill.a = 1.0f; // ���� �� 1�� �����Ͽ� ������ ���̵��� ��

        Color originalColorText = fillText.color;
        Color targetColorText = originalColorText;
        targetColorText.a = 1.0f; // ���� �� 1�� �����Ͽ� ������ ���̵��� ��

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

        // ���̵� ���� �Ϸ�Ǿ��� ���� ó��
    }
}