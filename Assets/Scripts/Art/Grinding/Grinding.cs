using UnityEngine;

public class Grinding : MonoBehaviour
{
    [Header("Settings")]
    public int grindThreshold = 10; // Number of collisions required
    private int grindCount = 0;
    [SerializeField] private GameObject pigmentObj;
    [SerializeField] private ParticleSystem splash;

    void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is the small stone
        if (collision.gameObject.CompareTag("SmallStone"))
        {
            grindCount++;
            Debug.Log($"Grinding progress: {grindCount}/{grindThreshold}");
            splash.Play();
            if (grindCount >= grindThreshold)
            {
                CompleteGrinding();
            }
        }
        else
        {
            Debug.Log(collision.gameObject.tag);
        }
    }

    void CompleteGrinding()
    {
        Debug.Log("Ingredient transformed into pigment!");
        this.enabled = false;
        pigmentObj.SetActive(true);
        this.gameObject.SetActive(false);
        // Add logic to transform the ingredient into pigment
    }
}
