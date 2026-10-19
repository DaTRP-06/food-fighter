using UnityEngine;

public class BurgerEnemy : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1;

    [Header("Movement")]
    public float moveSpeed = 2f;
    
    [Header("Wall & Pit Detection")]
    public Transform wallCheck;
    public Transform pitCheck;
    public float checkRadius = 0.2f;
    public float pitCheckDistance = 0.5f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool movingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Gerak horizontal (Unity 6 menggunakan linearVelocity)
        float direction = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        CheckObstacles();
    }

    void CheckObstacles()
    {
        // 1. Cek Benturan Dinding
        bool isHittingWall = false;
        if (wallCheck != null)
        {
            isHittingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);
        }

        // 2. Cek Deteksi Jurang (Raycast ke bawah dari PitCheck)
        bool isNearPit = false;
        if (pitCheck != null)
        {
            RaycastHit2D groundInfo = Physics2D.Raycast(pitCheck.position, Vector2.down, pitCheckDistance, groundLayer);
            isNearPit = groundInfo.collider == null;
        }

        // Balik arah jika menabrak dinding ATAU berada di pinggir jurang
        if (isHittingWall || isNearPit)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStatus player = collision.gameObject.GetComponent<PlayerStatus>();
            if (player != null)
            {
                player.TakeDamage(damageAmount, transform.position);

                Flip();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        }

        if (pitCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(pitCheck.position, Vector2.down * pitCheckDistance);
        }
    }
}