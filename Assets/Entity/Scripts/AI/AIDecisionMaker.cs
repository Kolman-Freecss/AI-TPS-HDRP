#region

using UnityEngine;

#endregion

namespace Entity.Scripts.AI
{
    public class AIDecisionMaker : MonoBehaviour, IVisible
    {
        public enum RangedEnemyType
        {
            NonRanged,
            Valiants,
            Cautious,
            Ambushers,
            Guardian
        }

        [SerializeField] RangedEnemyType rangedEnemyType = RangedEnemyType.NonRanged;

        [SerializeField] string allegiance = "Enemy";
        [SerializeField] private float shootRange = 15f;
        [SerializeField] public float valiantRange = 5f;

        [SerializeField] private AIState startState;

        EntitySight entitySight;
        EntityWeapons entityWeapons;
        MeleeAttackState meleeAttackState;
        PatrolState patrolState;
        SeekingState seekingState;
        ShootingState shootingState;
        ValiantState valiantState;
        LookingInLastPerceivedPosition lookingInLastPerceivedPosition;

        AIState[] aiStates;
        AIState currentState;

        public Transform target { get; private set; }
        Vector3 lastPerceivedPosition;
        bool hasLastPerceivedPosition;

        private void Awake()
        {
            aiStates = GetComponents<AIState>();
            meleeAttackState = GetComponent<MeleeAttackState>();
            patrolState = GetComponent<PatrolState>();
            entitySight = GetComponentInChildren<EntitySight>();
            seekingState = GetComponent<SeekingState>();
            entityWeapons = GetComponent<EntityWeapons>();
            shootingState = GetComponent<ShootingState>();
            valiantState = GetComponent<ValiantState>();
            lookingInLastPerceivedPosition = GetComponent<LookingInLastPerceivedPosition>();

            lookingInLastPerceivedPosition.onLastPerceivedPositionReached.AddListener(OnLastPerceivedPositionReached);
        }

        void OnLastPerceivedPositionReached()
        {
            hasLastPerceivedPosition = false;
        }

        private void Start()
        {
            foreach (AIState s in aiStates)
            {
                s.decissionMaker = this;
            }

            SetState(startState);
        }

        void Update()
        {
            Transform target = entitySight.visiblesInSight.Find((x) => x.GetAllegiance() != GetAllegiance())
                ?.GetTransform();

            if (target)
            {
                lastPerceivedPosition = target.position;
                hasLastPerceivedPosition = rangedEnemyType != RangedEnemyType.Guardian;
                if (entityWeapons && rangedEnemyType != RangedEnemyType.NonRanged)
                {
                    if (Vector3.Distance(target.position, transform.position) < shootRange)
                    {
                        if (rangedEnemyType == RangedEnemyType.Valiants)
                        {
                            SetState(valiantState);
                        }
                        else
                        {
                            SetState(shootingState);
                        }
                    }
                    else
                    {
                        if (rangedEnemyType == RangedEnemyType.Guardian)
                        {
                            SetState(shootingState);
                        }
                        else
                        {
                            SetState(seekingState);
                        }
                    }
                }
                else
                {
                    SetState(meleeAttackState);
                }
            }
            else if (hasLastPerceivedPosition)
            {
                lookingInLastPerceivedPosition.lastPerceivedPosition = lastPerceivedPosition;
                SetState(lookingInLastPerceivedPosition);
            }
            else
            {
                SetState(patrolState);
            }
        }

        private void SetState(AIState newState)
        {
            if (currentState != newState)
            {
                currentState?.Exit();
                foreach (AIState s in aiStates)
                {
                    if (s == newState)
                    {
                        s.enabled = true;
                        s.Enter();
                    }
                    else
                    {
                        s.enabled = false;
                    }
                }

                currentState = newState;
            }
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public string GetAllegiance()
        {
            return allegiance;
        }
    }
}