using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 2f;
    public float attackCooldown = 0.5f;
    public float attackRange = 3f;
    public LayerMask monsterLayer;

    [Header("Combat Detection")]
    public float combatDetectionRange = 5f;
    public LayerMask enemyLayer;

    [Header("Visual Feedback")]
    public GameObject attackEffect;

    [Header("Mobile Support")]
    public MobileInputManager mobileInput;

    [Header("Debug")]
    public bool showDebugLogs = true;

    private float nextAttackTime = 0f;
    private Animator animator;
    private bool isInCombatStance = false;
    private bool isNearEnemy = false;
    private MonsterAIfight nearestMonster = null;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on player!");
        }

        if (mobileInput == null)
        {
            mobileInput = Object.FindFirstObjectByType<MobileInputManager>();
            if (mobileInput != null)
                Debug.Log("MobileInputManager found automatically!");
        }
    }

    void Update()
    {
        // Check for nearby enemies
        CheckNearbyEnemies();
        UpdateCombatStance();

        // Detect attack input (mouse or mobile)
        if (GetAttackInput())
        {
            if (Time.time >= nextAttackTime)
            {
                PerformAttack(); // play animation + damage
                nextAttackTime = Time.time + attackCooldown;
            }
            else if (showDebugLogs)
            {
                Debug.Log("Attack on cooldown!");
            }
        }
    }

    bool GetAttackInput()
    {
        bool input = false;

        // Mobile button
        if (mobileInput != null && mobileInput.IsAttackPressed())
        {
            input = true;
            if (showDebugLogs) Debug.Log("Mobile attack button pressed!");
        }

#if ENABLE_INPUT_SYSTEM
        // PC - Left click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            input = true;
            if (showDebugLogs) Debug.Log("Mouse click attack detected!");
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            input = true;
            if (showDebugLogs) Debug.Log("Mouse click attack detected!");
        }
#endif
        return input;
    }

    void CheckNearbyEnemies()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, combatDetectionRange, enemyLayer);
        isNearEnemy = enemies.Length > 0;

        nearestMonster = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider enemy in enemies)
        {
            MonsterAIfight monster = enemy.GetComponent<MonsterAIfight>();
            if (monster != null && !monster.IsMonsterDead())
            {
                float distance = Vector3.Distance(transform.position, monster.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestMonster = monster;
                }
            }
        }
    }

    void UpdateCombatStance()
    {
        if (isNearEnemy && !isInCombatStance)
        {
            isInCombatStance = true;
            if (animator != null) animator.SetBool("Combat", true);
        }
        else if (!isNearEnemy && isInCombatStance)
        {
            isInCombatStance = false;
            if (animator != null) animator.SetBool("Combat", false);
        }
    }

    void PerformAttack()
    {
        Debug.Log("========== ATTACK STARTED ==========");

        // 🔹 1. Trigger Attack Animation
        if (animator != null)
        {
            animator.SetTrigger("Attack 01");
            if (showDebugLogs) Debug.Log("Attack animation triggered!");
        }

        // 🔹 2. Detect Monsters and Apply Damage
        Collider[] hitMonsters = Physics.OverlapSphere(transform.position, attackRange, monsterLayer);
        if (hitMonsters.Length > 0)
        {
            foreach (Collider col in hitMonsters)
            {
                MonsterAIfight monster = col.GetComponent<MonsterAIfight>();
                if (monster != null && !monster.IsMonsterDead())
                {
                    monster.TakeDamage(attackDamage);
                    if (showDebugLogs)
                        Debug.Log($"<color=green>Damaged {monster.name} for {attackDamage}!</color>");

                    if (attackEffect != null)
                        Instantiate(attackEffect, monster.transform.position + Vector3.up, Quaternion.identity);
                }
            }
        }
        else if (nearestMonster != null)
        {
            float dist = Vector3.Distance(transform.position, nearestMonster.transform.position);
            if (dist <= attackRange)
            {
                nearestMonster.TakeDamage(attackDamage);
                if (attackEffect != null)
                    Instantiate(attackEffect, nearestMonster.transform.position + Vector3.up, Quaternion.identity);
            }
        }
        else
        {
            if (showDebugLogs) Debug.Log("No monsters found to attack!");
        }

        Debug.Log("========== ATTACK ENDED ==========");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, combatDetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
