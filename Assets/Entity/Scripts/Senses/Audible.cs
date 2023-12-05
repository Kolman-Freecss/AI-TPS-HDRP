#region

using UnityEngine;

#endregion

namespace Entity.Scripts.Senses
{
    public class Audible : MonoBehaviour
    {
        [SerializeField] private float range = 10f;
        [SerializeField] private float emissionFrequency = 5f;
        [SerializeField] private string allegiance;

        [SerializeField] private float speedThresholdToEmit = 0f;

        private float lastEmissionTime;
        Vector3 lastPosition;

        private void Start()
        {
            lastEmissionTime = Time.time + Random.Range(0f, 1f / emissionFrequency);
            lastPosition = transform.position;
        }

        private void Update()
        {
            float currentSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;

            if (currentSpeed >= speedThresholdToEmit)
            {
                if (Time.time - lastEmissionTime > (1f / emissionFrequency))
                {
                    lastEmissionTime = Time.time;
                    Emit();
                }

                lastPosition = transform.position;
            }
        }

        private void Emit()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, range);
            foreach (Collider collider in colliders)
            {
                collider.GetComponent<EntityAudition>()?.NotifyAudible(this);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}