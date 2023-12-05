#region

using System;
using Entity.Scripts.AI;
using UnityEngine;

#endregion

public class MeleeAttackState : AIState
{
    [SerializeField] private float attackDistance = 1f;


    public override void Enter()
    {
        throw new NotImplementedException();
    }

    void Update()
    {
        Vector3 destination = decissionMaker ? decissionMaker.transform.position : transform.position;

        navMeshAgent.SetDestination(destination);

        animator.SetBool(
            "isAttacking",
            decissionMaker
                ? Vector3.Distance(decissionMaker.transform.position, transform.position) < attackDistance
                : false);
    }

    public override void Exit()
    {
        throw new NotImplementedException();
    }
}