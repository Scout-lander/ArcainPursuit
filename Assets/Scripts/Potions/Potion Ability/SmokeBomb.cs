using UnityEngine;

public class SmokeBomb : MonoBehaviour
{
    private float duration;

    public void Activate(PlayerStats playerStats, float effectDuration)
    {
        duration = effectDuration;
        // Logic to blind enemies within the effect
        // You may want to use a collider or trigger to detect enemies in range

        Invoke(nameof(DestroyEffect), duration); // Destroy after duration
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Apply blinding effect to the enemy
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                //enemy.Blind(duration); // Assuming the Enemy script has a Blind method
            }
        }
    }

    private void DestroyEffect()
    {
        Destroy(gameObject);
    }
}
