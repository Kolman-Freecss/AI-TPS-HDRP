#region

using UnityEngine;
using UnityEngine.AI;
using CharacterController = _3rdPartyAssets.Packages.KolmanFreecss.Systems.CharacterController.CharacterController;

#endregion

public class Enemy : CharacterController
{
    [SerializeField] private Transform leftHand;

    [SerializeField] private string allegiance = "Enemy";

    private NavMeshAgent agent;
    private EntitySight entitySight;

    private PatrolState patrolState;
    private MeleeAttackState meleeAttackState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        entitySight = GetComponentInChildren<EntitySight>();
        patrolState = GetComponent<PatrolState>();
        meleeAttackState = GetComponent<MeleeAttackState>();
    }

    #region IEntityAnimable Implementation

    public override Vector3 GetLastVelocity()
    {
        return agent.velocity;
    }

    public override float GetVerticalVelocity()
    {
        return 0f;
    }

    public override float GetJumpSpeed()
    {
        return 0f;
    }

    public override bool IsGrounded()
    {
        return true;
    }

    public override Transform GetLeftHand()
    {
        return leftHand;
    }

    #endregion

    #region IVisible Implementation

    public override Transform GetTransform()
    {
        return transform;
    }

    public override string GetAllegiance()
    {
        return allegiance;
    }

    #endregion
}