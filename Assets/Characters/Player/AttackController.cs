using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class AttackController : MonoBehaviour
{
    [SerializeField] private HitboxComponent hitboxComponent;
    
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
        
        hitboxComponent.Reset();
        _animator.SetTrigger(AttackTrigger);
    }
}
