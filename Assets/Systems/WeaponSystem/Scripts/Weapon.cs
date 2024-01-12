#region

using System.Collections;
using _3rdPartyAssets.Packages.KolmanFreecss.Scripts;
using Systems.WeaponSystem.Scripts;
using UnityEngine;
using CharacterController = _3rdPartyAssets.Packages.KolmanFreecss.Systems.CharacterController.CharacterController;

#endregion

public class Weapon : MonoBehaviour
{
    public enum ShotMode
    {
        ShotByShot,
        Continuous
    };

    public ShotMode shotMode;

    [Header("Weapon UI Settings")] [SerializeField]
    private Sprite weaponSprite;

    [Header("Weapon Settings")] public WeaponType WeaponType;

    [SerializeField] private int ammoCount;

    [SerializeField] private int ammoInClipCapacity;

    [SerializeField] private int ammoClips;

    [SerializeField] private int timeToReload;

    [SerializeField] private AudioClip shootAudioClip;
    [SerializeField] private AudioClip reloadAudioClip;
    [SerializeField] private bool haveMagazine = false;
    [SerializeField] private AudioClip emptyClipAudioClip;
    [SerializeField] private AudioClip magazineDropAudioClip;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject magazineWeapon;

    [Header("IK Constraints Settings")] [SerializeField]
    private Transform leftHandWeaponIKTarget;

    [SerializeField] private Transform rightHandWeaponIKTarget;
    [SerializeField] private Transform leftHintWeaponIKTarget;
    [SerializeField] private Transform rightHintWeaponIKTarget;

    #region Member Variables

    protected Animator animator;
    private Barrel[] barrels;

    private bool isReloading = false;
    private AudioSource audioSource;
    private WeaponAnimationEvent weaponAnimationEvent;
    [HideInInspector] public bool active = false;

    private GameObject magazineHand;

    //TODO: Remove this dependency
    [SerializeField] private CharacterController entityAnimable;

    [HideInInspector] public bool shooting = false;

    //TODO: Shit aux variables for shotbyshot
    [HideInInspector] public bool playWeaponShoot = false;
    [HideInInspector] public bool playWeaponShotByShot = false;
    [HideInInspector] public bool checkCanShot = false;
    [HideInInspector] public bool playCantShoot = false;

    #endregion

    private void Awake()
    {
        barrels = GetComponentsInChildren<Barrel>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        weaponAnimationEvent = GetComponent<WeaponAnimationEvent>();

        haveMagazine = magazineWeapon != null;

        foreach (Barrel barrel in barrels) barrel.AssignWeapon(this);
    }

    private void OnEnable()
    {
        if (weaponAnimationEvent != null) weaponAnimationEvent.animationEvent.AddListener(OnWeaponAnimationEvent);
    }

    #region Shot Logic

    public void ResetWeaponTemp()
    {
        animator.speed = 0f;
        isReloading = false;
        animator.Rebind();
        if (magazineHand)
            Destroy(magazineHand);
        animator.speed = 1f;
    }

    public void PlayShotSound()
    {
        if (audioSource == null || shootAudioClip == null)
        {
            Debug.LogWarning("No audio source found for " + gameObject.name);
            return;
        }

        audioSource.PlayOneShot(shootAudioClip);
    }

    public void PlayEmptyClipSound()
    {
        if (audioSource == null || emptyClipAudioClip == null)
        {
            Debug.LogWarning("No audio source found for " + gameObject.name);
            return;
        }

        audioSource.PlayOneShot(emptyClipAudioClip);
    }

    public void PlayShot()
    {
        if (!playWeaponShoot)
        {
            playWeaponShoot = true;
            DecreaseCurrentAmmo(1);
            PlayShotSound();
        }
    }

    public void Shot()
    {
        playWeaponShoot = false;
        checkCanShot = false;
        playWeaponShotByShot = false;
        playCantShoot = false;
        foreach (Barrel barrel in barrels) barrel.Shot();
    }

    public void StartShooting()
    {
        foreach (Barrel barrel in barrels) barrel.StartShooting();
    }

    public void StopShooting()
    {
        foreach (Barrel barrel in barrels) barrel.StopShooting();
    }

