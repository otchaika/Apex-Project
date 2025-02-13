using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1.0f;
    public float wait_time = 11f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait_for_intro());
    }
    IEnumerator Wait_for_intro() 
    {
        yield return new WaitForSeconds(wait_time);
        yield return StartCoroutine(FadeIn());
        SceneManager.LoadScene(1);
    }
    public IEnumerator FadeIn()
    {
        
        Debug.Log("Запуск FadeIn");
        float elapsedTime = 0;
        canvasGroup.blocksRaycasts = true; 
        while (canvasGroup.alpha < 1)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        Debug.Log("FadeIn завершён");
    }

}
