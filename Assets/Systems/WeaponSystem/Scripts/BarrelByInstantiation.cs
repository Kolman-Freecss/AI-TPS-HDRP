#region

using UnityEngine;

#endregion

public class BarrelByInstantiation : Barrel
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;

    [SerializeField] private float cadence = 10f; //Shots/s

    private float nextShotTime = 0f;

    public override void Shot()
    {
        if (Time.time > nextShotTime && !weapon.shooting)
        {
            nextShotTime = Time.time + 1f / cadence;
            weapon.shooting = true;
            if (!weapon.CanShot())
            {
                CantShoot();
                weapon.shooting = false;
                return;
            }

            base.Shot();

            Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            weapon.shooting = false;
        }
    }
}