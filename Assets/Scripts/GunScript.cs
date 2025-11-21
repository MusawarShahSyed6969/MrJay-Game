using UnityEngine;

public class GunScript : MonoBehaviour
{
    [Header("Gun Settings")]
    public float fireRate = 0.2f;
    public float damage = 20f;
    public float range = 10f;

    [Header("References")]
    public Transform firePoint;
    public LayerMask hitLayers;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public GameObject hitEffect;
    public Transform GunPos;

    HealthComponent health;

    private float nextFireTime;

    // Called by Player script
    public void SetFacing(bool facingRight)
    {
        if (facingRight)
            GunPos.localRotation = Quaternion.Euler(0, 0, 0);   // face right
        else
        {
            GunPos.localRotation = Quaternion.Euler(0, 180, 0); // face left
            Debug.Log("Left Mode");
        }
    }

    public void TryShoot()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        if (audioSource && shootSound)
            audioSource.PlayOneShot(shootSound);

        // Raycast direction
        Vector2 dir = GunPos.right;

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, dir, range, hitLayers);

        if (hit.collider != null)
        {
            Debug.Log("Hit: " + hit.collider.name);

            if (hitEffect)
            {
                GameObject effect = Instantiate(hitEffect, hit.point, Quaternion.identity);
                Destroy(effect, 0.5f);
            }

              // OOP magic: works with ANY damageable object
            IDamageable dmg = hit.collider.GetComponent<IDamageable>();
            if (dmg != null)
                dmg.TakeDamage(damage);

        }
    }

    // Gizmo preview in Scene View
    void OnDrawGizmos()
    {
        if (firePoint == null) return;


        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(firePoint.position, firePoint.position + GunPos.right * range);
    }
}
