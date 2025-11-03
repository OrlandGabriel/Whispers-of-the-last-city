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
    [SerializeField] private float roamRadius = 10f; // How far from spawn point to roam
    [SerializeField] private float roamWaitTime = 2f; // Wait time at each roam point
    [SerializeField] private float idleTimeBeforeRoam = 1f; // Time to wait before picking new roam point

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

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        // Save spawn point for roaming
        spawnPoint = transform.position;

        // Try to snap monster to NavMesh if not close enough
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
            return;
        }

        // Configure NavMeshAgent
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
        else
        {
            m_Agent.updatePosition = false;
            m_Agent.updateRotation = false;
        }

        m_Agent.isStopped = false;

        // Find the Timer script
        timerScript = Object.FindFirstObjectByType<Timer>();
        if (timerScript == null && showDebugInfo)
        {
            Debug.LogWarning("Timer script not found in scene!");
        }

        if (Target == null)
        {
            Debug.LogError("✗ NO TARGET ASSIGNED! Monster won't chase without a target!");
        }

        if (showDebugInfo) Debug.Log("Monster AI initialized successfully");
    }

    void Update()
    {
        if (playerIsDead) return;

        // Check if agent is on NavMesh
        if (!m_Agent.isOnNavMesh)
        {
            Debug.LogError("Monster is NOT on NavMesh!");
            return;
        }

        // Calculate distance to target
        if (Target != null)
        {
            m_Distance = Vector3.Distance(transform.position, Target.position);
        }

        // Decide behavior based on player distance
        if (Target != null && m_Distance <= detectionRange)
        {
            // Player is in range - chase or attack
            isRoaming = false;
            isWaiting = false;

            if (m_Distance <= AttackDistance)
            {
                AttackBehavior();
            }
            else
            {
                ChaseBehavior();
            }
        }
        else
        {
            // Player is out of range or no target - roam
            if (enableRoaming)
            {
                RoamBehavior();
            }
            else
            {
                IdleBehavior();
            }
        }

        // Debug visualization
        if (Target != null)
        {
            Debug.DrawLine(transform.position, Target.position,
                m_Distance <= AttackDistance ? Color.red : 
                m_Distance <= detectionRange ? Color.yellow : Color.gray);
        }
    }

    void AttackBehavior()
    {
        // Stop and attack
        m_Agent.isStopped = true;
        m_Agent.ResetPath();

        if (m_Animator != null)
        {
            m_Animator.SetBool("isAttacking", true);
            m_Animator.SetBool("isWalking", false);
        }

        // Face target
        Vector3 direction = (Target.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // Execute attack
        if (Time.time >= nextAttackTime)
        {
            AttackPlayer();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void ChaseBehavior()
    {
        // Chase the player
        m_Agent.isStopped = false;
        m_Agent.SetDestination(Target.position);

        if (m_Animator != null)
        {
            m_Animator.SetBool("isAttacking", false);
            m_Animator.SetBool("isWalking", true);
        }

        if (showDebugInfo)
        {
            Debug.DrawLine(transform.position, Target.position, Color.green);
        }
    }

    void RoamBehavior()
    {
        if (m_Animator != null)
        {
            m_Animator.SetBool("isAttacking", false);
        }

        // Handle waiting at roam point
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            
            if (m_Animator != null)
            {
                m_Animator.SetBool("isWalking", false);
            }

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                isRoaming = false;
            }
            return;
        }

        // Check if we need to pick a new roam point
        if (!isRoaming || !m_Agent.hasPath || m_Agent.remainingDistance < 0.5f)
        {
            if (isRoaming && m_Agent.remainingDistance < 0.5f)
            {
                // Reached roam point, wait before picking new one
                isWaiting = true;
                waitTimer = roamWaitTime;
                m_Agent.isStopped = true;
                return;
            }

            // Pick new random roam point
            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection += spawnPoint;
            randomDirection.y = transform.position.y; // Keep same height

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, roamRadius, NavMesh.AllAreas))
            {
                m_Agent.isStopped = false;
                m_Agent.SetDestination(navHit.position);
                isRoaming = true;

                if (showDebugInfo)
                {
                    Debug.DrawLine(transform.position, navHit.position, Color.blue, 2f);
                }
            }
        }

        // Update walking animation
        if (m_Animator != null && m_Agent.velocity.magnitude > 0.1f)
        {
            m_Animator.SetBool("isWalking", true);
        }
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
        if (showDebugInfo) Debug.Log("Monster attacks player!");

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
        {
            timerScript.GameOver();
            Debug.Log("Game Over triggered by Monster!");
        }
        else
        {
            Debug.LogError("Cannot trigger Game Over - Timer script not found!");
        }
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
        // Show attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackDistance);

        // Show detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Show roam radius from spawn point
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