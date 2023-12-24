#region

using UnityEngine;

#endregion

public class CombatZone : MonoBehaviour
{
    private Enemy[] enemies;

    private void Awake()
    {
        enemies = GetComponentsInChildren<Enemy>();
    }

    private void Start()
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.gameObject.SetActive(false);
        }
    }
}