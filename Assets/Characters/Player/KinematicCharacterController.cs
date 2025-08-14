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
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");


    [SerializeField]
    private float walkSpeed = 4.0f;

    [SerializeField]
    private float jumpSpeed = 5.0f;

    private Animator _animator;

    private float _walkInput;
    public float WalkInput
    {
        set => _walkInput = Math.Clamp(value, -1.0f, 1.0f);
    }

    private Vector2 _moveInput;

    protected override void OnEnable()
    {
        base.OnEnable();

        _animator = GetComponent<Animator>();
    }

    public Vector2 MoveInput
    {
        set => _moveInput = value.sqrMagnitude > 1.0f ? value.normalized : value;
    }

    protected override Vector2 ComputeVelocity()
    {
        return _moveInput * walkSpeed;
    }

    public void OnWalkInput(InputAction.CallbackContext context)
    {
        WalkInput = context.ReadValue<float>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();

        if (_moveInput.x != 0.0f || _moveInput.y != 0.0f)
        {
            _animator.SetFloat(Horizontal, _moveInput.x);
            _animator.SetFloat(Vertical, _moveInput.y);
        }
        _animator.speed = _moveInput is { x: 0.0f, y: 0.0f } ? 0.0f : 1.0f;
        Debug.Log(_animator.GetFloat(Horizontal));
    }
}