using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Invincibility Settings")]
    public bool hasInvincibility = true;
    public float invincibilityDuration = 1f;
    private bool isInvincible = false;

    [Header("Animation")]
    public Animator animator;
    public string damageAnimParam = "Damage";
    public string deathAnimParam = "Death";
    [Header("UI (Debug)")]
    public TextMeshProUGUI healthText;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    public void TakeDamage(float amount)
    {
        if (isInvincible || isDead) return;

        currentHealth -= amount;
        //================================
        //ANIMATIONS (disabled for now)

        //if (animator != null && !string.IsNullOrEmpty(damageAnimParam))
        //animator.SetTrigger(damageAnimParam);


        //if (currentHealth <= 0)
        //{
        //  isDead = true;
        //   StartCoroutine(HandleDeathAfterDamage());
        //}
        //else if (hasInvincibility)
        //================================ 
        StartCoroutine(InvincibilityFrames());
        
        if (healthText != null)
            healthText.text = "HP: " + Mathf.RoundToInt(currentHealth).ToString();
    }

    private System.Collections.IEnumerator HandleDeathAfterDamage()
    {
        
        float damageAnimTime = 0.4f;

        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(damageAnimParam))
                damageAnimTime = stateInfo.length;
        }

        yield return new WaitForSeconds(damageAnimTime);
        Die();
    }

    private System.Collections.IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float elapsed = 0f;
            while (elapsed < invincibilityDuration)
            {
                sr.enabled = !sr.enabled;
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }
            sr.enabled = true;
        }

        isInvincible = false;
    }

    void Die()
    {
        if (animator != null && !string.IsNullOrEmpty(deathAnimParam))
            animator.SetTrigger(deathAnimParam);

        
        Destroy(gameObject, 1f);
    }
}
