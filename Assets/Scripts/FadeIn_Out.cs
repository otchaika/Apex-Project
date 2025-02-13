using System.Collections;
using UnityEngine;

public class FadeIn_Out : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1.0f;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Debug.LogError("CanvasGroup не назначен!");
            }
        }
    }

    public IEnumerator FadeIn()
    {
        Debug.Log("Запуск FadeIn");
        float elapsedTime = 0;
        while (canvasGroup.alpha < 1)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        Debug.Log("FadeIn завершён");
    }

    public IEnumerator FadeOut()
    {
        Debug.Log("Запуск FadeOut");
        float elapsedTime = 0;
        while (canvasGroup.alpha > 0)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - elapsedTime / fadeDuration);
            yield return null;
        }
        Debug.Log("FadeOut завершён");
    }
}
