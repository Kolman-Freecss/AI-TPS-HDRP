#region

using Entity.Scripts.AI;

#endregion

public class IdleState : AIState
{
    private void Update()
    {
        navMeshAgent.destination = transform.position;
    }
}