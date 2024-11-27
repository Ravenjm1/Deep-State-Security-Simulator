using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : NetworkBehaviour
{
    // Перечисление состояний
    private enum State
    {
        Idle,
        Run,
        Attack,
        Dead
    }
    
    private State currentState = State.Idle; // Начальное состояние

    float idleMoveInterval = 5f; // Интервал перемещения
    float idleMoveRadius = 5f;  // Радиус перемещения

    float idleSpeed = 2f;
    float runSpeed = 4.5f;

    [SyncVar] NetworkIdentity netTargetPlayer;
    Transform targetPlayer;     // Ссылка на игрока
    float detectionRange = 10f; // Радиус обнаружения игрока
    LayerMask obstacleMask;     // Маска для проверки препятствий

    float attackRange = 2.5f;     // Дистанция атаки
    float attackDamage = 1f;   // Урон
    float attackCooldown = 0.5f;  // Время между атаками

    private float maxHealth = 100f;
    private float currentHealth;

    private NavMeshAgent agent;        // Для навигации
    private EnemyAnimator animator;         // Анимации

    private float idleTimer;           // Таймер для Idle
    private bool isAttacking;          // Флаг атаки

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<EnemyAnimator>();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Run:
                HandleRun();
                break;
            case State.Attack:
                HandleAttack();
                break;
            case State.Dead:
                HandleDead();
                break;
        }

        FindNearPlayer();
    }

    void FindNearPlayer()
    {
        if (isServer)
        {
            netTargetPlayer = null;
            targetPlayer = null;
            float _dist = 9999;

            foreach(PlayerController player in LocationContext.GetDependency.ListPlayers)
            {
                if (CanSeePlayer(player.transform))
                {
                    var _newDist = Vector3.Distance(transform.position, player.transform.position);
                    if (_newDist < _dist)
                    {
                        targetPlayer = player.transform;
                        _dist = _newDist;
                    }
                }
            }
            if (targetPlayer)
                netTargetPlayer = targetPlayer.GetComponent<NetworkIdentity>();
        }
        else 
        {
            targetPlayer = netTargetPlayer? netTargetPlayer.transform : null;  
        }
    }

    // 1. Idle State
    private void HandleIdle()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleMoveInterval)
        {
            idleTimer = 0;
            Vector3 randomDirection = Random.insideUnitSphere * idleMoveRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, idleMoveRadius, 1))
            {
                agent.SetDestination(hit.position);
            }
        }

        agent.speed = idleSpeed;
        animator.MoveAnimation(0.5f);


        // Проверка на игрока
        if (CanSeePlayer(targetPlayer))
        {
            currentState = State.Run;
        }
    }

    // 2. Run State
    private void HandleRun()
    {
        if (targetPlayer == null) return;

        agent.SetDestination(targetPlayer.position);
        agent.speed = runSpeed;
        animator.MoveAnimation(1);

        // Если расстояние до игрока меньше дистанции атаки
        if (Vector3.Distance(transform.position, targetPlayer.position) <= attackRange)
        {
            currentState = State.Attack;
        }
        else if (!CanSeePlayer(targetPlayer))
        {
            currentState = State.Idle;
        }
    }

    // 3. Attack State
    private void HandleAttack()
    {
        if (isAttacking) return;

        agent.isStopped = true; // Останавливаем движение
        animator.AttackAnimation(true); // Запускаем анимацию атаки
        isAttacking = true;

        // Урон игроку через время анимации
        Invoke(nameof(DealDamage), 0.2f);
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void DealDamage()
    {
        if (targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= attackRange)
        {
            // Здесь вызывается метод для уменьшения здоровья игрока
            if (LocationContext.GetDependency.Player.gameObject == targetPlayer.gameObject)
            {
                targetPlayer.GetComponent<PlayerStats>().GetDamage(attackDamage);
            }
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;
        agent.isStopped = false;

        animator.AttackAnimation(false);
        // Переход в другое состояние
        if (CanSeePlayer(targetPlayer) && Vector3.Distance(transform.position, targetPlayer.position) > attackRange)
        {
            currentState = State.Run;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    // 4. Dead State
    private void HandleDead()
    {
        agent.isStopped = true;
        animator.DieAnimation();
        // Отключение всех взаимодействий
        this.enabled = false;
    }

    // Уменьшение здоровья врага
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        if (currentState != State.Dead)
        {
            currentState = State.Dead;
            Invoke(nameof(Remove), 2f);
        }
    }
    [Server]
    private void Remove()
    {
        NetworkServer.Destroy(gameObject);
    }

    // Проверка видимости игрока
    private bool CanSeePlayer(Transform playerTransform)
    {
        if (playerTransform == null)
            return false;

        Vector3 directionToPlayer = playerTransform.position - transform.position;
        if (Vector3.Distance(transform.position, playerTransform.position) <= detectionRange &&
            !Physics.Raycast(transform.position, directionToPlayer, directionToPlayer.magnitude, obstacleMask))
        {
            return true;
        }

        return false;
    }
}