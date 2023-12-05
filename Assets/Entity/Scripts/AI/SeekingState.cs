#region

using Entity.Scripts.AI;

#endregion

public class SeekingState : AIState
{
    void Update()
    {
        navMeshAgent.SetDestination(decissionMaker.transform.position);
    }
}