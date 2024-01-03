#region

using Entity.Scripts.Senses;
using UnityEngine;

#endregion

namespace Entity.Scripts.AI
{
    public class AIDecisionMaker : MonoBehaviour, IVisible
    {
        public enum RangedEnemyType
        {
            NonRanged, // They don't shoot
            Valiants, // They move closer to the player
            Cautious, // They move closer to the player but they don't move around
            Ambushers, // They don't move until they see the player
            Guardian // They don't move until they see the player, and they don't move around
        }

        [SerializeField] private RangedEnemyType rangedEnemyType = RangedEnemyType.NonRanged;

        [SerializeField] private string allegiance = "Enemy";
        [SerializeField] private float shootRange = 15f;
        [SerializeField] public float valiantRange = 5f;

        [SerializeField] private AIState startState;

        private EntitySight entitySight;
        private EntityAudition entityAudition;
        private EntityWeapons entityWeapons;

        private IdleState idleState;
        private MeleeAttackState meleeAttackState;
        private PatrolState patrolState;
        private SeekingState seekingState;
        private ShootingState shootingState;
        private ValiantState valiantState;
        private LookingInLastPerceivedPosition lookingInLastPerceivedPosition;

        private AIState[] aiStates;
        private AIState currentState;

        public Transform target { get; private set; }
        private Vector3 lastPerceivedPosition;
        private bool hasLastPerceivedPosition;

        private void Awake()
        {
            aiStates = GetComponents<AIState>();

            entityWeapons = GetComponent<EntityWeapons>();
            entitySight = GetComponentInChildren<EntitySight>();
            entityAudition = GetComponentInChildren<EntityAudition>();

            idleState = GetComponent<IdleState>();
            meleeAttackState = GetComponent<MeleeAttackState>();
            patrolState = GetComponent<PatrolState>();
            seekingState = GetComponent<SeekingState>();
            shootingState = GetComponent<ShootingState>();
            valiantState = GetComponent<ValiantState>();
            lookingInLastPerceivedPosition = GetComponent<LookingInLastPerceivedPosition>();

            lookingInLastPerceivedPosition.onLastPerceivedPositionReached.AddListener(OnLastPerceivedPositionReached);
        }

        private void OnLastPerceivedPositionReached()
        {
            hasLastPerceivedPosition = false;
        }

        private void Start()
        {
            foreach (AIState s in aiStates) s.decissionMaker = this;

            SetState(startState);
        }

        private void Update()
        {
            // Choose target
            Transform visibleTarget = entitySight.visiblesInSight.Find((x) => x.GetAllegiance() != GetAllegiance())
                ?.GetTransform();

            Transform audibleTarget = rangedEnemyType == RangedEnemyType.Ambushers
                ? null
                : entityAudition.heardAudibles.Find(
                        (x) => x.GetAllegiance() != GetAllegiance())
                    ?.audible.transform;

            target = null;
            if (!visibleTarget)
                target = audibleTarget;
            else if (audibleTarget)
                target = Vector3.Distance(visibleTarget.position, transform.position) <
                         Vector3.Distance(audibleTarget.position, transform.position)
                    ? visibleTarget
                    : audibleTarget;
            else
                target = visibleTarget;

            // Can see target? Can hear target?
            bool canSeeTarget = entitySight.visiblesInSight.Find(
                (x) => x.GetTransform() == target) != null;

            bool canHearTarget = entityAudition.heardAudibles.Find(
                (x) => x.audible.transform == target) != null;

            EntityLife targetEntityLife = target ? target.GetComponent<EntityLife>() : null;
            bool isTargetAlive = targetEntityLife ? targetEntityLife.IsAlive() : false;

            // Make decission
            if (target && isTargetAlive)
            {
                if (rangedEnemyType == RangedEnemyType.Ambushers) rangedEnemyType = RangedEnemyType.Valiants;

                lastPerceivedPosition = target.position;
                hasLastPerceivedPosition = rangedEnemyType != RangedEnemyType.Guardian;
                bool hasAmmo = entityWeapons ? entityWeapons.GetCurrentWeapon().HasAmmoOrAmmoClips() : false;
                if (entityWeapons && rangedEnemyType != RangedEnemyType.NonRanged && hasAmmo)
                {
                    if (canSeeTarget)
                    {
                        if (Vector3.Distance(target.position, transform.position) < shootRange)
                        {
                            if (rangedEnemyType == RangedEnemyType.Valiants)
                                SetState(valiantState);
                            else
                                SetState(shootingState);
                        }
                    }
                    else
                    {
                        if (rangedEnemyType == RangedEnemyType.Guardian)
                            SetState(shootingState);
                        else
                            SetState(seekingState);
                    }
                }
                else
                {
                    SetState(meleeAttackState);
                }
            }
            else if (target && !isTargetAlive)
            {
                entityAudition.RemoveAudible(target.GetComponent<Audible>());
                SetState(patrolState);
            }
            else if (hasLastPerceivedPosition)
            {
                lookingInLastPerceivedPosition.lastPerceivedPosition = lastPerceivedPosition;
                SetState(lookingInLastPerceivedPosition);
            }
            else if (rangedEnemyType == RangedEnemyType.Ambushers)
            {
                SetState(idleState);
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
                    if (s == newState)
                    {
                        s.enabled = true;
                        s.target = target;
                        s.Enter();
                    }
                    else
                    {
                        s.enabled = false;
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