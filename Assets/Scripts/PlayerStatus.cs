using System.Collections;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Knockback Settings")]
    public Vector2 knockbackForce = new Vector2(7f, 9f);
    public float knockbackDuration = 0.25f;

    [Header("Hit Flash Settings")]
    public float flashDuration = 0.15f;
    public Material whiteFlashMaterial;

    [Header("Death Settings")]
    public int deathBlinkCount = 5;
    public float deathBlinkInterval = 0.1f;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Material originalMaterial;
    private PlayerMovement playerMovement;
    private Collider2D playerCollider;

    private bool isInvulnerable = false;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCollider = GetComponent<Collider2D>();

        if (sprite != null)
        {
            originalMaterial = sprite.material;
        }

        if (whiteFlashMaterial == null)
        {
            Shader guiShader = Shader.Find("GUI/Text Shader");
            if (guiShader != null)
            {
                whiteFlashMaterial = new Material(guiShader);
            }
        }
    }

    public void TakeDamage(int damage, Vector3 attackerPosition)
    {
        if (isInvulnerable || isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(ApplyKnockback(attackerPosition));
        StartCoroutine(FlashWhite());
    }

    private IEnumerator ApplyKnockback(Vector3 attackerPosition)
    {
        isInvulnerable = true;

        if (playerMovement != null)
            playerMovement.isKnockedBack = true;

        float dirX = transform.position.x < attackerPosition.x ? -1f : 1f;
        rb.linearVelocity = new Vector2(dirX * knockbackForce.x, knockbackForce.y);

        yield return new WaitForSeconds(knockbackDuration);

        if (playerMovement != null && !isDead)
            playerMovement.isKnockedBack = false;

        isInvulnerable = false;
    }

    private IEnumerator FlashWhite()
    {
        if (sprite != null)
        {
            if (whiteFlashMaterial != null)
            {
                sprite.material = whiteFlashMaterial;
                yield return new WaitForSeconds(flashDuration);
                sprite.material = originalMaterial;
            }
            else
            {
                Color originalColor = sprite.color;
                sprite.color = Color.red;
                yield return new WaitForSeconds(flashDuration);
                sprite.color = originalColor;
            }
        }
    }

    private void Die()
    {
        isDead = true;
        isInvulnerable = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        StartCoroutine(DieSequence());
    }

    private IEnumerator DieSequence()
    {
        for (int i = 0; i < deathBlinkCount; i++)
        {
            if (sprite != null) sprite.color = Color.red;
            yield return new WaitForSeconds(deathBlinkInterval);

            if (sprite != null) sprite.color = Color.clear;
            yield return new WaitForSeconds(deathBlinkInterval);
        }

        Destroy(gameObject);
    }
}