    public bool CanShot()
    {
        if (shotMode == ShotMode.ShotByShot)
            return (HasAmmo() || playWeaponShoot) && !IsReloading();
        return HasAmmo() && !IsReloading();
    }

    #endregion


    #region Ammo Logic

    private void OnWeaponAnimationEvent(string eventName)
    {
        if (!active) return;
        switch (eventName)
        {
            case "detach_magazine":
                DetachMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                break;
            case "refill_one_ammo":
                RefillOneAmmo();
                break;
            case "refill_one_ammo_instant":
                RefillOneAmmoInstant();
                break;
        }

        void RefillOneAmmoInstant()
        {
            if (!IsAmmoFull() && HasAmmoClips())
            {
                audioSource.PlayOneShot(reloadAudioClip);
                ReduceCurrentAmmoClip();
                IncreaseCurrentAmmo(1);
            }
        }

        void RefillOneAmmo()
        {
            if (!IsAmmoFull() && HasAmmoClips())
            {
                audioSource.PlayOneShot(reloadAudioClip);
                ReduceCurrentAmmoClip();
                IncreaseCurrentAmmo(1);
            }

            if (IsAmmoFull() || !HasAmmoClips()) animator.SetBool("Reloading", false);
        }

        void DetachMagazine()
        {
            magazineHand = Instantiate(magazineWeapon, entityAnimable.GetLeftHand(), true);
            magazineHand.GetComponent<MeshRenderer>().enabled = false;
            magazineWeapon.SetActive(false);
        }

        void DropMagazine()
        {
            GameObject droppedMagazine = Instantiate(magazineHand, magazineHand.transform.position,
                magazineHand.transform.rotation);
            droppedMagazine.GetComponent<MeshRenderer>().enabled = true;
            droppedMagazine.AddComponent<Rigidbody>();
            droppedMagazine.AddComponent<BoxCollider>();
            AudioSource localAudioSource = droppedMagazine.AddComponent<AudioSource>();
            SoundOnCollision s = droppedMagazine.AddComponent<SoundOnCollision>();
            s.collisionSound = magazineDropAudioClip;
            s.audioSource = localAudioSource;
            droppedMagazine.SetActive(true);
            // s.audioSource.outputAudioMixerGroup = SoundManager.Instance.EffectsAudioMixerGroup;
            Destroy(droppedMagazine, 5f);
            magazineHand.SetActive(false);
        }

        void RefillMagazine()
        {
            magazineHand.SetActive(false);
        }

        void AttachMagazine()
        {
            IncreaseAmmo();
            magazineWeapon.SetActive(true);
            Destroy(magazineHand);
        }

        void IncreaseAmmo()
        {
            int ammoToReload = ammoInClipCapacity - ammoCount;
            ReduceCurrentAmmoClip();
            IncreaseCurrentAmmo(ammoToReload);
        }
    }

    public void Reload()
    {
        if (CanReload())
        {
            isReloading = true;
            if (animator != null) animator.SetTrigger("Reload");

            if (audioSource != null && reloadAudioClip != null && haveMagazine)
                audioSource.PlayOneShot(reloadAudioClip);

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
            if (!haveMagazine)
                animator.SetBool("Reloading", true);
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
        int maxAmmo = ammoAmount;
        maxAmmo = Mathf.Clamp(maxAmmo, 0, ammoInClipCapacity - ammoCount);
        ammoCount += maxAmmo;
    }

    public void DecreaseCurrentAmmo(int ammoAmount)
    {
        ammoCount -= ammoAmount;
        if (ammoCount < 0) ammoCount = 0;
    }

    public void ReduceCurrentAmmoClip()
    {
        ammoClips--;
    }

    public bool CanReload()
    {
        return HasAmmoClips() && !IsAmmoFull() && !IsReloading();
    }

    public bool HasAmmoOrAmmoClips()
    {
        return HasAmmo() || HasAmmoClips();
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

    private void OnDisable()
    {
        if (weaponAnimationEvent != null) weaponAnimationEvent.animationEvent.RemoveListener(OnWeaponAnimationEvent);
    }


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

    public GameObject BulletPrefab => bulletPrefab;

    public Sprite WeaponSprite => weaponSprite;

    #endregion
}