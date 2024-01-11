#region

using UnityEngine;

#endregion

public class Explosion : MonoBehaviour
{
    [SerializeField] private float range = 5f;
    [SerializeField] private float force = 1000f;
    [SerializeField] private float upwardsModifier = 1000f;
    [SerializeField] private LayerMask layerMask = Physics.DefaultRaycastLayers;
    [SerializeField] private GameObject visualExplosionPrefab;
    [SerializeField] private float damage = 5f;

    private AudioSource audioSource;
    [SerializeField] private AudioClip explosionAudioClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, layerMask);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<HurtBox>(out HurtBox hb)) hb.NotifyHit(this, damage);

            if (collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
                rb.AddExplosionForce(force, transform.position, range, upwardsModifier);
        }

        audioSource.PlayOneShot(explosionAudioClip);
        Instantiate(visualExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject, 5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}