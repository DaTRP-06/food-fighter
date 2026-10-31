using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 12f;
    [SerializeField] private LayerMask jumpableGround;

    [HideInInspector] public bool isKnockedBack;
    public CollectibleManager cm;

    // attack variables
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackDuration = 0.3f;
    [SerializeField] private float spawnDelay = 0.02f;
    private float attackTimer = 0f;
    private float firePointOffsetX;
    private int shootDirection = 1;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;

    private enum MovementState { idle, running, jumping, falling }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (firePoint != null)
        {
            firePointOffsetX = Mathf.Abs(firePoint.localPosition.x);
        }
    }

    void Update()
    {
        if (isKnockedBack) return;

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float moveInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveInput -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveInput += 1f;
        }

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        FlipSprite(moveInput);

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
        }

        UpdateAnimationState(moveInput);

        if (Keyboard.current != null && Keyboard.current.zKey.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        anim.SetTrigger("attack");
        anim.SetInteger("state", 99);
        attackTimer = attackDuration;
        shootDirection = sprite.flipX ? -1 : 1;
        Invoke(nameof(SpawnFireball), spawnDelay);
    }

    private void SpawnFireball()
    {
        if (!enabled || projectilePrefab == null || firePoint == null) return;
        Vector3 localOffset = new Vector3(
            firePointOffsetX * shootDirection,
            firePoint.localPosition.y,
            firePoint.localPosition.z);
        Vector3 spawnPosition = transform.TransformPoint(localOffset);

        Quaternion spawnRotation = Quaternion.Euler(0f, shootDirection == -1 ? 180f : 0f, 0f);

        Instantiate(projectilePrefab, spawnPosition, spawnRotation);
    }

    private void UpdateAnimationState(float moveInput)
    {
        MovementState state;

        if (moveInput > 0f || moveInput < 0f)
        {
            state = MovementState.running;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.linearVelocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.linearVelocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }

    private void FlipSprite(float moveInput)
    {
        if (moveInput > 0f)
        {
            sprite.flipX = false;
        }
        else if (moveInput < 0f)
        {
            sprite.flipX = true;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(
            coll.bounds.center,
            coll.bounds.size,
            0f,
            Vector2.down,
            .1f,
            jumpableGround);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin") || other.gameObject.name.IndexOf("Coin", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            if (cm != null)
            {
                cm.points += 10;
            }
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Diamond") || other.CompareTag("Collectible") || other.gameObject.name.IndexOf("Diamond", System.StringComparison.OrdinalIgnoreCase) >= 0 || other.gameObject.name.IndexOf("Carrot", System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            if (cm != null)
            {
                cm.diamondCount++;
            }
            Destroy(other.gameObject);
        }
    }
}
