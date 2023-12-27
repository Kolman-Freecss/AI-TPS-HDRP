#region

using UnityEngine;

#endregion

public class Weapon : MonoBehaviour
{
    public enum ShotMode
    {
        ShotByShot,
        Continuous
    };

    public ShotMode shotMode;

    [Header("IK Constraints Settings")] [SerializeField]
    private Transform leftHandWeaponIKTarget;

    [SerializeField] private Transform rightHandWeaponIKTarget;
    [SerializeField] private Transform leftHintWeaponIKTarget;
    [SerializeField] private Transform rightHintWeaponIKTarget;

    Barrel[] barrels;

    private void Awake()
    {
        barrels = GetComponentsInChildren<Barrel>();
    }

    public void Shot()
    {
        foreach (Barrel barrel in barrels)
        {
            barrel.Shot();
        }
    }

    public void StartShooting()
    {
        foreach (Barrel barrel in barrels)
        {
            barrel.StartShooting();
        }
    }

    public void StopShooting()
    {
        foreach (Barrel barrel in barrels)
        {
            barrel.StopShooting();
        }
    }

    #region Getters & Setters

    public Transform LeftHandWeaponIKTarget => leftHandWeaponIKTarget;
    public Transform RightHandWeaponIKTarget => rightHandWeaponIKTarget;
    public Transform LeftHintWeaponIKTarget => leftHintWeaponIKTarget;

    public Transform RightHintWeaponIKTarget => rightHintWeaponIKTarget;

    #endregion
}