using UnityEngine;

public class GunScript : MonoBehaviour
{
    [Header("Gun Settings")]
    public float fireRate = 0.2f;
    public float damage = 20f;
    public float range = 10f;

    [Header("References")]
    public Player player;

    public Transform firePoint;
    public LayerMask hitLayers;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public GameObject hitEffect;
    public GameObject FireEffect;
    public Transform GunPos; // Used for visual flipping only

    private float nextFireTime;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    // Visual flip only
    public void SetFacing(bool right)
    {
        GunPos.localScale = new Vector3(right ? 1 : -1, 1, 1);
    }

    public void TryShoot()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        if (audioSource && shootSound)
            audioSource.PlayOneShot(shootSound);

        Vector2 dir;

        // Determine shooting direction
        if (player.aimingUp)
        {
            dir = Vector2.up;
        }
        else
        {
            dir = player.facingRight ? Vector2.right : Vector2.left;
        }

        // Fire the ray
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, dir, range, hitLayers);

        // Muzzle flash
        if (FireEffect)
        {
            var fx = Instantiate(FireEffect, firePoint.position, Quaternion.identity);
            Destroy(fx, 0.5f);
        }

        // Hit effect & damage
        if (hit.collider != null)
        {
            if (hitEffect)
            {
                var fx2 = Instantiate(hitEffect, hit.point, Quaternion.identity);
                Destroy(fx2, 0.5f);
            }

            IDamageable dmg = hit.collider.GetComponent<IDamageable>();
            if (dmg != null)
                dmg.TakeDamage(damage);
        }
    }

    void OnDrawGizmos()
    {
        if (!firePoint) return;

        Gizmos.color = Color.yellow;
        Vector3 dir = player != null && player.aimingUp ? Vector3.up : (player != null && player.facingRight ? Vector3.right : Vector3.left);
        Gizmos.DrawLine(firePoint.position, firePoint.position + dir * range);
    }
}
