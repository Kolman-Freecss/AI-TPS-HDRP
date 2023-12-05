#region

using UnityEngine;

#endregion

namespace Entity.Scripts.AI
{
    public class ShootingState : AIState
    {
        [SerializeField] private float shotsPerSecondInShotByShotWeapon = 2f;

        private float lastShotTime = 0f;

        public override void Enter()
        {
            navMeshAgent.SetDestination(transform.position);
            if (entityWeapons.GetCurrentWeapon().shotMode == Weapon.ShotMode.Continuous)
            {
                entityWeapons.StartShooting();
            }
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
        }

        protected virtual void PostUpdate()
        {
        }

        private void UpdateShoot()
        {
            if (entityWeapons.GetCurrentWeapon().shotMode == Weapon.ShotMode.ShotByShot)
            {
                if ((Time.time - lastShotTime) > 1f / shotsPerSecondInShotByShotWeapon)
                {
                    entityWeapons.Shot();
                    lastShotTime = Time.time;
                }
            }
        }

        private void UpdateOrientation()
        {
            Vector3 desiredDirection = target.position - decissionMaker.transform.position;

            desiredDirection = Vector3.ProjectOnPlane(desiredDirection, Vector3.up);
            float angularDistance = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);
            float angleToApply = navMeshAgent.angularSpeed * Time.deltaTime;
            angleToApply = Mathf.Min(angleToApply, Mathf.Abs(angularDistance));
            angleToApply *= Mathf.Sign(angularDistance);
            Quaternion rotationToApply = Quaternion.AngleAxis(angleToApply, Vector3.up);
            decissionMaker.transform.rotation = decissionMaker.transform.rotation * rotationToApply;
        }

        public override void Exit()
        {
            if (entityWeapons.GetCurrentWeapon().shotMode == Weapon.ShotMode.Continuous)
            {
                entityWeapons.StopShooting();
            }
        }
    }
}