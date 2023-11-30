#region

using System.Collections.Generic;
using UnityEngine;

#endregion

public class EntitySight : MonoBehaviour
{
    [SerializeField] private Vector3 sightSize = new Vector3(20f, 30f, 30f);
    [SerializeField] private LayerMask layerMaskVisibles = Physics.DefaultRaycastLayers;
    [SerializeField] private LayerMask layerMaskOccluders = Physics.DefaultRaycastLayers;
    [SerializeField] Vector2 sightAngle = new Vector2(60f, 45f);
    [SerializeField] float frequency = 0.5f;

    public List<IVisible> visiblesInSight = new();
    private float lastSightCheckTime = 0f;

    private void Start()
    {
        lastSightCheckTime = Time.time;
        lastSightCheckTime += Random.Range(0f, 1f / frequency);
    }

    private void Update()
    {
        if ((Time.time - lastSightCheckTime) > (1f / frequency))
        {
            lastSightCheckTime += (1f / frequency);

            visiblesInSight.Clear();
            Collider[] colliders = Physics.OverlapBox(
                transform.position + (transform.forward * sightSize.z / 2f),
                sightSize / 2f,
                transform.rotation,
                layerMaskVisibles);

            foreach (Collider collider in colliders)
            {
                Vector3 direction = collider.transform.position - transform.position;
                // float horizontalAngle = Vector3.SignedAngle(transform.forward, direction, transform.up);
                // float verticalAngle = Vector3.SignedAngle(transform.forward, direction, transform.right);
                // if ((Mathf.Abs(horizontalAngle) < sightAngle.x) && (Mathf.Abs(verticalAngle) < sightAngle.y))
                // {
                if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity,
                        layerMaskOccluders))
                {
                    if (hit.collider == collider)
                    {
                        IVisible visible = collider.GetComponent<IVisible>();
                        if (visible != null)
                        {
                            visiblesInSight.Add(visible);
                        }
                    }
                }
                // }
            }
        }
    }
}