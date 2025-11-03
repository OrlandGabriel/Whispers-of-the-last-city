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

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 120f;

    [Header("Game Over Settings")]
    [SerializeField] private float delayBeforeGameOver = 1f;

    [Header("Settings")]
    public bool useRootMotion = false;
    [SerializeField] private bool showDebugInfo = true;

    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private float m_Distance;
    private float nextAttackTime = 0f;
    private bool playerIsDead = false;
    private Timer timerScript;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        Debug.Log("=== MONSTER AI INITIALIZATION ===");
        
        // Try to snap monster to NavMesh if not close enough
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            Debug.Log("✓ Monster snapped to NavMesh at: " + hit.position);
        }
        else
        {
            Debug.LogError("✗ Monster is too far from NavMesh! Move it closer to the blue area.");
            return;
        }

        // FORCE NavMeshAgent settings
        m_Agent.enabled = true;
        m_Agent.speed = moveSpeed;
        m_Agent.angularSpeed = rotationSpeed;
        m_Agent.acceleration = 8f;
        m_Agent.stoppingDistance = 0.5f;
        m_Agent.autoBraking = true;
        m_Agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        
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

        // Debug all settings
        Debug.Log($"Agent Enabled: {m_Agent.enabled}");
        Debug.Log($"Agent Speed: {m_Agent.speed}");
        Debug.Log($"Agent Is Stopped: {m_Agent.isStopped}");
        Debug.Log($"Agent On NavMesh: {m_Agent.isOnNavMesh}");
        Debug.Log($"Update Position: {m_Agent.updatePosition}");
        Debug.Log($"Update Rotation: {m_Agent.updateRotation}");

        // Find the Timer script in the scene
        timerScript = Object.FindFirstObjectByType<Timer>();
        if (timerScript == null)
        {
            Debug.LogWarning("Timer script not found in scene!");
        }

        if (Target != null)
        {
            Debug.Log($"✓ Target assigned: {Target.name}");
            float distanceToTarget = Vector3.Distance(transform.position, Target.position);
            Debug.Log($"Distance to target: {distanceToTarget}");
        }
        else
        {
            Debug.LogError("✗ NO TARGET ASSIGNED! Monster won't move without a target!");
        }

        Debug.Log("=== INITIALIZATION COMPLETE ===");
    }

    void Update()
    {
        if (Target == null)
        {
            if (showDebugInfo)
                Debug.LogWarning("Monster has no target!");
            return;
        }

        if (playerIsDead) return;

        // Check if agent is on NavMesh
        if (!m_Agent.isOnNavMesh)
        {
            Debug.LogError("Monster is NOT on NavMesh!");
            return;
        }

        m_Distance = Vector3.Distance(transform.position, Target.position);

        if (m_Distance <= AttackDistance)
        {
            // Close enough to attack
            m_Agent.isStopped = true;
            m_Agent.ResetPath();

            if (m_Animator != null)
            {
                m_Animator.SetBool("isAttacking", true);
                m_Animator.SetBool("isWalking", false);
            }

            // Face target while attacking
            Vector3 direction = (Target.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }

            // Attack the player
            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {
            // Too far, chase the target
            m_Agent.isStopped = false;
            
            // Set destination
            bool pathSet = m_Agent.SetDestination(Target.position);
            
            if (showDebugInfo && Time.frameCount % 60 == 0) // Log every 60 frames
            {
                Debug.Log($"=== MOVEMENT DEBUG ===");
                Debug.Log($"Distance to target: {m_Distance:F2}");
                Debug.Log($"Path set successfully: {pathSet}");
                Debug.Log($"Has path: {m_Agent.hasPath}");
                Debug.Log($"Path pending: {m_Agent.pathPending}");
                Debug.Log($"Path status: {m_Agent.pathStatus}");
                Debug.Log($"Velocity: {m_Agent.velocity.magnitude:F2}");
                Debug.Log($"Desired velocity: {m_Agent.desiredVelocity.magnitude:F2}");
                Debug.Log($"Is stopped: {m_Agent.isStopped}");
                Debug.Log($"Remaining distance: {m_Agent.remainingDistance:F2}");
            }

            if (m_Animator != null)
            {
                m_Animator.SetBool("isAttacking", false);
                m_Animator.SetBool("isWalking", true);
                
                // Drive animator speed parameter if it exists
                if (m_Animator.parameters.Length > 0)
                {
                    m_Animator.SetFloat("Speed", m_Agent.velocity.magnitude);
                }
            }
        }

        // Debug visualization
        Debug.DrawLine(transform.position, Target.position,
            m_Distance <= AttackDistance ? Color.red : Color.green);
        
        // Show velocity
        Debug.DrawRay(transform.position + Vector3.up, m_Agent.velocity, Color.blue);
        
        // Show destination
        if (m_Agent.hasPath)
        {
            Debug.DrawLine(transform.position, m_Agent.destination, Color.cyan);
        }
    }

    void AttackPlayer()
    {
        Debug.Log("Monster attacks player!");

        if (!playerIsDead)
            StartCoroutine(AttackAndTriggerGameOver());
    }

    private IEnumerator AttackAndTriggerGameOver()
    {
        playerIsDead = true;

        // Play attack animation
        if (m_Animator != null)
            m_Animator.SetBool("isAttacking", true);

        // Wait before showing game over
        yield return new WaitForSeconds(delayBeforeGameOver);

        // Trigger game over through Timer script
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
        // Only use this if root motion is enabled
        if (!useRootMotion) return;

        // Sync the agent's position with the animator's root motion
        Vector3 position = m_Animator.rootPosition;
        position.y = m_Agent.nextPosition.y; // Keep NavMesh Y position
        transform.position = position;

        // Update the NavMeshAgent so it knows where we are
        m_Agent.nextPosition = transform.position;
    }

    void OnDrawGizmosSelected()
    {
        // Show attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackDistance);

        if (Target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, Target.position);
        }

        // Show NavMesh status
        if (Application.isPlaying && m_Agent != null)
        {
            if (m_Agent.hasPath)
            {
                Gizmos.color = Color.cyan;
                Vector3[] corners = m_Agent.path.corners;
                for (int i = 0; i < corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(corners[i], corners[i + 1]);
                }
            }
        }
    }
}