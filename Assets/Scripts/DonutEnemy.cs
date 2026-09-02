using UnityEngine;

public class DonutEnemy : MonoBehaviour
{
    [Header("Movement Speed")]
    public float patrolSpeed = 4f;
    public float chargeSpeed = 8f;

    [Header("Target & Detection Settings")]
    public float yTolerance = 0.6f;
    public float detectionDistance = 8f;
    
    [Header("Wall & Pit Detection")]
    public Transform wallCheck;
    public Transform pitCheck;
    public float checkRadius = 0.2f;
    public float pitCheckDistance = 0.5f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Transform playerTransform;
    private bool movingRight = true;
    private bool isCharging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        DetectPlayer();
        Move();
        CheckObstacles();
    }

    void DetectPlayer()
    {
        if (playerTransform == null) return;

        bool isSameHeight = Mathf.Abs(transform.position.y - playerTransform.position.y) <= yTolerance;
        float xDistance = playerTransform.position.x - transform.position.x;
        bool isInRange = Mathf.Abs(xDistance) <= detectionDistance;

        if (isSameHeight && isInRange)
        {
            isCharging = true;

            if (xDistance > 0 && !movingRight)
            {
                Flip();
            }
            else if (xDistance < 0 && movingRight)
            {
                Flip();
            }
        }
        else
        {
            isCharging = false;
        }
    }

    void Move()
    {
        float speed = isCharging ? chargeSpeed : patrolSpeed;
        float dir = movingRight ? 1f : -1f;
        
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
    }

    void CheckObstacles()
    {
        // 1. Cek Dinding
        bool isHittingWall = false;
        if (wallCheck != null)
        {
            isHittingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);
        }

        // 2. Cek Jurang
        bool isNearPit = false;
        if (pitCheck != null)
        {
            RaycastHit2D groundInfo = Physics2D.Raycast(pitCheck.position, Vector2.down, pitCheckDistance, groundLayer);
            isNearPit = groundInfo.collider == null;
        }

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

    private void OnDrawGizmosSelected()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        }

        if (pitCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(pitCheck.position, Vector2.down * pitCheckDistance);
        }
    }
}