using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;

namespace Platformer.Mechanics
{
    public class KinematicMoveResult
    {
        public float DistanceMoved = 0.0f;
        public float RequestedDistance = 0.0f;
        public readonly List<RaycastHit2D> Hits = new ();

        public bool Collided()
        {
            return Hits.Count > 0;
        }
    }
    
    /// <summary>
    /// Implements game physics for some in game entity.
    /// </summary>
    public class KinematicObject : MonoBehaviour
    {
        /// <summary>
        /// The minimum normal (dot product) considered suitable for the entity sit on.
        /// </summary>
        public float minGroundNormalY = .65f;

        /// <summary>
        /// A custom gravity coefficient applied to this entity.
        /// </summary>
        public float gravityModifier = 1f;

        /// <summary>
        /// The current velocity of the entity.
        /// </summary>
        [SerializeField]
        public Vector2 velocity;

        /// <summary>
        /// Is the entity currently sitting on a surface?
        /// </summary>
        /// <value></value>
        [SerializeField]
        public bool isGrounded = false;
        
        protected Rigidbody2D Body;
        protected ContactFilter2D ContactFilter;

        [SerializeField]
        private float minMoveDistance = 0.001f;
        
        [SerializeField]
        private float shellRadius = 0.01f;

        private int _maxMoveCount = 5;

        private Vector2 floorNormal;

        /// <summary>
        /// Teleport to some position.
        /// </summary>
        /// <param name="position"></param>
        public void Teleport(Vector3 position)
        {
            Body.position = position;
            velocity *= 0;
            Body.linearVelocity *= 0;
        }

        protected virtual void OnEnable()
        {
            Body = GetComponent<Rigidbody2D>();
            Body.bodyType = RigidbodyType2D.Kinematic;
        }

        protected virtual void OnDisable()
        {
            Body.bodyType = RigidbodyType2D.Dynamic;
        }

        protected virtual void Start()
        {
            ContactFilter.useTriggers = false;
            ContactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            ContactFilter.useLayerMask = true;
        }
        
        [Pure]
        protected virtual Vector2 ComputeVelocity()
        {
            return velocity;
        }

        protected virtual void FixedUpdate()
        {
            var initialPosition = Body.position;
            velocity = ComputeVelocity();

            if (velocity is { x: 0.0f, y: 0.0f })
            {
                return;
            }
            
            if (!isGrounded)
            {
                // velocity += Physics2D.gravity * Time.fixedDeltaTime;
            }
            
            CollideAndSlide(velocity * Time.fixedDeltaTime);
            var deltaPosition = Body.position - initialPosition;
        }

        protected KinematicMoveResult PerformMovement(Vector2 move)
        {
            var moveDistance = move.magnitude;
            if (moveDistance < minMoveDistance)
            {
                return new KinematicMoveResult();
            }
            
            var moveDirection = move.normalized;
            var sweepDistance = (moveDistance + shellRadius) * 2.0f;

            var collisionResult = new KinematicMoveResult();
            var count = Body.Cast(move.normalized, ContactFilter, collisionResult.Hits, sweepDistance);
            if (count == 0)
            {
                collisionResult.RequestedDistance = moveDistance;
                collisionResult.DistanceMoved = moveDistance;
                //Body.MovePosition(Body.position + move);
                Body.position += move;
                return collisionResult;
            }
            
            collisionResult.Hits.Sort((hitA, hitB) => Mathf.Approximately(hitA.distance, hitB.distance) ? 0 : (hitA.distance < hitB.distance ? -1 : 1));
            // Sort by distance, ascending
            var closestHit = collisionResult.Hits.First();

            var modifiedShellRadius = shellRadius / Vector2.Dot(moveDirection, -closestHit.normal);
            if (closestHit.distance <= modifiedShellRadius)
            {
                // Debug.DrawRay(Body.position, new Vector2(0.0f, 1.0f), Color.red);
                return collisionResult;
            }

            if (closestHit.distance - shellRadius > moveDistance)
            {
                collisionResult.DistanceMoved = moveDistance;
                // Body.MovePosition(Body.position + move);
                Body.position += move;
                return collisionResult;
            }
            
            // Body.MovePosition(Body.position + moveDirection * (closestHit.distance - shellRadius));
            Body.position += moveDirection * (closestHit.distance - shellRadius);
            
            // Debug.DrawRay(closestHit.point, moveDirection * -closestHit.distance, Color.cyan);
            // Debug.DrawRay(closestHit.point, new Vector2(closestHit.normal.y, closestHit.normal.x) * 0.1f, Color.red);
            return collisionResult;
        }

        protected void CollideAndSlide(Vector2 movement)
        {
            if (movement.sqrMagnitude < minMoveDistance * minMoveDistance)
            {
                return;
            }

            var startPosition = Body.position;
            
            var movementThisStep = movement;
            var movementLength = movement.magnitude;
            
            for (var moveCount = 0; moveCount < _maxMoveCount; ++moveCount)
            {
                var movementResult = PerformMovement(movementThisStep);
                
                if (!movementResult.Collided())
                {
                    return;
                }

                movementLength -= movementResult.DistanceMoved;
                if (movementLength <= 0.0f)
                {
                    return;
                }

                var collisionNormal = movementResult.Hits.First().normal;
                var moveDirection = movementThisStep.normalized;
                var remainingMovement = moveDirection * movementLength;
                remainingMovement -= Vector2.Dot(movementThisStep, collisionNormal) * collisionNormal;
                movementLength = remainingMovement.magnitude;
                
                var planeDirection = (movementThisStep - Vector2.Dot(movementThisStep, collisionNormal) * collisionNormal).normalized;
                var slideMovement = planeDirection * movementLength;
                movementThisStep = slideMovement;
            }
        }

        protected void ResolvePenetrations()
        {
            var colliders = new List<Collider2D>();
            var overlappingColliders = new List<Collider2D>();
            Body.GetAttachedColliders(colliders);

            if (colliders.Count >= 0)
            {
                var collider = colliders.First();
                if (collider.Overlap(overlappingColliders) <= 0)
                {
                    return;
                }
                var delta = collider.Distance(overlappingColliders.First());
                var resolutionDelta = delta.normal * delta.distance;
                Body.position += resolutionDelta;
                Debug.DrawRay(Body.position, resolutionDelta * 5.0f, Color.magenta, 2.0f);
            }
        }

        
        /**
         * <summary>
         * Sweep to the floor and snap if a floor is within range
         * </summary>
         * <returns>
         * true if floor was hit
         * </returns>
         */
        protected bool SnapToFloor()
        {
            if (velocity.y > 0.0f)
            {
                return false;
            }
            
            var hits = new List<RaycastHit2D>();
            var distance = velocity.y * Time.fixedDeltaTime + shellRadius + 0.1f;
            Body.Cast(Vector2.down, ContactFilter, hits, distance);
            if (hits.Count == 0)
            {
                return false;
            }

            if (hits.First().distance <= 0.0f)
            {
                return false;
            }

            Body.position = hits.First().centroid + new Vector2(0.0f, shellRadius);
            return true;
        }
    }
}