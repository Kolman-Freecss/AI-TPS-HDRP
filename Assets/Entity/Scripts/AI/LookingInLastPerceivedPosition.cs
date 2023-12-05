#region

using Entity.Scripts.AI;
using UnityEngine;
using UnityEngine.Events;

#endregion

public class LookingInLastPerceivedPosition : AIState
{
    [SerializeField] public UnityEvent onLastPerceivedPositionReached;

    [SerializeField] public Vector3 lastPerceivedPosition;

    [SerializeField] private float reachingDistance = 1f;

    private void Update()
    {
        navMeshAgent.SetDestination(lastPerceivedPosition);
        if (Vector3.Distance(lastPerceivedPosition, lastPerceivedPosition) < reachingDistance)
        {
            onLastPerceivedPositionReached.Invoke();
        }
    }
}