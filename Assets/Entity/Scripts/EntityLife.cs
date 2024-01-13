#region

using Gameplay.GameplayObjects.Interactables._derivatives;
using Gameplay.GameplayObjects.Player.Script;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

#endregion

public class EntityLife : MonoBehaviour
{
    [SerializeField] private float minDeathPushForce = 3000f;
    [SerializeField] private float maxDeathPushForce = 4500f;
    [SerializeField] private float maxLife = 3f;
    [SerializeField] private float timeToDestroyEnemyAfterDeath = 45f;

    [Header("Debug")] [SerializeField] private bool debugHit;
    [SerializeField] private Transform debugOffender;
    [SerializeField] private float damage = 1f;

    private HurtBox hurtBox;
    private EntityRagdollizer entityRagdollizer;
    private float currentLife;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private PlayerController playerController;
    private CharacterController characterController;
    private LifeBar lifeBar;
    private Enemy enemy;

    private EntityWeapons entityWeapons;

    public UnityEvent onDeath = new();

    private void OnValidate()
    {
        if (debugHit)
        {
            debugHit = false;
            OnHitNotifiedWithOffender(damage, debugOffender ? debugOffender : transform);
        }
    }

    private void Awake()
    {
        entityWeapons = GetComponent<EntityWeapons>();
        currentLife = maxLife;
        hurtBox = GetComponent<HurtBox>();
        entityRagdollizer = GetComponentInChildren<EntityRagdollizer>();
        animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        enemy = GetComponent<Enemy>();
        lifeBar = GetComponentInChildren<LifeBar>();
    }

    private void OnEnable()
    {
        hurtBox.onHitNotifiedWithOffender.AddListener(OnHitNotifiedWithOffender);
    }

    private void OnDisable()
    {
        hurtBox.onHitNotifiedWithOffender.RemoveListener(OnHitNotifiedWithOffender);
    }

    public void IncreaseLife(int lifeToRestore)
    {
        currentLife += lifeToRestore;
        if (currentLife > maxLife)
            currentLife = maxLife;
        lifeBar.SetNormalizedValue(Mathf.Clamp01(currentLife / maxLife));
        lifeBar.SetText(currentLife);
    }

    private void OnHitNotifiedWithOffender(float damage, Transform offender)
    {
        if (currentLife > 0f)
        {
            currentLife -= 1f;
            if (lifeBar.LifeBarText != null)
                lifeBar.SetText(currentLife);
            lifeBar.SetNormalizedValue(Mathf.Clamp01(currentLife / maxLife));
            if (currentLife <= 0f)
            {
                if (!playerController)
                    animator.enabled = false;

                if (navMeshAgent) navMeshAgent.enabled = false;

                if (playerController)
                {
                    GetComponent<FPSCameraController>().enabled = false;
                    entityWeapons.DisableAllWeapons();
                    animator.SetTrigger("DeathAction");
                    playerController.enabled = false;
                    onDeath.Invoke();
                }

                if (characterController)
                {
                    characterController.enabled = false;
                    onDeath.Invoke();
                }

                if (enemy)
                {
                    enemy.enabled = false;
                    enemy.GetComponent<CapsuleCollider>().enabled = false;
                    entityRagdollizer.Ragdollize();
                    entityRagdollizer.Push(transform.position - offender.position, minDeathPushForce,
                        maxDeathPushForce);

                    enemy.GetComponent<SphereCollider>().enabled = true;
                    EnemyInteractable enemyInteractable = enemy.GetComponent<EnemyInteractable>();
                    enemyInteractable.enabled = true;
                    if (enemyInteractable.OnInteraction.GetPersistentEventCount() == 0)
                        enemyInteractable.OnInteraction.AddListener(GameManager.Instance.m_player
                            .GetComponent<PlayerStats>().OnInteraction);
                    EnemyLoot el = enemy.GetComponent<EnemyLoot>();
                    el.DropLoot();

                    GameManager.Instance.m_player.GetComponent<PlayerStats>().OnEnemyDeath(el);

                    var parent = transform.parent;
                    Destroy(parent.gameObject, timeToDestroyEnemyAfterDeath);
                }
            }
        }
    }

    #region Getter & Setter

    public bool IsAlive()
    {
        return currentLife > 0f;
    }

    #endregion
}