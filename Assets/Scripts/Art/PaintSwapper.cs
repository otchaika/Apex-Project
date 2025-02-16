using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;

public class PaintSwapper : MonoBehaviour
{
    public PaintingManager manager;

    [SerializeField] private AudioClip Wash;
    AudioSource Washing;

    public Color thisColor = Color.yellow;
    // Start is called before the first frame update
    void Start()
    {
        Washing = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided with " + other.name);
        if (other.gameObject.CompareTag("PaintingFinger"))
        {
            if (other.gameObject.name.Contains("Left"))
            {
                manager.SwapColor(thisColor, "left");
                if (!Washing.isPlaying)
                {
                    Washing.clip = Wash;
                    Washing.Play(0);
                }
            }
            else if (other.gameObject.name.Contains("Right"))
            {
                manager.SwapColor(thisColor, "right");
                if (!Washing.isPlaying)
                {
                    Washing.clip = Wash;
                    Washing.Play(0);
                }
            }
            //Checking the painting collider of the brush
            else if (other.gameObject.name.Contains("paint_coll"))
                manager.SwapColor(thisColor, "brush");
        }
            

    }
    

    
}
