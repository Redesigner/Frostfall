using System;
using Platformer.Mechanics;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(KinematicObject))]
public class EnemyPathfindingComponent : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private KinematicObject _kinematicObject;
    
    [SerializeField]
    private GameObject target;
    
    private void OnEnable()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _kinematicObject = GetComponent<KinematicObject>();
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
        _kinematicObject.velocity = _navMeshAgent.desiredVelocity;
        Debug.DrawRay(_kinematicObject.gameObject.transform.position, _navMeshAgent.desiredVelocity, Color.red, Time.fixedDeltaTime);
    }
}
