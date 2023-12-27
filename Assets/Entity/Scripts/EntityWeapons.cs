#region

using UnityEngine;

#endregion

public class EntityWeapons : MonoBehaviour
{
    [SerializeField] private Transform weaponsParent;
    [SerializeField] private int startingWeaponIndex = 0;

    [Header("IK Constraints Settings")] [SerializeField]
    private Transform leftHandIKTarget;

    [SerializeField] private Transform rightHandIKTarget;
    [SerializeField] private Transform leftHintIKTarget;
    [SerializeField] private Transform rightHintIKTarget;

    Weapon[] weapons;
    private int currentWeapon = -1;

    private void Awake()
    {
        if (weaponsParent != null)
        {
            weapons = weaponsParent.GetComponentsInChildren<Weapon>();
            currentWeapon = weapons.Length > 0 ? 0 : -1;
            SetCurrentWeapon(startingWeaponIndex);
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
        if (currentWeapon != -1)
        {
            Weapon currentWeapon = weapons[this.currentWeapon];
            if (currentWeapon.LeftHandWeaponIKTarget != null)
                SetIKTarget(leftHandIKTarget, currentWeapon.LeftHandWeaponIKTarget);
            if (currentWeapon.RightHandWeaponIKTarget != null)
                SetIKTarget(rightHandIKTarget, currentWeapon.RightHandWeaponIKTarget);
            if (currentWeapon.LeftHintWeaponIKTarget != null)
                SetIKTarget(leftHintIKTarget, currentWeapon.LeftHintWeaponIKTarget);
            if (currentWeapon.RightHintWeaponIKTarget != null)
                SetIKTarget(rightHintIKTarget, currentWeapon.RightHintWeaponIKTarget);
        }

        void SetIKTarget(Transform ikConstraint, Transform weaponTarget)
        {
            ikConstraint.position = weaponTarget.position;
            ikConstraint.rotation = weaponTarget.rotation;
        }
    }

    internal void SelectNextWeapon()
    {
        int nextWeapon = ++currentWeapon;
        if (nextWeapon >= weapons.Length)
        {
            nextWeapon = 0;
        }

        SetCurrentWeapon(nextWeapon);
    }

    internal void SelectPreviousWeapon()
    {
        int prevWeapon = --currentWeapon;
        if (prevWeapon < weapons.Length)
        {
            prevWeapon = weapons.Length - 1;
        }

        SetCurrentWeapon(prevWeapon);
    }

    public void SetCurrentWeapon(int selectedWeapon)
    {
        for (int j = 0; j < weapons.Length; j++)
        {
            weapons[j].gameObject.SetActive(j == selectedWeapon);
        }

        currentWeapon = selectedWeapon;
    }

    public void Shot()
    {
        if (currentWeapon != -1)
        {
            weapons[currentWeapon].Shot();
        }
    }

    public void StartShooting()
    {
        if (currentWeapon != -1)
        {
            weapons[currentWeapon].StartShooting();
        }
    }

    public void StopShooting()
    {
        if (currentWeapon != -1)
        {
            weapons[currentWeapon].StopShooting();
        }
    }

    public bool HasCurrentWeapon()
    {
        return currentWeapon != -1;
    }

    public Weapon GetCurrentWeapon()
    {
        return weapons[currentWeapon];
    }
}