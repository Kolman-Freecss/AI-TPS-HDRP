#region

using System.Collections;
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

    [Header("Weapon Settings")] [SerializeField]
    private int ammoCount;

    [SerializeField] private int ammoInClipCapacity;

    [SerializeField] private int ammoClips;

    [SerializeField] private int timeToReload;

    [SerializeField] private AudioClip shootAudioClip;

    [Header("IK Constraints Settings")] [SerializeField]
    private Transform leftHandWeaponIKTarget;

    [SerializeField] private Transform rightHandWeaponIKTarget;
    [SerializeField] private Transform leftHintWeaponIKTarget;
    [SerializeField] private Transform rightHintWeaponIKTarget;

    protected Animator animator;
    Barrel[] barrels;

    private bool isReloading = false;
    private AudioSource audioSource;

    private void Awake()
    {
        barrels = GetComponentsInChildren<Barrel>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    #region Shot Logic

    public void PlayShotSound()
    {
        audioSource.PlayOneShot(shootAudioClip);
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

    public bool CanShot()
    {
        return HasAmmo() && !IsReloading();
    }

    #endregion


    #region Ammo Logic

    public void Reload()
    {
        if (CanReload())
        {
            isReloading = true;
            if (animator != null)
            {
                animator.SetTrigger("Reload");
            }

            int ammoToReload = ammoInClipCapacity - ammoCount;
            ReduceCurrentAmmoClip();
            IncreaseCurrentAmmo(ammoToReload);
            StartCoroutine(ReloadCoroutine());
        }
        else if (!HasAmmoClips())
        {
            Debug.Log("No ammo clips");
        }
        else if (IsAmmoFull())
        {
            Debug.Log("Ammo is full");
        }
        else if (IsReloading())
        {
            Debug.Log("Reloading");
        }

        IEnumerator ReloadCoroutine()
        {
            yield return new WaitForSeconds(timeToReload);
            isReloading = false;
        }
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    public void IncreaseCurrentAmmo(int ammoAmount)
    {
        ammoCount += ammoAmount;
    }

    public void DecreaseCurrentAmmo(int ammoAmount)
    {
        ammoCount -= ammoAmount;
    }

    public void ReduceCurrentAmmoClip()
    {
        ammoClips--;
    }

    public bool CanReload()
    {
        return HasAmmoClips() && !IsAmmoFull() && !IsReloading();
    }

    public bool IsAmmoFull()
    {
        return ammoCount == ammoInClipCapacity;
    }

    public bool IsCurrentAmmoEmpty()
    {
        return ammoCount == 0;
    }

    public bool HasAmmo()
    {
        return ammoCount > 0;
    }

    public bool HasAmmoClips()
    {
        return ammoClips > 0;
    }

    public string GetAmmoInfo()
    {
        return ammoCount + "/" + ammoClips;
    }

    #endregion


    #region Getters & Setters

    public Transform LeftHandWeaponIKTarget => leftHandWeaponIKTarget;
    public Transform RightHandWeaponIKTarget => rightHandWeaponIKTarget;
    public Transform LeftHintWeaponIKTarget => leftHintWeaponIKTarget;

    public Transform RightHintWeaponIKTarget => rightHintWeaponIKTarget;

    public Animator Animator => animator;

    public int AmmoCount => ammoCount;

    public int AmmoInClipCapacity => ammoInClipCapacity;

    public int AmmoClips => ammoClips;

    public AudioSource AudioSource => audioSource;

    #endregion
}