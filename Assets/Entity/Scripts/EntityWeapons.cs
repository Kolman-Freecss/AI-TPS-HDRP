#region

using Gameplay.GameplayObjects.UI;
using Systems.WeaponSystem.Scripts;
using UnityEngine;
using UnityEngine.Animations.Rigging;

#endregion

public class EntityWeapons : MonoBehaviour
{
    [SerializeField] private Transform weaponsParent;
    [SerializeField] private int startingWeaponIndex = 0;
    [SerializeField] private MeleeWeapon meleeWeapon;

    [Header("IK Constraints Settings")] [SerializeField]
    private Transform leftHandIKTarget;

    [SerializeField] private Transform rightHandIKTarget;
    [SerializeField] private Transform leftHintIKTarget;
    [SerializeField] private Transform rightHintIKTarget;

    private Weapon[] weapons;
    private WeaponUI weaponsUI;
    private int currentWeapon = -1;
    private bool haveWeapon = false;
    private AudioSource weaponsAudioSource;
    private bool isPlayer = false;

    private RigBuilder rigBuilder;

    private PlayerMeleeAttackController playerMeleeAttackController;
    private PlayerController _playerController;
    private Enemy enemy;

    protected virtual void Awake()
    {
        enemy = GetComponent<Enemy>();
        playerMeleeAttackController = GetComponent<PlayerMeleeAttackController>();
        _playerController = GetComponent<PlayerController>();
        rigBuilder = GetComponentInChildren<RigBuilder>();

        if (weaponsParent != null)
        {
            weapons = weaponsParent.GetComponentsInChildren<Weapon>();
            isPlayer = GetComponent<PlayerController>() != null;
            if (isPlayer) weaponsUI = GetComponentInChildren<WeaponUI>();
            currentWeapon = weapons.Length > 0 ? 0 : -1;
            if (weaponsUI != null)
                for (int i = 0; i < weapons.Length; i++)
                    weaponsUI.AddWeapon(weapons[i], i);
            SetCurrentWeapon(startingWeaponIndex);
            weaponsAudioSource = weaponsParent.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogWarning("No weapons parent found for " + gameObject.name);
        }
    }

    private void LateUpdate()
    {
        SetIKTargets();
    }

    private void SetIKTargets()
    {
        bool meleeAttacking = playerMeleeAttackController != null && _playerController.meleeAttacking;
        bool enemyMeleeAttacking = enemy != null && !haveWeapon;
        bool isPlayer = _playerController != null;
        if (haveWeapon && !meleeAttacking)
        {
            rigBuilder.enabled = true;
            if (!isPlayer && enemyMeleeAttacking) return;
            Weapon currentWeaponTarget = weapons[currentWeapon];
            SetTargetsToIK(leftHandIKTarget, currentWeaponTarget.LeftHandWeaponIKTarget);
            SetTargetsToIK(rightHandIKTarget, currentWeaponTarget.RightHandWeaponIKTarget);
            SetTargetsToIK(leftHintIKTarget, currentWeaponTarget.LeftHintWeaponIKTarget);
            SetTargetsToIK(rightHintIKTarget, currentWeaponTarget.RightHintWeaponIKTarget);
        }
        else if (meleeAttacking)
        {
            rigBuilder.enabled = true;
            SetTargetsToIK(leftHandIKTarget, meleeWeapon.LeftHandWeaponIKTarget);
            SetTargetsToIK(rightHandIKTarget, meleeWeapon.RightHandWeaponIKTarget);
            SetTargetsToIK(leftHintIKTarget, meleeWeapon.LeftHintWeaponIKTarget);
            SetTargetsToIK(rightHintIKTarget, meleeWeapon.RightHintWeaponIKTarget);
        }
        else
        {
            rigBuilder.enabled = false;
        }

        void SetTargetsToIK(Transform bodyTarget, Transform weaponBodyConstraint)
        {
            if (bodyTarget != null && weaponBodyConstraint != null) SetIKTarget(bodyTarget, weaponBodyConstraint);
        }

        void SetIKTarget(Transform ikConstraint, Transform weaponTarget)
        {
            ikConstraint.position = weaponTarget.position;
            ikConstraint.rotation = weaponTarget.rotation;
        }
    }

    public void DisableAllWeapons()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(false);
            weapons[i].gameObject.GetComponent<Weapon>().active = false;
        }

        haveWeapon = false;
    }

    internal void SelectNextWeapon()
    {
        int nextWeapon = currentWeapon + 1;
        if (nextWeapon >= weapons.Length) nextWeapon = 0;

        SetCurrentWeapon(nextWeapon);
    }

    internal void SelectPreviousWeapon()
    {
        int prevWeapon = currentWeapon - 1;
        if (prevWeapon <= -1) prevWeapon = weapons.Length - 1;

        SetCurrentWeapon(prevWeapon);
    }

    public void SetCurrentWeapon(int selectedWeapon)
    {
        if (haveWeapon)
            weapons[currentWeapon].ResetWeaponTemp();
        for (int j = 0; j < weapons.Length; j++)
        {
            weapons[j].gameObject.SetActive(j == selectedWeapon);
            weapons[j].gameObject.GetComponent<Weapon>().active = j == selectedWeapon;
        }

        currentWeapon = selectedWeapon;
        haveWeapon = currentWeapon != -1;

        if (isPlayer)
        {
            if (haveWeapon)
                weaponsUI.SetWeaponSprite(weapons[currentWeapon].WeaponType);
            else
                weaponsUI.SetWeaponSprite(WeaponType.NONE);
        }

        if (weaponsAudioSource != null)
            weaponsAudioSource.Play();
    }

    public void ResetWeapon()
    {
        if (currentWeapon != -1 && haveWeapon)
            weapons[currentWeapon].ResetWeaponTemp();
    }

    public virtual void Reload()
    {
        if (currentWeapon != -1) weapons[currentWeapon].Reload();
    }

    public void Shot()
    {
        if (currentWeapon != -1) weapons[currentWeapon].Shot();
    }

    public void StartShooting()
    {
        if (currentWeapon != -1) weapons[currentWeapon].StartShooting();
    }

    public void StopShooting()
    {
        if (currentWeapon != -1) weapons[currentWeapon].StopShooting();
    }

    public bool HasCurrentWeapon()
    {
        return currentWeapon != -1;
    }

    public Weapon GetCurrentWeapon()
    {
        return weapons[currentWeapon];
    }

    public Weapon GetWeaponByIndex(int index)
    {
        return weapons[index];
    }

    public int GetCurrentWeaponIndex()
    {
        return currentWeapon;
    }
}