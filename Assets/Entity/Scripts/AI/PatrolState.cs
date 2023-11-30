#region

using UnityEngine;
using UnityEngine.AI;

#endregion

public class PatrolState : MonoBehaviour
{
    [SerializeField] Transform patrolPointsParent;
    [SerializeField] float reachingDistance = 1f;

    private NavMeshAgent navMeshAgent;
    private int currentPoint;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
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
}