using System;
using UnityEngine;

public class HitboxComponent : MonoBehaviour
{
    [SerializeField] private GameObject owner;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var targetHealth = other.GetComponent<HealthComponent>();
        if (targetHealth == null)
        {
            return;
        }
        
        targetHealth.TakeDamage(5.0f, owner);
    }
}
