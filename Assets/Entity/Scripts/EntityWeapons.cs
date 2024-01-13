#region

using Gameplay.GameplayObjects.UI;
using Systems.WeaponSystem.Scripts;
using UnityEngine;

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

    private PlayerMeleeAttackController playerMeleeAttackController;
    private PlayerController _playerController;

    protected virtual void Awake()
    {
        playerMeleeAttackController = GetComponent<PlayerMeleeAttackController>();
        _playerController = GetComponent<PlayerController>();
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
        if (currentWeapon != -1 && !meleeAttacking)
        {
            Weapon currentWeapon = weapons[this.currentWeapon];
            if (currentWeapon.LeftHandWeaponIKTarget != null && leftHandIKTarget != null)
                SetIKTarget(leftHandIKTarget, currentWeapon.LeftHandWeaponIKTarget);
            if (currentWeapon.RightHandWeaponIKTarget != null && rightHandIKTarget != null)
                SetIKTarget(rightHandIKTarget, currentWeapon.RightHandWeaponIKTarget);
            if (currentWeapon.LeftHintWeaponIKTarget != null && leftHintIKTarget != null)
                SetIKTarget(leftHintIKTarget, currentWeapon.LeftHintWeaponIKTarget);
            if (currentWeapon.RightHintWeaponIKTarget != null && rightHintIKTarget != null)
                SetIKTarget(rightHintIKTarget, currentWeapon.RightHintWeaponIKTarget);
        }
        else
        {
            if (meleeWeapon.LeftHandWeaponIKTarget != null && leftHandIKTarget != null)
                SetIKTarget(leftHandIKTarget, meleeWeapon.LeftHandWeaponIKTarget);
            if (meleeWeapon.RightHandWeaponIKTarget != null && rightHandIKTarget != null)
                SetIKTarget(rightHandIKTarget, meleeWeapon.RightHandWeaponIKTarget);
            if (meleeWeapon.LeftHintWeaponIKTarget != null && leftHintIKTarget != null)
                SetIKTarget(leftHintIKTarget, meleeWeapon.LeftHintWeaponIKTarget);
            if (meleeWeapon.RightHintWeaponIKTarget != null && rightHintIKTarget != null)
                SetIKTarget(rightHintIKTarget, meleeWeapon.RightHintWeaponIKTarget);
        }

        void SetIKTarget(Transform ikConstraint, Transform weaponTarget)
        {
            ikConstraint.position = weaponTarget.position;
            ikConstraint.rotation = weaponTarget.rotation;
        }
    }

    internal void SelectNextWeapon()
    {
        int nextWeapon = currentWeapon + 1;
        if (nextWeapon >= weapons.Length) nextWeapon = -1;

        SetCurrentWeapon(nextWeapon);
    }

    internal void SelectPreviousWeapon()
    {
        int prevWeapon = currentWeapon - 1;
        if (prevWeapon < -1) prevWeapon = weapons.Length - 1;

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