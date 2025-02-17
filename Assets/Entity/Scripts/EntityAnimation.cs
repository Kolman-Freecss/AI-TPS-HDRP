﻿#region

using UnityEngine;

#endregion

namespace Entity.Scripts
{
    public class EntityAnimation : MonoBehaviour
    {
        [Header("Animation")] [SerializeField] private float transitionVelocity = 1f;

        private Vector3 smoothedAnimationVelocity = Vector3.zero;
        private Animator animator;
        private IEntityAnimable entityAnimable;
        private PlayerController playerController;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            entityAnimable = GetComponent<IEntityAnimable>();
            playerController = GetComponent<PlayerController>();
        }

        private void Update()
        {
            UpdateAnimation(entityAnimable.GetLastVelocity(),
                entityAnimable.GetVerticalVelocity(),
                entityAnimable.GetJumpSpeed(),
                entityAnimable.IsGrounded(),
                entityAnimable.HaveWeapon()
            );
        }

        private void UpdateAnimation(Vector3 lastVelocity, float verticalVelocity, float jumpSpeed, bool isGrounded,
            bool haveWeapon)
        {
            Vector3 velocityDistance = lastVelocity - smoothedAnimationVelocity;
            float transitionVelocityToApply = transitionVelocity * Time.deltaTime;
            transitionVelocityToApply = Mathf.Min(transitionVelocityToApply, velocityDistance.magnitude);

            smoothedAnimationVelocity += velocityDistance.normalized * transitionVelocityToApply;

            Vector3 localSmoothedAnimationVelocity = transform.InverseTransformDirection(lastVelocity);
            animator.SetFloat("SidewardVelocity", localSmoothedAnimationVelocity.x);
            animator.SetFloat("ForwardVelocity", localSmoothedAnimationVelocity.z);

            float clampedVerticalVelocity = Mathf.Clamp(verticalVelocity, -jumpSpeed, jumpSpeed);
            float normalizedVerticalVelocity = Mathf.InverseLerp(-jumpSpeed, jumpSpeed, clampedVerticalVelocity);

            animator.SetFloat("NormalizedVerticalVelocity", normalizedVerticalVelocity);
            animator.SetBool("IsGrounded", isGrounded);
            if (playerController != null) animator.SetBool("HaveWeapon", haveWeapon);
        }

        public Animator GetAnimator()
        {
            return animator;
        }
    }
}