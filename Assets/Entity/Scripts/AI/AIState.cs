#region

using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Entity.Scripts.AI
{
    public abstract class AIState : MonoBehaviour
    {
        public AIDecisionMaker decissionMaker;

        protected NavMeshAgent navMeshAgent;
        protected Animator animator;
        protected EntityWeapons entityWeapons;
        public Transform target;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            entityWeapons = GetComponent<EntityWeapons>();
            decissionMaker = GetComponent<AIDecisionMaker>();
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }
    }
}