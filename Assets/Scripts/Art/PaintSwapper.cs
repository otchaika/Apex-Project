using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class PaintSwapper : MonoBehaviour
{
    public PaintingManager manager;
    private XRGrabInteractable interactable;

    public Color thisColor = Color.yellow;
    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<XRGrabInteractable>();
        interactable.hoverEntered.AddListener(SwapColor);
    }

    void SwapColor(HoverEnterEventArgs args)
    {
        manager.SwapColor(thisColor);
    }
    

    
}
