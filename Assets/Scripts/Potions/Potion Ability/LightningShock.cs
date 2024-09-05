using UnityEngine;

public class LightningShock : MonoBehaviour
{
    private float duration;
    private float range;

    public void Activate(PlayerStats playerStats, float effectDuration, float shockRange)
    {
        duration = effectDuration;
        range = shockRange;

        // Continuously shock enemies within range
        InvokeRepeating(nameof(ShockEnemies), 0, 0.5f); // Shock every 0.5 seconds
        Invoke(nameof(DestroyEffect), duration); // Destroy after duration
    }

    private void ShockEnemies()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, range);
        foreach (var enemyCollider in enemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                EnemyStats enemy = enemyCollider.GetComponent<EnemyStats>();
                if (enemy != null)
                {
                    enemy.TakeDamage(10); // Adjust damage as needed
                    // Additional effects like stun or slow can be added here
                }
            }
        }
    }

    private void DestroyEffect()
    {
        CancelInvoke(nameof(ShockEnemies));
        Destroy(gameObject);
    }
}
