#region

using Entity.Scripts.AI;

#endregion

public class SeekingState : AIState
{
    private void Update()
    {
        navMeshAgent.SetDestination(target.transform.position);
    }
}