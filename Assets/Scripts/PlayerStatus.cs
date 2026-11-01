using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerStatus : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("UI Reference")]
    public HealthUI healthUI;
    public GameObject gameOverPanel;

    [Header("Knockback Settings")]
    public Vector2 knockbackForce = new Vector2(7f, 9f);
    public float knockbackDuration = 0.25f;

    [Header("Hit Flash Settings")]
    public float flashDuration = 0.18f;
    public Material whiteFlashMaterial;

    [Header("Respawn Settings")]
    public float respawnInvulnerableDuration = 1f;
    public float respawnFlashDelay = 0.75f;

    [Header("Death Settings")]
    public int deathBlinkCount = 5;
    public float deathBlinkInterval = 0.1f;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Material originalMaterial;
    private PlayerMovement playerMovement;
    private Collider2D playerCollider;

    private Vector3 startPosition;
    private bool isInvulnerable = false;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCollider = GetComponent<Collider2D>();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (healthUI != null)
        {
            healthUI.InitializeHearts(maxHealth);
        }

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

    private void Update()
    {
        if (isDead && Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void TakeDamage(int damage, Vector3 attackerPosition)
    {
        if (isInvulnerable || isDead) return;

        currentHealth -= damage;
        AudioManager.PlayPlayerDamage();

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(ApplyKnockback(attackerPosition));
        StartCoroutine(FlashWhite());
    }

    public void FallIntoVoid(int damage, Vector3? respawnPosition)
    {
        if (isDead) return;

        currentHealth -= damage;
        AudioManager.PlayPlayerDamage();

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        Vector3 target = respawnPosition ?? startPosition;
        Respawn(target);
    }

    private void Respawn(Vector3 position)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        transform.position = new Vector3(position.x, position.y, transform.position.z);

        if (playerMovement != null)
        {
            playerMovement.isKnockedBack = false;
        }

        StartCoroutine(RespawnInvulnerability());
        StartCoroutine(DelayedFlash());
    }

    private IEnumerator RespawnInvulnerability()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(respawnInvulnerableDuration);
        if (!isDead) isInvulnerable = false;
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

    private IEnumerator DelayedFlash()
    {
        yield return new WaitForSeconds(respawnFlashDelay);

        // Batalkan kilatan kalau player sudah mati selama menunggu
        if (isDead) yield break;

        yield return StartCoroutine(FlashWhite());
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

        if (sprite != null)
        {
            sprite.color = Color.red;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
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
