/*
 *  Author: ariel oliveira [o.arielg@gmail.com]
 *  Modified: Added combat system integration
 */

using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public delegate void OnHealthChangedDelegate();
    public OnHealthChangedDelegate onHealthChangedCallback;
    
    public delegate void OnPlayerDeathDelegate();
    public OnPlayerDeathDelegate onPlayerDeathCallback;

    #region Singleton
    private static PlayerStats instance;
    public static PlayerStats Instance
    {
        get
        {
            if (instance == null)
                instance = Object.FindFirstObjectByType<PlayerStats>();
            return instance;
        }
    }
    #endregion

    [Header("Health Settings")]
    [SerializeField]
    private float health = 10f;
    [SerializeField]
    private float maxHealth = 10f;
    [SerializeField]
    private float maxTotalHealth = 20f;
    
    [Header("Combat Settings")]
    [SerializeField]
    private bool invulnerable = false;
    [SerializeField]
    private float invulnerabilityDuration = 0.5f;
    
    [Header("Debug")]
    [SerializeField]
    private bool showDebugLogs = true;

    private bool isDead = false;
    private float lastDamageTime = 0f;
    private Animator animator;

    public float Health { get { return health; } }
    public float MaxHealth { get { return maxHealth; } }
    public float MaxTotalHealth { get { return maxTotalHealth; } }
    public bool IsDead { get { return isDead; } }
    public bool IsInvulnerable { get { return invulnerable && Time.time - lastDamageTime < invulnerabilityDuration; } }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Initialize health
        if (health <= 0)
        {
            health = maxHealth;
        }
        
        if (showDebugLogs)
        {
            Debug.Log(string.Format("[PlayerStats] Initialized - Health: {0}/{1}, Max Total: {2}", health, maxHealth, maxTotalHealth));
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;
        
        float oldHealth = health;
        health += healAmount;
        ClampHealth();
        
        if (showDebugLogs)
        {
            Debug.Log(string.Format("<color=green>[PlayerStats] Healed {0}! Health: {1} -> {2}</color>", healAmount, oldHealth, health));
        }
    }

    public void TakeDamage(float dmg)
    {
        // Prevent damage if dead or invulnerable
        if (isDead)
        {
            if (showDebugLogs) Debug.Log("[PlayerStats] Already dead, ignoring damage.");
            return;
        }
        
        if (IsInvulnerable)
        {
            if (showDebugLogs) Debug.Log("[PlayerStats] Invulnerable! Damage blocked.");
            return;
        }
        
        float oldHealth = health;
        health -= dmg;
        lastDamageTime = Time.time;
        
        Debug.Log(string.Format("<color=red>[PlayerStats] X TOOK {0} DAMAGE! Health: {1} -> {2}/{3}</color>", dmg, oldHealth, health, maxHealth));
        
        ClampHealth();
        
        // Play hit animation if available
        if (animator != null && !isDead)
        {
            if (HasParameter(animator, "Hit"))
            {
                animator.SetTrigger("Hit");
            }
        }
        
        // Check for death
        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    public void AddHealth()
    {
        if (maxHealth < maxTotalHealth)
        {
            maxHealth += 1;
            health = maxHealth;

            if (showDebugLogs)
            {
                Debug.Log(string.Format("<color=cyan>[PlayerStats] Max health increased! New max: {0}</color>", maxHealth));
            }

            if (onHealthChangedCallback != null)
                onHealthChangedCallback.Invoke();
        }
        else
        {
            if (showDebugLogs)
            {
                Debug.Log(string.Format("[PlayerStats] Already at max total health ({0})", maxTotalHealth));
            }
        }
    }

    private void Die()
    {
        isDead = true;
        health = 0;
        
        Debug.Log("<color=yellow>========== PLAYER DIED ==========</color>");
        
        // Trigger death animation
        if (animator != null)
        {
            if (HasParameter(animator, "Death"))
            {
                animator.SetTrigger("Death");
                Debug.Log("[PlayerStats] Death animation triggered");
            }
            else if (HasParameter(animator, "IsDead"))
            {
                animator.SetBool("IsDead", true);
                Debug.Log("[PlayerStats] IsDead set to true");
            }
            else if (HasParameter(animator, "Dead"))
            {
                animator.SetBool("Dead", true);
                Debug.Log("[PlayerStats] Dead set to true");
            }
        }
        
        // Disable player controls
        PlayerStats movement = GetComponent<PlayerStats>();
        if (movement != null)
        {
            movement.enabled = false;
        }
        
        PlayerAttack attack = GetComponent<PlayerAttack>();
        if (attack != null)
        {
            attack.enabled = false;
        }
        
        // Invoke death callback
        if (onPlayerDeathCallback != null)
        {
            onPlayerDeathCallback.Invoke();
        }
        
        if (onHealthChangedCallback != null)
        {
            onHealthChangedCallback.Invoke();
        }
    }

    public void Revive()
    {
        isDead = false;
        health = maxHealth;
        
        Debug.Log("<color=green>[PlayerStats] Player revived!</color>");
        
        // Re-enable components
        PlayerStats movement = GetComponent<PlayerStats>();
        if (movement != null)
        {
            movement.enabled = true;
        }
        
        PlayerAttack attack = GetComponent<PlayerAttack>();
        if (attack != null)
        {
            attack.enabled = true;
        }
        
        // Reset animator
        if (animator != null)
        {
            if (HasParameter(animator, "IsDead"))
            {
                animator.SetBool("IsDead", false);
            }
            else if (HasParameter(animator, "Dead"))
            {
                animator.SetBool("Dead", false);
            }
        }
        
        if (onHealthChangedCallback != null)
        {
            onHealthChangedCallback.Invoke();
        }
    }

    void ClampHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth);

        if (onHealthChangedCallback != null)
            onHealthChangedCallback.Invoke();
    }
    
    private bool HasParameter(Animator anim, string paramName)
    {
        if (anim == null) return false;
        
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
    
    // Public method to set invulnerability (useful for power-ups or testing)
    public void SetInvulnerable(bool value)
    {
        invulnerable = value;
        if (showDebugLogs)
        {
            Debug.Log(string.Format("[PlayerStats] Invulnerability {0}", value ? "ENABLED" : "DISABLED"));
        }
    }
    
    // Reset health to max (useful for checkpoints or respawn)
    public void ResetHealth()
    {
        health = maxHealth;
        isDead = false;
        ClampHealth();
        
        if (showDebugLogs)
        {
            Debug.Log(string.Format("[PlayerStats] Health reset to {0}", maxHealth));
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        
        // Draw health bar above player
        Vector3 healthBarPos = transform.position + Vector3.up * 2.5f;
        float healthPercent = health / maxHealth;
        
        // Background (red)
        Gizmos.color = Color.red;
        Gizmos.DrawLine(healthBarPos - Vector3.right * 0.5f, healthBarPos + Vector3.right * 0.5f);
        
        // Health (green)
        Gizmos.color = Color.green;
        Gizmos.DrawLine(healthBarPos - Vector3.right * 0.5f, 
                       healthBarPos + Vector3.right * (healthPercent - 0.5f));
        
        // Invulnerability indicator
        if (IsInvulnerable)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}