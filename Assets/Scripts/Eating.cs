using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eating : MonoBehaviour
{
    public Material grilledMaterial; // Assign the exact "Grilled" material in the Unity Inspector
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mushroom"))
        {
            Renderer renderer = other.GetComponent<Renderer>();
            if (renderer != null)
            {
                Debug.Log("Renderer material: " + renderer.sharedMaterial.name);
                // Compare using sharedMaterial
                if (renderer.sharedMaterial == grilledMaterial)
                {
                    Debug.Log("The object is a grilled mushroom!");
                    other.gameObject.SetActive(false);
                    //Destroy(other.gameObject);
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
