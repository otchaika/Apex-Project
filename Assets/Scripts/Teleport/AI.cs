using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    Rigidbody rb;
    Animator anim;
    public Transform player;
    public Waypoint startWaypoint; // Начальный Waypoint
    NPCStateMachine currentState;

    void Start()
    {
        Debug.Log("AI Start method called.");

        rb = this.GetComponent<Rigidbody>();
        anim = this.GetComponent<Animator>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player != null)
            {
                Debug.Log("Player found: " + player.name);
            }
            else
            {
                Debug.LogError("Player not found");
            }
        }

        if (startWaypoint == null)
        {
            Debug.LogError("Start Waypoint is not set!");
            return;
        }

        // Устанавливаем начальное состояние как Walk с начальным waypoint
        currentState = new NPCStateMachine.Walk(this.gameObject, rb, anim, player, startWaypoint);
    }

    void Update()
    {
        // Обрабатываем текущее состояние
        currentState = currentState.Process();
    }
}