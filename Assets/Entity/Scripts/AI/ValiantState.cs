#region

using Entity.Scripts.AI;
using UnityEngine;

#endregion

public class ValiantState : ShootingState
{
    [SerializeField] private float seekFactor = 1f;

    protected override void PreUpdate()
    {
        UpdateGetCloserAndMoveAround();
    }

    void UpdateGetCloserAndMoveAround()
    {
        Vector3 desiredPosition = transform.position + transform.right;
        if (Vector3.Distance(transform.position, decissionMaker.transform.position) > decissionMaker.valiantRange)
        {
            Vector3 direction = decissionMaker.target.position - transform.position;
            desiredPosition += direction.normalized * seekFactor;
        }

        navMeshAgent.SetDestination(desiredPosition);
    }
}