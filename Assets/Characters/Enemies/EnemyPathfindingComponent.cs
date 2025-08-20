using System;
using Platformer.Mechanics;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(KinematicCharacterController))]
public class EnemyPathfindingComponent : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private KinematicCharacterController _kinematicObject;
    
    [SerializeField]
    private GameObject target;
    
    private void OnEnable()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _kinematicObject = GetComponent<KinematicCharacterController>();
    }

    public void Start()
    {
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _navMeshAgent.destination = Vector3.zero;
        _navMeshAgent.updatePosition = false;
        // _navMeshAgent.isStopped = true;
    }

    public void FixedUpdate()
    {
        if (!target)
        {
            return;
        }

        _navMeshAgent.nextPosition = _kinematicObject.transform.position;
        _navMeshAgent.destination = target.transform.position;
        _kinematicObject.moveInput = _navMeshAgent.desiredVelocity;
        Debug.DrawRay(_kinematicObject.gameObject.transform.position, _navMeshAgent.desiredVelocity, Color.red, Time.fixedDeltaTime);
    }
}
