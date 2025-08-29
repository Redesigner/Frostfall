using System;
using Platformer.Mechanics;
using UnityEngine;

[RequireComponent(typeof(KinematicObject))]
public class CarryableObject : MonoBehaviour
{
    [SerializeField] private GameObject sprite;
    
    [SerializeField] [Min(0.0f)] private float fallRate = 1.0f;

    private float _verticalVelocity = 0.0f;
    
    private KinematicObject _kinematicObject;

    private float _height = 0.0f;
    private bool _isGrounded = true;

    private void FixedUpdate()
    {
        if (_isGrounded)
        {
            return;
        }

        _verticalVelocity -= fallRate * Time.fixedDeltaTime;
        _height += _verticalVelocity * Time.fixedDeltaTime;

        if (_height <= 0.0f)
        {
            _isGrounded = true;
            _verticalVelocity = 0.0f;
            _kinematicObject.velocity = Vector2.zero;
        }
        sprite.transform.localPosition =  new Vector3(0.0f, _height, 0.0f);
    }

    public void Start()
    {
        _kinematicObject = GetComponent<KinematicObject>();
    }

    public void Pickup()
    {
        _kinematicObject.enabled = false;
        _height = 1.0f;
        _isGrounded = true;
        sprite.transform.localPosition = new(0.0f, _height, 0.0f);
    }

    public void Release(Vector2 velocity)
    {
        _kinematicObject.enabled = true;
        _kinematicObject.velocity = velocity;
        _height = 1.0f;
        _isGrounded = false;
    }
}
