using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;

namespace Platformer.Mechanics
{
    public class KinematicMoveResult
    {
        public float distanceMoved;
        public readonly List<RaycastHit2D> hits = new ();

        public bool Collided()
        {
            return hits.Count > 0;
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

        private Rigidbody2D _body;
        private ContactFilter2D _contactFilter;

        [SerializeField]
        private float minMoveDistance = 0.001f;
        
        [SerializeField]
        private float shellRadius = 0.01f;

        private const int MaxMoveCount = 5;

        /// <summary>
        /// Teleport to some position.
        /// </summary>
        /// <param name="position"></param>
        public void Teleport(Vector3 position)
        {
            _body.position = position;
            velocity *= 0;
            _body.linearVelocity *= 0;
        }

        protected virtual void OnEnable()
        {
            _body = GetComponent<Rigidbody2D>();
            _body.bodyType = RigidbodyType2D.Kinematic;
        }

        protected virtual void Start()
        {
            _contactFilter.useTriggers = false;
            _contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            _contactFilter.useLayerMask = true;
        }
        
        [Pure]
        protected virtual Vector2 ComputeVelocity()
        {
            return velocity;
        }

        protected virtual void FixedUpdate()
        {
            velocity = ComputeVelocity();

            if (velocity is { x: 0.0f, y: 0.0f })
            {
                return;
            }
            
            CollideAndSlide(velocity * Time.fixedDeltaTime);
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
            var count = _body.Cast(move.normalized, _contactFilter, collisionResult.hits, sweepDistance);
            if (count == 0)
            {
                collisionResult.distanceMoved = moveDistance;
                //Body.MovePosition(Body.position + move);
                _body.position += move;
                return collisionResult;
            }
            
            collisionResult.hits.Sort((hitA, hitB) => Mathf.Approximately(hitA.distance, hitB.distance) ? 0 : (hitA.distance < hitB.distance ? -1 : 1));
            // Sort by distance, ascending
            var closestHit = collisionResult.hits.First();

            var modifiedShellRadius = shellRadius / Vector2.Dot(moveDirection, -closestHit.normal);
            if (closestHit.distance <= modifiedShellRadius)
            {
                // Debug.DrawRay(Body.position, new Vector2(0.0f, 1.0f), Color.red);
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                OnMovementHit(collisionResult.hits.First());
                return collisionResult;
            }

            if (closestHit.distance - shellRadius > moveDistance)
            {
                collisionResult.distanceMoved = moveDistance;
                // Body.MovePosition(Body.position + move);
                _body.position += move;
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                OnMovementHit(collisionResult.hits.First());
                return collisionResult;
            }
            
            _body.position += moveDirection * (closestHit.distance - shellRadius);
            OnMovementHit(collisionResult.hits.First());
            return collisionResult;
        }

        protected virtual void OnMovementHit(RaycastHit2D hit)
        {
        }

        protected bool CollideAndSlide(Vector2 movement)
        {
            if (movement.sqrMagnitude < minMoveDistance * minMoveDistance)
            {
                return false;
            }
            
            var movementThisStep = movement;
            var movementLength = movement.magnitude;
            var collidedThisMove = false;
            
            for (var moveCount = 0; moveCount < MaxMoveCount; ++moveCount)
            {
                var movementResult = PerformMovement(movementThisStep);

                if (movementResult.Collided())
                {
                    collidedThisMove = true;
                }
                else
                {
                    return collidedThisMove;
                }

                movementLength -= movementResult.distanceMoved;
                if (movementLength <= 0.0f)
                {
                    return true;
                }

                var collisionNormal = movementResult.hits.First().normal;
                var moveDirection = movementThisStep.normalized;
                var remainingMovement = moveDirection * movementLength;
                remainingMovement -= Vector2.Dot(movementThisStep, collisionNormal) * collisionNormal;
                movementLength = remainingMovement.magnitude;
                
                var planeDirection = (movementThisStep - Vector2.Dot(movementThisStep, collisionNormal) * collisionNormal).normalized;
                var slideMovement = planeDirection * movementLength;
                movementThisStep = slideMovement;
            }

            return true;
        }

        protected void ResolvePenetrations()
        {
            var colliders = new List<Collider2D>();
            var overlappingColliders = new List<Collider2D>();
            _body.GetAttachedColliders(colliders);

            if (colliders.Count < 0)
            {
                return;
            }
            
            var mainCollider = colliders.First();
            if (mainCollider.Overlap(overlappingColliders) <= 0)
            {
                return;
            }
            var delta = mainCollider.Distance(overlappingColliders.First());
            var resolutionDelta = delta.normal * delta.distance;
            _body.position += resolutionDelta;
            Debug.DrawRay(_body.position, resolutionDelta * 5.0f, Color.magenta, 2.0f);
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
            _body.Cast(Vector2.down, _contactFilter, hits, distance);
            if (hits.Count == 0)
            {
                return false;
            }

            if (hits.First().distance <= 0.0f)
            {
                return false;
            }

            _body.position = hits.First().centroid + new Vector2(0.0f, shellRadius);
            return true;
        }
    }
}