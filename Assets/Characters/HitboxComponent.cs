using System;
using System.Collections.Generic;
using UnityEngine;

public class HitboxComponent : MonoBehaviour
{
    [SerializeField] private GameObject owner;
    private readonly List<HealthComponent> _hitEnemies = new();
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var targetHealth = other.GetComponent<HealthComponent>();
        if (targetHealth == null || _hitEnemies.Contains(targetHealth))
        {
            return;
        }
        
        targetHealth.TakeDamage(5.0f, owner);
        _hitEnemies.Add(targetHealth);
    }

    public void Reset()
    {
        _hitEnemies.Clear();
    }
}
