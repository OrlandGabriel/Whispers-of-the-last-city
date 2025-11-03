using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("Target & Combat")]
    public Transform Target;
    public float AttackDistance = 2f;
    public float attackCooldown = 1.5f;
    public float detectionRange = 15f; // How far monster can detect player

    [Header("Roaming Settings")]
    [SerializeField] private bool enableRoaming = true;
    [SerializeField] private float roamRadius = 10f;
    [SerializeField] private float roamWaitTime = 2f;
    [SerializeField] private float idleTimeBeforeRoam = 1f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 120f;

    [Header("Game Over Settings")]
    [SerializeField] private float delayBeforeGameOver = 1f;

    [Header("Settings")]
    public bool useRootMotion = false;
    [SerializeField] private bool showDebugInfo = false;

    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private float m_Distance;
    private float nextAttackTime = 0f;
    private bool playerIsDead = false;
    private Timer timerScript;

    // Roaming variables
    private Vector3 spawnPoint;
    private bool isRoaming = false;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private Vector3 currentRoamPoint;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        spawnPoint = transform.position;

        // Ensure monster starts on NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            spawnPoint = hit.position;
            if (showDebugInfo) Debug.Log("✓ Monster snapped to NavMesh at: " + hit.position);
        }
        else
        {
            Debug.LogError("✗ Monster is too far from NavMesh!");
            enabled = false;
            return;
        }

        // Configure NavMesh Agent
        m_Agent.enabled = true;
        m_Agent.speed = moveSpeed;
        m_Agent.angularSpeed = rotationSpeed;
        m_Agent.acceleration = 8f;
        m_Agent.stoppingDistance = 0.5f;
        m_Agent.autoBraking = true;

        if (!useRootMotion)
        {
            m_Agent.updatePosition = true;
            m_Agent.updateRotation = true;
        }

        timerScript = Object.FindFirstObjectByType<Timer>();

        if (Target == null)
        {
            Debug.LogError("✗ NO TARGET ASSIGNED! Monster won't chase without a target!");
        }
    }

    void Update()
    {
        if (playerIsDead || !m_Agent.isOnNavMesh) return;

        if (Target != null)
            m_Distance = Vector3.Distance(transform.position, Target.position);

        // --- DETECTION LOGIC ---
        if (Target != null && m_Distance <= detectionRange)
        {
            // Player detected
            StopAllCoroutines(); // stop roaming coroutine if any
            isWaiting = false;
            isRoaming = false;

            if (m_Distance <= AttackDistance)
                AttackBehavior();
            else
                ChaseBehavior();
        }
        else
        {
            // Player not detected
            if (enableRoaming)
                RoamBehavior();
            else
                IdleBehavior();
        }

        // Debug Lines
        if (Target != null)
        {
            Debug.DrawLine(transform.position, Target.position,
                m_Distance <= AttackDistance ? Color.red :
                m_Distance <= detectionRange ? Color.yellow : Color.gray);
        }
    }

    void AttackBehavior()
    {
        m_Agent.isStopped = true;

        if (m_Animator != null)
        {
            m_Animator.SetBool("isAttacking", true);
            m_Animator.SetBool("isWalking", false);
        }

        // Face the target
        Vector3 direction = (Target.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // Perform attack if cooldown expired
        if (Time.time >= nextAttackTime)
        {
            AttackPlayer();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void ChaseBehavior()
    {
        m_Agent.isStopped = false;
        m_Agent.SetDestination(Target.position);

        if (m_Animator != null)
        {
            m_Animator.SetBool("isAttacking", false);
            m_Animator.SetBool("isWalking", true);
        }
    }

    void RoamBehavior()
    {
        if (m_Animator != null)
            m_Animator.SetBool("isAttacking", false);

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            m_Agent.isStopped = true;

            if (m_Animator != null)
                m_Animator.SetBool("isWalking", false);

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                PickNewRoamPoint();
            }
            return;
        }

        if (isRoaming && m_Agent.hasPath)
        {
            if (!m_Agent.pathPending && m_Agent.remainingDistance <= m_Agent.stoppingDistance + 0.2f)
            {
                // Arrived at roam point
                isRoaming = false;
                isWaiting = true;
                waitTimer = roamWaitTime;
                if (showDebugInfo) Debug.Log("Reached roam point, waiting...");
            }
        }
        else if (!isRoaming && !isWaiting)
        {
            PickNewRoamPoint();
        }

        if (m_Animator != null)
            m_Animator.SetBool("isWalking", m_Agent.velocity.magnitude > 0.1f);
    }

    void PickNewRoamPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * roamRadius;
            Vector3 randomPoint = spawnPoint + new Vector3(randomCircle.x, 0, randomCircle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, roamRadius, NavMesh.AllAreas))
            {
                currentRoamPoint = hit.position;
                m_Agent.isStopped = false;
                m_Agent.SetDestination(currentRoamPoint);
                isRoaming = true;

                if (showDebugInfo)
                    Debug.DrawLine(transform.position, currentRoamPoint, Color.blue, 2f);

                return;
            }
        }

        if (showDebugInfo) Debug.LogWarning("Failed to find valid roam point.");
        isWaiting = true;
        waitTimer = idleTimeBeforeRoam;
    }

    void IdleBehavior()
    {
        m_Agent.isStopped = true;

        if (m_Animator != null)
        {
            m_Animator.SetBool("isAttacking", false);
            m_Animator.SetBool("isWalking", false);
        }
    }

    void AttackPlayer()
    {
        if (!playerIsDead)
            StartCoroutine(AttackAndTriggerGameOver());
    }

    private IEnumerator AttackAndTriggerGameOver()
    {
        playerIsDead = true;

        if (m_Animator != null)
            m_Animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(delayBeforeGameOver);

        if (timerScript != null)
            timerScript.GameOver();
        else
            Debug.LogError("Timer script missing!");
    }

    void OnAnimatorMove()
    {
        if (!useRootMotion) return;

        Vector3 position = m_Animator.rootPosition;
        position.y = m_Agent.nextPosition.y;
        transform.position = position;
        m_Agent.nextPosition = transform.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (enableRoaming)
        {
            Gizmos.color = Color.blue;
            Vector3 center = Application.isPlaying ? spawnPoint : transform.position;
            Gizmos.DrawWireSphere(center, roamRadius);
        }

        if (Target != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, Target.position);
        }
    }
}
