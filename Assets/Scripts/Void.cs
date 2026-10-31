using UnityEngine;

public class Void : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    [Header("Damage")]
    [SerializeField] private int damageAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus player = other.GetComponent<PlayerStatus>();
            if (player != null)
            {
                // Jika respawnPoint kosong, PlayerStatus memakai posisi awal player
                Vector3? target = respawnPoint != null ? respawnPoint.position : (Vector3?)null;
                player.FallIntoVoid(damageAmount, target);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (respawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(respawnPoint.position, 0.4f);
            Gizmos.DrawLine(transform.position, respawnPoint.position);
        }
    }
}
