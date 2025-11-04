using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 2f;
    public float attackCooldown = 0.5f;
    public LayerMask monsterLayer;
    
    [Header("Combat Detection")]
    public float combatDetectionRange = 5f;
    public LayerMask enemyLayer;
    
    [Header("Interaction Settings")]
    public float interactionRange = 3f;
    public LayerMask interactableLayer;
    
    [Header("Visual Feedback")]
    public GameObject attackEffect;
    
    [Header("Mobile Support")]
    public MobileInputManager mobileInput;
    
    private float nextAttackTime = 0f;
    private Camera mainCamera;
    private Animator animator;
    private bool isInCombatStance = false;
    private bool isNearEnemy = false;
    private MonsterAIfight nearestMonster = null;
    
    void Start()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        
        if (mobileInput == null)
        {
            mobileInput = Object.FindFirstObjectByType<MobileInputManager>();
            if (mobileInput != null)
            {
                Debug.Log("Found MobileInputManager!");
            }
            else
            {
                Debug.LogWarning("MobileInputManager not found!");
            }
        }
        
        if (animator == null)
        {
            Debug.LogError("Animator component not found on player!");
        }
        else
        {
            Debug.Log("Animator found on player!");
        }
    }
    
    void Update()
    {
        CheckNearbyEnemies();
        UpdateCombatStance();
        
        bool attackInput = GetAttackInput();
        
        if (attackInput)
        {
            Debug.Log($"Attack input detected!");
            
            // Check if not in combat stance first
            if (!isInCombatStance)
            {
                Debug.Log("Cannot attack - not in combat mode!");
                return;
            }
            
            // Check for interaction (prioritize interaction over attack)
            if (TryInteract())
            {
                Debug.Log("Interaction triggered instead of attack");
                return;
            }
            
            // Check cooldown AFTER triggering animation for responsive feel
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
            else
            {
                Debug.Log("Attack on cooldown - animation will play when ready!");
                // Optionally play a "blocked" animation or sound here
            }
        }
        
        if (mobileInput != null && mobileInput.IsInteractPressed())
        {
            TryInteract();
        }
    }
    
    bool GetAttackInput()
    {
        bool input = false;
        
        if (mobileInput != null)
        {
            input = mobileInput.IsAttackPressed();
            if (input)
            {
                Debug.Log("Mobile attack button detected!");
            }
        }
        
        #if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Mouse left click detected!");
            input = true;
        }
        #else
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse button 0 detected!");
            input = true;
        }
        #endif
        
        return input;
    }
    
    void CheckNearbyEnemies()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, combatDetectionRange, enemyLayer);
        
        bool wasNearEnemy = isNearEnemy;
        isNearEnemy = enemies.Length > 0;
        
        if (isNearEnemy)
        {
            float closestDistance = Mathf.Infinity;
            nearestMonster = null;
            
            foreach (Collider enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    MonsterAIfight monster = enemy.GetComponent<MonsterAIfight>();
                    if (monster != null && !monster.IsMonsterDead())
                    {
                        nearestMonster = monster;
                        closestDistance = distance;
                    }
                }
            }
        }
        else
        {
            nearestMonster = null;
        }
        
        if (isNearEnemy && !wasNearEnemy)
        {
            Debug.Log("Entered combat zone!");
        }
        else if (!isNearEnemy && wasNearEnemy)
        {
            Debug.Log("Exited combat zone!");
        }
    }
    
    void UpdateCombatStance()
    {
        if (isNearEnemy && !isInCombatStance)
        {
            isInCombatStance = true;
            if (animator != null)
            {
                animator.SetBool("Combat", true);
                Debug.Log("Combat stance ENABLED");
            }
        }
        else if (!isNearEnemy && isInCombatStance)
        {
            isInCombatStance = false;
            if (animator != null)
            {
                animator.SetBool("Combat", false);
                Debug.Log("Combat stance DISABLED");
            }
        }
    }
    
    void Attack()
    {
        Debug.Log("========== ATTACK STARTED ==========");
        
        // Trigger animation immediately on button press
        if (animator != null)
        {
            animator.SetTrigger("Attack 01");
            Debug.Log("Attack 01 trigger SET - Animation playing NOW!");
        }
        
        if (nearestMonster != null)
        {
            float distanceToMonster = Vector3.Distance(transform.position, nearestMonster.transform.position);
            Debug.Log($"Attacking {nearestMonster.name} at distance: {distanceToMonster}");
            
            nearestMonster.TakeDamage(attackDamage);
            Debug.Log($"<color=green>DAMAGED {nearestMonster.name} for {attackDamage} damage!</color>");
            
            if (attackEffect != null)
            {
                Vector3 effectPosition = nearestMonster.transform.position + Vector3.up;
                Instantiate(attackEffect, effectPosition, Quaternion.identity);
            }
        }
        else
        {
            Debug.Log("<color=red>No monster to attack!</color>");
        }
        
        Debug.Log("========== ATTACK ENDED ==========");
    }
    
    bool TryInteract()
    {
        Collider[] interactables = Physics.OverlapSphere(transform.position, interactionRange, interactableLayer);
        
        if (interactables.Length > 0)
        {
            Collider closest = null;
            float closestDistance = Mathf.Infinity;
            
            foreach (Collider col in interactables)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = col;
                }
            }
            
            if (closest != null)
            {
                IInteractable interactable = closest.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    Debug.Log("Interacted with: " + closest.name);
                    return true;
                }
                
                closest.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
                Debug.Log("Sent interact message to: " + closest.name);
                return true;
            }
        }
        
        return false;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, combatDetectionRange);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        
        if (nearestMonster != null && Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, nearestMonster.transform.position);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(nearestMonster.transform.position, 0.5f);
        }
    }
}

public interface IInteractable
{
    void Interact();
}