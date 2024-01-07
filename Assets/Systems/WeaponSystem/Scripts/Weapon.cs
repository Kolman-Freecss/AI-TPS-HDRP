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

    [Header("Weapon Settings")] public WeaponType WeaponType;

    [SerializeField] private int ammoCount;

    [SerializeField] private int ammoInClipCapacity;

    [SerializeField] private int ammoClips;

    [SerializeField] private int timeToReload;

    [SerializeField] private AudioClip shootAudioClip;
    [SerializeField] private AudioClip reloadAudioClip;
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

    #endregion

    private void Awake()
    {
        barrels = GetComponentsInChildren<Barrel>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        weaponAnimationEvent = GetComponent<WeaponAnimationEvent>();
    }

    private void Start()
    {
        if (weaponAnimationEvent != null) weaponAnimationEvent.animationEvent.AddListener(OnWeaponAnimationEvent);
    }

    #region Shot Logic

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

    public void Shot()
    {
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
        }

        void DetachMagazine()
        {
            magazineHand = Instantiate(magazineWeapon, entityAnimable.GetLeftHand(), true);
            magazineWeapon.SetActive(false);
        }

        void DropMagazine()
        {
            GameObject droppedMagazine = Instantiate(magazineHand, magazineHand.transform.position,
                magazineHand.transform.rotation);
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
            magazineHand.SetActive(true);
        }

        void AttachMagazine()
        {
            magazineWeapon.SetActive(true);
            Destroy(magazineHand);
        }
    }

    public void Reload()
    {
        if (CanReload())
        {
            isReloading = true;
            if (animator != null) animator.SetTrigger("Reload");

            if (audioSource != null && reloadAudioClip != null) audioSource.PlayOneShot(reloadAudioClip);

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

    #endregion
}