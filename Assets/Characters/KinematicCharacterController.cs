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
    private static readonly int Pushing = Animator.StringToHash("Pushing");

    public Vector2 lookDirection
    {
        get => _lookDirection;
        private set => _lookDirection = value;
    }
    private Vector2 _lookDirection;


    [SerializeField]
    private float walkSpeed = 4.0f;
    
    private bool _movementEnabled = true;

    private Animator _animator;

    private List<(KinematicListener, RaycastHit2D)> _objectsHitThisFrame = new();

    private Vector2 _moveInput;

    private bool _isPushing = false;

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
        _objectsHitThisFrame.Clear();
        velocity = ComputeVelocity();

        if (velocity is { x: 0.0f, y: 0.0f })
        {
            if (!_isPushing)
            {
                return;
            }
            _animator.SetBool(Pushing, false);
            _isPushing = false;
            return;
        }

        if (CollideAndSlide(velocity * Time.fixedDeltaTime))
        {
            if (!_isPushing)
            {
                _animator.SetBool(Pushing, true);
                _isPushing = true;
            }
        }
        else
        {
            if (_isPushing)
            {
                _animator.SetBool(Pushing, false);
                _isPushing = false;
            }
        }

        foreach (var objectHit in _objectsHitThisFrame)
        {
            objectHit.Item1.OnHit(this, objectHit.Item2);
        }
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (!_movementEnabled || GameState.instance.paused)
        {
            return;
        }
        

        if (_moveInput.x != 0.0f || _moveInput.y != 0.0f)
        {
            _lookDirection.x = _moveInput.x;
            _lookDirection.y = _moveInput.y;
            
            _animator.SetFloat(HorizontalBlend, _moveInput.x);
            _animator.SetFloat(VerticalBlend, _moveInput.y);
            _animator.SetFloat(SpeedBlend, 1.0f);
        }
        else
        {
            _animator.SetFloat(SpeedBlend, 0.0f);
        }
    }

    protected override void OnMovementHit(RaycastHit2D hit)
    {
        Debug.DrawRay(hit.point, hit.normal, Color.blue);
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        var listener = hit.collider.GetComponent<KinematicListener>();
        if (!listener)
        {
            return;
        }
        
        if (_objectsHitThisFrame.Any(objectHit => objectHit.Item1 == listener))
        {
            return;
        }
        _objectsHitThisFrame.Add((listener, hit));
    }

    public void EnableMovement()
    {
        _movementEnabled = true;
        
        // Pull our last registered input value into animator
        if (_moveInput.x != 0.0f || _moveInput.y != 0.0f)
        {
            _lookDirection.x = _moveInput.x;
            _lookDirection.y = _moveInput.y;
            
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