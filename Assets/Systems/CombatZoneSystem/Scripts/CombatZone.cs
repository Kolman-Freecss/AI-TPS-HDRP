#region

using UnityEngine;

#endregion

public class CombatZone : MonoBehaviour
{
    private Enemy[] enemies;

    private bool triggered;

    private void Awake()
    {
        enemies = GetComponentsInChildren<Enemy>();
    }

    private void Start()
    {
        triggered = false;
        foreach (Enemy enemy in enemies) enemy.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        foreach (Enemy enemy in enemies)
            enemy.gameObject.SetActive(true);
        triggered = true;
    }
}