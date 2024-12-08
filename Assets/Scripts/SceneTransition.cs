using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldSpaceTransitions; // Импортируем пространство имен, где находится Transition

public class SceneTransition : MonoBehaviour
{
    public Transition transitionScript;
    public GameObject child;
    void Start()
    {
        

        transitionScript = child.GetComponent<Transition>();
        if (transitionScript == null)
        {
            Debug.LogError("Transition script is not attached to the Transit object!");
            return;
        }
        //Transition script = child.GetComponent<Transition>();//SetActive(true);
        
        // Для теста: сразу запускаем обратную анимацию
        //transitionScript.TriggerBackwardTransition();*/
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Transition triggered");
            
            
            StartCoroutine(HandleSceneTransition());
            //transitionScript.TriggerBackwardTransition();
        }
        else
        {
            Debug.LogError("Transition script is not ready or Player tag is incorrect!");
        }
    }

    IEnumerator HandleSceneTransition()
    {
        yield return new WaitForSeconds(1);
        child.SetActive(true);
        transitionScript.TriggerBackwardTransition();
        // Ждём завершения обратной анимации (время задаётся bwdInterval в Transition)
        yield return new WaitForSeconds(transitionScript.bwdInterval+1);

        // Загружаем следующую сцену
        SceneManager.LoadScene(1);

        // Ждём пока сцена загрузится
        //yield return new WaitUntil(() => SceneManager.GetActiveScene().buildIndex == 1);
    }
}
