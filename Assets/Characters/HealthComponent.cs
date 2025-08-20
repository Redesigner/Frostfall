using UnityEngine;
using UnityEngine.Events;
public class HealthComponent : MonoBehaviour
{
    [SerializeField] public float maxHealth;

    [SerializeField] private float health;

    public UnityEvent<GameObject> onTakeDamage;
    public UnityEvent onDeath;

    [field: SerializeField] [ReadOnly] public bool alive { get; private set; } = true;

    public void TakeDamage(float damage, GameObject source)
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
            onTakeDamage.Invoke(source);
            GetComponent<KinematicCharacterController>().Knockback((gameObject.transform.position - source.transform.position) * 5.0f, 0.25f);
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
