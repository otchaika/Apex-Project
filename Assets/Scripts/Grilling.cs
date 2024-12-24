using System.Collections;
using UnityEngine;

public class Grilling : MonoBehaviour
{
    private float timer;
    public Material grilledMaterial; // Assign the new material in the Unity Editor

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Mushroom"))
        {
            timer += Time.deltaTime; // Increment timer when the object is in the trigger

            if (timer >= 4.0f) // Check if 4 seconds have elapsed
            {
                ChangeMaterial(other.gameObject); // Change the material
                timer = 0.0f; // Reset the timer
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Mushroom"))
        {
            timer = 0.0f; // Reset the timer when the object exits the trigger
        }
    }

    private void ChangeMaterial(GameObject target)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = grilledMaterial; // Change the material
        }
    }
}