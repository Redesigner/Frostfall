using System;
using System.Collections.Generic;
using System.Linq;
using Platformer.Mechanics;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Animator))]
public class KinematicCharacterController : KinematicObject
{
    private static readonly int HorizontalBlend = Animator.StringToHash("Horizontal");
    private static readonly int VerticalBlend = Animator.StringToHash("Vertical");
    private static readonly int SpeedBlend = Animator.StringToHash("Speed");


    [SerializeField]
    private float walkSpeed = 4.0f;
    
    private bool _movementEnabled = true;

    private Animator _animator;

    private Vector2 _moveInput;

    private TimerHandle _knockbackTimer;

    protected override void OnEnable()
    {
        base.OnEnable();

        _animator = GetComponent<Animator>();
    }

    public Vector2 moveInput
    {
        set => _moveInput = value.sqrMagnitude > 1.0f ? value.normalized : value;
    }

    protected override Vector2 ComputeVelocity()
    {
        return _movementEnabled ? _moveInput * walkSpeed : velocity;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (!_movementEnabled)
        {
            return;
        }
        

        if (_moveInput.x != 0.0f || _moveInput.y != 0.0f)
        {
            _animator.SetFloat(HorizontalBlend, _moveInput.x);
            _animator.SetFloat(VerticalBlend, _moveInput.y);
            _animator.SetFloat(SpeedBlend, 1.0f);
        }
        else
        {
            _animator.SetFloat(SpeedBlend, 0.0f);
        }
    }

    public void EnableMovement()
    {
        _movementEnabled = true;
        
        // Pull our last registered input value into animator
        if (_moveInput.x != 0.0f || _moveInput.y != 0.0f)
        {
            _animator.SetFloat(HorizontalBlend, _moveInput.x);
            _animator.SetFloat(VerticalBlend, _moveInput.y);
            _animator.SetFloat(SpeedBlend, 1.0f);
        }
        else
        {
            _animator.SetFloat(SpeedBlend, 0.0f);
        }
    }

    public void DisableMovement()
    {
        _movementEnabled = false;
        velocity = Vector2.zero;
    }

    public void Knockback(Vector2 knockbackVector, float duration)
    {
        DisableMovement();
        velocity = knockbackVector;
        _knockbackTimer = TimerManager.instance.CreateTimer(this, duration, EnableMovement);
    }
}