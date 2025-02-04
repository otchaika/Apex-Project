using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    public float wait_time = 11f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait_for_intro());
    }
    IEnumerator Wait_for_intro() 
    {
        yield return new WaitForSeconds(wait_time);
        SceneManager.LoadScene(1);
    }

}
