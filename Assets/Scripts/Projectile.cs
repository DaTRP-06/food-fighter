using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
        
        Destroy(gameObject, 3f); 
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Enemy"))
        {
            AudioManager.PlayFireballHit();
            AudioManager.PlayEnemyDeath();
            Destroy(hitInfo.gameObject);

            Destroy(gameObject);
            // can probably add a wall gameObject later after making tilesets.
        }
    }
}