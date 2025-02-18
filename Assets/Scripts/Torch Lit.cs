using UnityEngine;

public class ChangeMaterialOnCollision : MonoBehaviour
{
    public Material newMaterial;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected");

        if (collision.gameObject.CompareTag("Campfire"))
        {
            Debug.Log("Collision with Campfire detected");

            Renderer objectRenderer = GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                objectRenderer.material = newMaterial;
                Debug.Log("Material changed successfully");
            }
            else
            {
                Debug.Log("Renderer component not found");
            }
        }
    }
}
