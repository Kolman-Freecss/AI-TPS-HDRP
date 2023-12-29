#region

using Entity.Scripts;
using UnityEngine;
using UnityEngine.AI;

#endregion

public class Enemy : MonoBehaviour, IEntityAnimable, IVisible
{
    [SerializeField] private Transform leftHand;

    [SerializeField] string allegiance = "Enemy";

    private NavMeshAgent agent;
    EntitySight entitySight;

    PatrolState patrolState;
    MeleeAttackState meleeAttackState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        entitySight = GetComponentInChildren<EntitySight>();
        patrolState = GetComponent<PatrolState>();
        meleeAttackState = GetComponent<MeleeAttackState>();
    }

    #region IEntityAnimable Implementation

    public Vector3 GetLastVelocity()
    {
        return agent.velocity;
    }

    public float GetVerticalVelocity()
    {
        return 0f;
    }

    public float GetJumpSpeed()
    {
        return 0f;
    }

    public bool IsGrounded()
    {
        return true;
    }

    public Transform GetLeftHand()
    {
        return leftHand;
    }

    #endregion

    #region IVisible Implementation

    public Transform GetTransform()
    {
        return transform;
    }

    public string GetAllegiance()
    {
        return allegiance;
    }

    #endregion
}