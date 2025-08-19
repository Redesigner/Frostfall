using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class AttackController : MonoBehaviour
{
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");

    private Animator _animator;

    protected void OnEnable()
    {
        _animator = GetComponent<Animator>();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        
        _animator.SetTrigger(AttackTrigger);
    }

    public void OnEnemyHit(Collider2D other)
    {
        other.GetComponent<HealthComponent>().TakeDamage(5.0f);
    }
}
