using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finite State Machine for NPC behavior.
/// </summary>
public class NPCStateMachine
{
    // Possible states of the NPC
    public enum STATE
    {
        IDLE, WALK
    };

    // Stages of a state (entry, update, exit)
    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public STATE currentStateName;
    public EVENT stage;
    protected GameObject npc;
    protected Rigidbody rb;
    protected Animator anim;
    protected NPCStateMachine nextState;
    protected float speed = 1.5f; // Movement speed
    protected float rotationSpeed = 2.0f; // Rotation speed

    /// <summary>
    /// Constructor for NPCStateMachine.
    /// </summary>
    public NPCStateMachine(GameObject _npc, Rigidbody _rb, Animator _anim)
    {
        npc = _npc ?? throw new System.ArgumentNullException(nameof(_npc), "NPC is null");
        rb = _rb ?? throw new System.ArgumentNullException(nameof(_rb), "Rigidbody is null");
        anim = _anim ?? throw new System.ArgumentNullException(nameof(_anim), "Animator is null");
        stage = EVENT.ENTER; // Initial stage
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }

    /// <summary>
    /// Processes the current state and transitions to the next if necessary.
    /// </summary>
    public NPCStateMachine Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }

    /// <summary>
    /// Moves the NPC towards the target position with smooth rotation.
    /// </summary>
    protected void MoveTowards(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - npc.transform.position).normalized;
        directionToTarget.y = 0; // Ensures movement stays on the horizontal plane
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Smoothly rotates NPC towards the target
        npc.transform.rotation = Quaternion.Slerp(
            npc.transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // Moves NPC forward in the direction it's facing
        Vector3 moveDirection = npc.transform.forward * (speed * Time.deltaTime);
        rb.MovePosition(npc.transform.position + moveDirection);
    }

    /// <summary>
    /// Idle state where the NPC waits for a specified duration before transitioning.
    /// </summary>
    public class Idle : NPCStateMachine
    {
        private float idleDuration;
        private float idleTimer = 0f;
        private Waypoint nextWaypoint;

        public Idle(GameObject _npc, Rigidbody _rb, Animator _anim, float duration, Waypoint nextWaypoint)
            : base(_npc, _rb, _anim)
        {
            currentStateName = STATE.IDLE;
            idleDuration = duration;
            this.nextWaypoint = nextWaypoint;
        }

        public override void Enter()
        {
            anim.SetBool("DoIdle", true); // Triggers idle animation
            rb.velocity = Vector3.zero; // Stops movement
            rb.angularVelocity = Vector3.zero;

            base.Enter();
            idleTimer = 0f;
        }

        public override void Update()
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleDuration)
            {
                nextState = new Walk(npc, rb, anim, nextWaypoint);
                stage = EVENT.EXIT;
            }
        }

        public override void Exit()
        {
            anim.SetBool("DoIdle", false); // Resets idle animation trigger
            base.Exit();
        }
    }

    /// <summary>
    /// Walking state where the NPC moves between waypoints.
    /// </summary>
    public class Walk : NPCStateMachine
    {
        private Waypoint currentWaypoint;

        public Walk(GameObject _npc, Rigidbody _rb, Animator _anim, Waypoint startWaypoint)
            : base(_npc, _rb, _anim)
        {
            currentStateName = STATE.WALK;
            currentWaypoint = startWaypoint;
        }

        public override void Enter()
        {
            anim.SetBool("DoWalk", true); // Triggers walking animation

            if (currentWaypoint == null)
            {
                Debug.LogError("No starting waypoint set! Switching to Idle.");
                nextState = new Idle(npc, rb, anim, 2f, null);
                stage = EVENT.EXIT;
                return;
            }

            base.Enter();
        }

        public override void Update()
        {
            if (currentWaypoint == null)
            {
               Debug.LogError("No waypoint to walk to. Switching to Idle.");
                nextState = new Idle(npc, rb, anim, 2f, null);
                stage = EVENT.EXIT;
                return;
            }

            Vector3 targetPosition = currentWaypoint.transform.position;
            MoveTowards(targetPosition);

            if (Vector3.Distance(npc.transform.position, targetPosition) < 1f)
            {
                //Debug.Log($"Reached waypoint: {currentWaypoint.name}");

                if (currentWaypoint.isIdlePoint)
                {
                    Debug.Log($"Switching to Idle at waypoint: {currentWaypoint.name}");
                    nextState = new Idle(npc, rb, anim, 5f, currentWaypoint.nextWaypoint);
                    stage = EVENT.EXIT;
                }
                else
                {
                    currentWaypoint = currentWaypoint.nextWaypoint;
                    if (currentWaypoint == null)
                    {
                        //Debug.Log("No more waypoints. Stopping.");
                        nextState = new Idle(npc, rb, anim, 2f, null);
                        stage = EVENT.EXIT;
                    }
                }
            }
        }

        public override void Exit()
        {
            anim.SetBool("DoWalk", false); // Resets walking animation trigger
            base.Exit();
        }
    }
}
