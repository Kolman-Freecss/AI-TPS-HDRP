#region

using UnityEngine;
using UnityEngine.AI;

#endregion

public class MeleeAttackState : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] private float attackDistance = 1f;

    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Vector3 destination = target ? target.position : transform.position;

        agent.SetDestination(destination);

        animator.SetBool(
            "isAttacking",
            target ? Vector3.Distance(target.position, transform.position) < attackDistance : false);
    }
}