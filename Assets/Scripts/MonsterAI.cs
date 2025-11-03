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

    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private float m_Distance;
    private float nextAttackTime = 0f;
    private bool playerIsDead = false;
    private Timer timerScript;

    void Start()
    {
        // Try to snap monster to NavMesh if not close enough
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            Debug.Log("Monster snapped to NavMesh at: " + hit.position);
        }
        else
        {
            Debug.LogError("Monster is too far from NavMesh! Move it closer to the blue area.");
        }

        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        // Configure NavMeshAgent for movement
        if (!useRootMotion)
        {
            m_Agent.speed = moveSpeed;
            m_Agent.angularSpeed = rotationSpeed;
            m_Agent.acceleration = 8f;
            m_Agent.updatePosition = true;
            m_Agent.updateRotation = true;
        }
        else
        {
            // Root motion settings
            m_Agent.updatePosition = false;
            m_Agent.updateRotation = false;
            m_Agent.speed = moveSpeed;
        }

        // Ensure the agent is active
        m_Agent.enabled = true;
        m_Agent.isStopped = false;

        // Find the Timer script in the scene
        timerScript = Object.FindFirstObjectByType<Timer>();
        if (timerScript == null)
        {
            Debug.LogWarning("Timer script not found in scene!");
        }

        if (Target != null)
        {
            Debug.Log("Monster initialized. Target: " + Target.name);
        }
        else
        {
            Debug.LogError("Monster has no target assigned!");
        }
    }

    void Update()
    {
        if (Target == null || playerIsDead) return;

        m_Distance = Vector3.Distance(transform.position, Target.position);

        if (m_Distance <= AttackDistance)
        {
            // Close enough to attack
            m_Agent.isStopped = true;
            m_Agent.ResetPath(); // Clear the path

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
            m_Agent.SetDestination(Target.position);

            if (m_Animator != null)
            {
                m_Animator.SetBool("isAttacking", false);
                m_Animator.SetBool("isWalking", true);
            }
        }

        // Debug visualization
        Debug.DrawLine(transform.position, Target.position,
            m_Distance <= AttackDistance ? Color.red : Color.green);
        
        // Debug agent state
        if (m_Agent.enabled)
        {
            Debug.DrawRay(transform.position, m_Agent.velocity, Color.blue);
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
    }
}