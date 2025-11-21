using UnityEngine;

public abstract class HealthComponent : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    /// <summary>
    /// Override this in child classes to define custom death behavior.
    /// </summary>
    protected abstract void Die();
}
