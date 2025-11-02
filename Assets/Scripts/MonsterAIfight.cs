using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAIfight : MonoBehaviour
{
    [Header("Target & Combat")]
    public Transform Target;
    public float AttackDistance = 2f;
    public float attackCooldown = 1.5f;
    public float damagePerAttack = 1f;

    [Header("Monster Health")]
    public float monsterHealth = 6f;
    public float monsterMaxHealth = 6f;

    [Header("Game Over Settings")]
    [SerializeField] private float delayBeforeGameOver = 1f;
    [SerializeField] private float delayBeforeDeath = 0.5f;
    
    [Header("Game Win Settings")]
    [SerializeField] private float delayBeforeGameWin = 0.5f;

    [Header("Settings")]
    public bool useRootMotion = false;

    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private float m_Distance;
    private float nextAttackTime = 0f;
    private bool playerIsDead = false;
    private bool monsterIsDead = false;
    private Timer timerScript;
    private PlayerStats playerStats;

    void Start()
    {
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

        timerScript = Object.FindFirstObjectByType<Timer>();
        if (timerScript == null)
        {
            Debug.LogWarning("Timer script not found in scene!");
        }

        if (Target != null)
        {
            playerStats = Target.GetComponent<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("PlayerStats component not found on target!");
            }
        }

        if (useRootMotion)
        {
            m_Agent.updatePosition = false;
            m_Agent.updateRotation = false;
        }

        if (Target != null)
            Debug.Log($"Monster initialized. Target: {Target.name}, Health: {monsterHealth}/{monsterMaxHealth}");
    }

    void Update()
    {
        if (Target == null || playerIsDead || monsterIsDead) return;

        m_Distance = Vector3.Distance(transform.position, Target.position);

        if (m_Distance <= AttackDistance)
        {
            m_Agent.isStopped = true;

            if (m_Animator != null)
            {
                m_Animator.SetBool("isAttacking", true);
                m_Animator.SetBool("isWalking", false);
            }

            Vector3 direction = (Target.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }

            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {
            m_Agent.isStopped = false;
            m_Agent.SetDestination(Target.position);

            if (m_Animator != null)
            {
                m_Animator.SetBool("isAttacking", false);
                m_Animator.SetBool("isWalking", true);
            }
        }

        Debug.DrawLine(transform.position, Target.position,
            m_Distance <= AttackDistance ? Color.red : Color.green);
    }

    void AttackPlayer()
    {
        Debug.Log("Monster attacks player!");

        if (!playerIsDead && playerStats != null)
        {
            playerStats.TakeDamage(damagePerAttack);
            Debug.Log($"Monster dealt {damagePerAttack} damage. Player health: {playerStats.Health}");

            if (playerStats.Health <= 0)
            {
                StartCoroutine(AttackAndTriggerGameOver());
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (monsterIsDead)
        {
            Debug.Log("Monster is already dead, ignoring damage.");
            return;
        }

        monsterHealth -= damage;
        Debug.Log($"<color=red>Monster took {damage} damage! Health: {monsterHealth}/{monsterMaxHealth}</color>");

        if (monsterHealth <= 0)
        {
            monsterHealth = 0;
            Die();
        }
    }

    public bool IsMonsterDead()
    {
        return monsterIsDead;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") || other.GetComponent<SwordCollision>() != null)
        {
            SwordCollision sword = other.GetComponent<SwordCollision>();
            if (sword != null)
            {
                Debug.Log("Monster detected sword collision!");
            }
        }
    }

    private void Die()
    {
        monsterIsDead = true;
        Debug.Log("<color=yellow>========== MONSTER DEFEATED ==========</color>");

        if (m_Agent != null)
        {
            m_Agent.isStopped = true;
            m_Agent.enabled = false;
        }

        if (m_Animator != null)
        {
            m_Animator.SetBool("isAttacking", false);
            m_Animator.SetBool("isWalking", false);
            
            if (HasParameter(m_Animator, "Death"))
            {
                m_Animator.SetTrigger("Death");
            }
            else if (HasParameter(m_Animator, "IsDead"))
            {
                m_Animator.SetBool("IsDead", true);
            }
            else if (HasParameter(m_Animator, "Dead"))
            {
                m_Animator.SetBool("Dead", true);
            }
            else
            {
                Debug.LogWarning("No death animation parameter found in animator!");
            }
        }

        StartCoroutine(TriggerGameWin());
    }
    
    private bool HasParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    private IEnumerator TriggerGameWin()
    {
        yield return new WaitForSeconds(delayBeforeGameWin);

        Time.timeScale = 0f;
        Debug.Log("<color=green>Game PAUSED! TimeScale set to 0</color>");

        if (timerScript != null)
        {
            Debug.Log("<color=green>Calling GameWin()!</color>");
            timerScript.GameWin();
        }
        else
        {
            Debug.LogError("Cannot trigger Game Win - Timer script not found!");
        }
    }

    private IEnumerator AttackAndTriggerGameOver()
    {
        playerIsDead = true;

        if (m_Animator != null)
            m_Animator.SetBool("isAttacking", true);

        if (Target != null)
        {
            Animator playerAnimator = Target.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                if (HasParameter(playerAnimator, "Death"))
                {
                    playerAnimator.SetTrigger("Death");
                    Debug.Log("Player death animation triggered!");
                }
                else if (HasParameter(playerAnimator, "IsDead"))
                {
                    playerAnimator.SetBool("IsDead", true);
                    Debug.Log("Player IsDead set to true!");
                }
                else if (HasParameter(playerAnimator, "Dead"))
                {
                    playerAnimator.SetBool("Dead", true);
                    Debug.Log("Player Dead set to true!");
                }
                else
                {
                    Debug.LogWarning("No death parameter found in player animator!");
                }
            }
        }

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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackDistance);

        if (Target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, Target.position);
        }
    }
}