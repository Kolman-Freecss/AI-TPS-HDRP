#region

using UnityEngine;

#endregion

public class BarrelByParticles : Barrel
{
    ParticleSystem.EmissionModule emission;

    private void Awake()
    {
        emission = GetComponentInChildren<ParticleSystem>().emission;
    }

    private void Start()
    {
        if (!weapon.CanShot())
        {
            base.CantShoot();
        }

        emission.enabled = false;
    }

    public override void StartShooting()
    {
        emission.enabled = true;
    }

    public override void StopShooting()
    {
        emission.enabled = false;
    }
}