#region

using Entity.Scripts.AI;
using UnityEngine;

#endregion

public class PatrolState : AIState
{
    [SerializeField] Transform patrolPointsParent;
    [SerializeField] float reachingDistance = 1f;

    private int currentPoint;

    public override void Enter()
    {
        // no - op
    }

    private void Update()
    {
        Vector3 destination = patrolPointsParent.GetChild(currentPoint).position;
        navMeshAgent.destination = destination;
        if ((transform.position - destination).sqrMagnitude < (reachingDistance * reachingDistance))
        {
            currentPoint++;
            if (currentPoint >= patrolPointsParent.childCount)
            {
                currentPoint = 0;
            }
        }
    }

    public override void Exit()
    {
        // no - op
    }
}