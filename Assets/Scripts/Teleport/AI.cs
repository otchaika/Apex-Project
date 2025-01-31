using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the NPC behavior by managing its state machine.
/// </summary>
public class AI : MonoBehaviour
{
    public Rigidbody rb;
    public Animator anim;
    public Waypoint startWaypoint; // Initial waypoint for movement
    NPCStateMachine currentState;

    void Start()
    {
        //Debug.Log("AI Start method called.");

        // Get Rigidbody and Animator components
        rb = this.GetComponent<Rigidbody>();
        anim = this.GetComponent<Animator>();

        if (startWaypoint == null)
        {
            Debug.LogError("Start Waypoint is not set!");
            return;
        }

        // Set initial state to Walk
        currentState = new NPCStateMachine.Walk(this.gameObject, rb, anim, startWaypoint);
    }

    void Update()
    {
        // Process the current state and transition if needed
        currentState = currentState.Process();
    }
}