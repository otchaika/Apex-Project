using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStateMachine
{
    public enum STATE
    {
        IDLE, WALK
    };

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public STATE currentStateName;
    public EVENT stage;
    protected GameObject npc;
    protected Rigidbody rb;
    protected Animator anim;
    protected Transform player;
    protected NPCStateMachine nextState;
    protected float speed = 1.5f; // Скорость движения
    protected float rotationSpeed = 2.0f; // Скорость поворота

    public NPCStateMachine(GameObject _npc, Rigidbody _rb, Animator _anim, Transform _player)
    {
        npc = _npc ?? throw new System.ArgumentNullException(nameof(_npc), "NPC is null");
        rb = _rb ?? throw new System.ArgumentNullException(nameof(_rb), "Rigidbody is null");
        anim = _anim ?? throw new System.ArgumentNullException(nameof(_anim), "Animator is null");
        player = _player;
        stage = EVENT.ENTER; // Начальная стадия
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }

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

    protected void MoveTowards(Vector3 targetPosition)
    {
        // Определяем направление к цели
        Vector3 directionToTarget = (targetPosition - npc.transform.position).normalized;

        // Убираем вертикальную компоненту, чтобы NPC двигался только по горизонтальной плоскости
        directionToTarget.y = 0;

        // Вычисляем целевое вращение для NPC
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Плавно поворачиваем NPC в сторону цели
        npc.transform.rotation = Quaternion.Slerp(
            npc.transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // Двигаем NPC вперёд в его текущем направлении
        Vector3 moveDirection = npc.transform.forward * (speed * Time.deltaTime);
        rb.MovePosition(npc.transform.position + moveDirection);
    }

    public class Idle : NPCStateMachine
    {
        private float idleDuration;
        private float idleTimer = 0f;
        private Waypoint nextWaypoint;

        public Idle(GameObject _npc, Rigidbody _rb, Animator _anim, Transform _player, float duration, Waypoint nextWaypoint)
            : base(_npc, _rb, _anim, _player)
        {
            currentStateName = STATE.IDLE;
            idleDuration = duration;
            this.nextWaypoint = nextWaypoint;
        }

        public override void Enter()
        {
            anim.SetTrigger("DoIdle"); // Запуск Idle анимации

            // Остановка Rigidbody, чтобы NPC не двигался
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            base.Enter();
            idleTimer = 0f;
        }

        public override void Update()
        {
            // Увеличиваем таймер ожидания
            idleTimer += Time.deltaTime;

            // Если время ожидания истекло, возвращаемся в Walk
            if (idleTimer >= idleDuration)
            {
                nextState = new Walk(npc, rb, anim, player, nextWaypoint);
                stage = EVENT.EXIT;
            }
        }

        public override void Exit()
        {
            anim.ResetTrigger("DoIdle"); // Сброс триггера Idle
            base.Exit();
        }
    }

    public class Walk : NPCStateMachine
    {
        private Waypoint currentWaypoint;

        public Walk(GameObject _npc, Rigidbody _rb, Animator _anim, Transform _player, Waypoint startWaypoint)
            : base(_npc, _rb, _anim, _player)
        {
            currentStateName = STATE.WALK;
            currentWaypoint = startWaypoint;
        }

        public override void Enter()
        {
            anim.SetTrigger("DoWalk"); // Запуск анимации движения

            if (currentWaypoint == null)
            {
                Debug.LogError("No starting waypoint set! Switching to Idle.");
                nextState = new Idle(npc, rb, anim, player, 2f, null);
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
                nextState = new Idle(npc, rb, anim, player, 2f, null);
                stage = EVENT.EXIT;
                return;
            }

            Vector3 targetPosition = currentWaypoint.transform.position;

            // Движемся к текущей точке с плавным поворотом
            MoveTowards(targetPosition);

            // Проверяем, достигли ли текущей точки
            if (Vector3.Distance(npc.transform.position, targetPosition) < 1f)
            {
                Debug.Log($"Reached waypoint: {currentWaypoint.name}");

                if (currentWaypoint.isIdlePoint)
                {
                    Debug.Log($"Switching to Idle at waypoint: {currentWaypoint.name}");
                    nextState = new Idle(npc, rb, anim, player, 5f, currentWaypoint.nextWaypoint);
                    stage = EVENT.EXIT;
                }
                else
                {
                    // Переход к следующей точке
                    currentWaypoint = currentWaypoint.nextWaypoint;
                    if (currentWaypoint == null)
                    {
                        Debug.Log("No more waypoints. Stopping.");
                        nextState = new Idle(npc, rb, anim, player, 2f, null);
                        stage = EVENT.EXIT;
                    }
                }
            }
        }

        public override void Exit()
        {
            anim.ResetTrigger("DoWalk");
            base.Exit();
        }
    }
}
