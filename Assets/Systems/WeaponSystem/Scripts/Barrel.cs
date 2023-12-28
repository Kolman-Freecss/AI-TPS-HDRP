#region

using UnityEngine;

#endregion

public abstract class Barrel : MonoBehaviour
{
    [Header("Debug")] [SerializeField] private bool debugShot;

    [Header("Debug")] [SerializeField] private bool debugContinuousShot;

    protected Weapon weapon;

    // void OnValidate()
    // {
    //     if (debugShot)
    //     {
    //         debugShot = false;
    //         Shot();
    //     }
    //     
    //     if (debugContinuousShot)
    //     {
    //         StartShooting();
    //     } else 
    //     {
    //         StopShooting();
    //     }
    // }

    private void Awake()
    {
        weapon = GetComponentInParent<Weapon>();
    }

    public virtual void Shot()
    {
        weapon.DecreaseCurrentAmmo(1);
        weapon.PlayShotSound();
        if (weapon.Animator != null)
        {
            Debug.Log("Shot animation");
            weapon.Animator.SetTrigger("Shoot");
        }
    }

    protected void CantShoot()
    {
        if (!weapon.IsReloading())
        {
            return;
        }

        if (weapon.IsCurrentAmmoEmpty())
        {
            weapon.Reload();
        }
    }

    public virtual void StartShooting()
    {
    }

    public virtual void StopShooting()
    {
    }
}