#region

using Entity.Scripts.AI;
using UnityEngine;

#endregion

public class MeleeAttackState : AIState
{
    [SerializeField] private float attackDistance = 1f;


    public override void Enter()
    {
        entityWeapons.DisableAllWeapons();
    }

    private void Update()
    {
        Vector3 destination = decissionMaker ? decissionMaker.target.transform.position : target.position;

        navMeshAgent.SetDestination(destination);

        animator.SetBool(
            "isAttacking",
            decissionMaker
                ? Vector3.Distance(decissionMaker.target.transform.position, transform.position) < attackDistance
                : false);
    }

    public override void Exit()
    {
    }
}