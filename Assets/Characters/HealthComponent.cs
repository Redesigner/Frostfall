using UnityEngine;
using UnityEngine.Events;
public class HealthComponent : MonoBehaviour
{
    [SerializeField] public float maxHealth;

    [SerializeField] private float health;

    public UnityEvent onTakeDamage;
    public UnityEvent onDeath;

    [field: SerializeField] [ReadOnly] public bool alive { get; private set; } = true;

    public void TakeDamage(float damage)
    {
        if (damage < 0.0f)
        {
            return;
        }

        if (!alive)
        {
            return;
        }

        health -= damage;
        if (health > 0.0f)
        {
            onTakeDamage.Invoke();
            return;
        }
        
        health = 0.0f;
        alive = false;
        onDeath.Invoke();
    }
    
    public void Heal(float healing)
    {
        if (!alive)
        {
            return;
        }

        if (healing < 0.0f)
        {
            return;
        }

        health += healing;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
}
