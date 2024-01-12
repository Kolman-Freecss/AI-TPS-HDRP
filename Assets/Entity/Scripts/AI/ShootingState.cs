#region

using UnityEngine;

#endregion

namespace Entity.Scripts.AI
{
    public class ShootingState : AIState
    {
        [SerializeField] private float shotsPerSecondInShotByShotWeapon = 2f;

        private float lastShotTime = 0f;
        private Weapon weapon;
        [SerializeField] private float verticalAngularSpeed = 100f;

        public override void Enter()
        {
            navMeshAgent.SetDestination(transform.position);
            if (entityWeapons.GetCurrentWeapon().shotMode == Weapon.ShotMode.Continuous) entityWeapons.StartShooting();
        }

        private void Update()
        {
            PreUpdate();
            UpdateOrientation();
            UpdateShoot();
            PostUpdate();
        }

        protected virtual void PreUpdate()
        {
            weapon = entityWeapons.GetCurrentWeapon();
        }

        protected virtual void PostUpdate()
        {
        }

        private void UpdateShoot()
        {
            Weapon currentWeapon = entityWeapons.GetCurrentWeapon();
            if (!currentWeapon.HasAmmo() && currentWeapon.HasAmmoClips())
            {
                if (!currentWeapon.IsReloading()) entityWeapons.Reload();
                return;
            }

            if (entityWeapons.GetCurrentWeapon().shotMode == Weapon.ShotMode.ShotByShot)
                if (Time.time - lastShotTime > 1f / shotsPerSecondInShotByShotWeapon)
                {
                    entityWeapons.Shot();
                    lastShotTime = Time.time;
                }
        }

        private void UpdateOrientation()
        {
            Vector3 desiredDirection = target.position - transform.position;

            desiredDirection = Vector3.ProjectOnPlane(desiredDirection, Vector3.up);
            float angularDistance = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);
            float angleToApply = navMeshAgent.angularSpeed * Time.deltaTime;
            angleToApply = Mathf.Min(angleToApply, Mathf.Abs(angularDistance));
            angleToApply *= Mathf.Sign(angularDistance);
            Quaternion rotationToApply = Quaternion.AngleAxis(angleToApply, Vector3.up);
            OrientateWeaponYAxe();
            transform.rotation = transform.rotation * rotationToApply;
        }

        private void OrientateWeaponYAxe()
        {
            Vector3 desiredDirection = target.position - weapon.transform.position;
            float angularDistance = Vector3.SignedAngle(weapon.transform.forward, desiredDirection, Vector3.right);
            // float rotationToApply = target.position.y * verticalAngularSpeed * Time.deltaTime;
            float rotationToApply = verticalAngularSpeed * Time.deltaTime;
            rotationToApply = Mathf.Min(rotationToApply, Mathf.Abs(angularDistance));
            rotationToApply *= Mathf.Sign(angularDistance);
            Quaternion rotationToApplyQ = Quaternion.AngleAxis(-rotationToApply, Vector3.right);
            weapon.transform.rotation = weapon.transform.rotation * rotationToApplyQ;
            // weapon.transform.Rotate(Vector3.right, rotationToApplyQ.eulerAngles.x);
        }

        public override void Exit()
        {
            if (entityWeapons.GetCurrentWeapon().shotMode == Weapon.ShotMode.Continuous) entityWeapons.StopShooting();
        }
    }
}