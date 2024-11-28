using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHands : MonoBehaviour
{
    public InputActionReference pinchAction;
    public InputActionReference gripAction;
    private Animator anim;
        
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float value = pinchAction.action.ReadValue<float>();
        anim.SetFloat("Trigger", value);
        value = gripAction.action.ReadValue<float>();
        anim.SetFloat("Grip", value);
    }
}
