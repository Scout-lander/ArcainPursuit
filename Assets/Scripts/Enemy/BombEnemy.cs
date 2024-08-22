using System.Collections;
using UnityEngine;

public class BombEnemy : MonoBehaviour
{
    protected bool ExplodingEnemy = true;
    private bool isExploding = false;
    public Color flashColor = new Color(1, 0, 0, 1);
    public ParticleSystem explodingEffect;

    [System.Serializable]
    public class Stats
    {
        public float movementSpeedIncrease = 3f, flashDuration = 0.2f, explosionDuration = 4f;
        public float explosionRadius, explosionStartDistance, explosionDamage;

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.movementSpeedIncrease += s2.movementSpeedIncrease;
            s1.flashDuration += s2.flashDuration;
            s1.explosionDuration += s2.explosionDuration;
            s1.explosionRadius += s2.explosionRadius;
            s1.explosionStartDistance += s2.explosionStartDistance;
            s1.explosionDamage += s2.explosionDamage;
            return s1;
        }
    }

    public Stats baseStats;
    [SerializeField] Stats actualStats;

    public Stats ActualStats
    {
        get { return actualStats; }
    }

    private Color originalColor;
    private SpriteRenderer sr;
    private Transform player;
    private EnemyStats enemystats;

    private int difficultyLevel;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        actualStats = baseStats;
        enemystats = GetComponent<EnemyStats>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Adjust enemy parameters based on difficulty level
        AdjustDifficultyLevel(DifficultyManager.Instance.CurrentDifficultyLevel);

        StartCoroutine(ExplodeCoroutine());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    private IEnumerator ExplodeCoroutine()
    {
        while (true)
        {
            yield return null;

            if (player != null && Vector2.Distance(transform.position, player.position) <= actualStats.explosionStartDistance)
            {
                isExploding = true;

                // Speed up the enemy's movement
                enemystats.ModifyActualStats(new EnemyStats.Stats { moveSpeed = actualStats.movementSpeedIncrease });
                enemystats.actualStats.moveSpeed = actualStats.movementSpeedIncrease;

                // Start continuous flashing
                StartCoroutine(FlashColor());

                // Wait for the explosion duration before exploding
                yield return new WaitForSeconds(actualStats.explosionDuration);

                // Explode
                Explode();

                // Destroy the enemy
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator FlashColor()
    {
        while (isExploding)
        {
            sr.color = flashColor;
            yield return new WaitForSeconds(actualStats.flashDuration / 2);
            sr.color = originalColor;
            yield return new WaitForSeconds(actualStats.flashDuration / 2);
        }
    }

    private void Explode()
    {
        if (explodingEffect != null)
        {
            Instantiate(explodingEffect, transform.position, Quaternion.identity);
        }

        // Deal damage to nearby objects within explosion radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, actualStats.explosionRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerStats>().TakeDamage(actualStats.explosionDamage);
            }
            else if (collider.CompareTag("Enemy"))
            {
                Destroy(collider.gameObject); // Destroy the enemy
            }
        }

        // Destroy the bomb enemy
        Destroy(gameObject);
    }

    public void ApplyDifficultyScaling(DifficultyManager.DifficultyStats difficultyStats)
    {
        actualStats += difficultyStats.bombEnemyStats;
    }

    public void AdjustDifficultyLevel(int difficultyLevel)
    {
        this.difficultyLevel = difficultyLevel;

        actualStats.explosionRadius = baseStats.explosionRadius * (1 + difficultyLevel * 0.2f);
        actualStats.explosionStartDistance = baseStats.explosionStartDistance * (1 + difficultyLevel * 0.1f);
        actualStats.explosionDamage = baseStats.explosionDamage * (1 + difficultyLevel * 0.3f);
    }
}
