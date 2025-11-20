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

    private float nextFireTime;

    RaycastHit2D lastHit;  // for gizmos


    void Update()
    {
         AimAtPointer();
    }
    
    public void TryShoot()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (audioSource && shootSound)
            audioSource.PlayOneShot(shootSound);

        // Use layer mask properly
        lastHit = Physics2D.Raycast(firePoint.position, firePoint.right, range, hitLayers);

        if (lastHit.collider != null)
        {
            Debug.Log("Hit: " + lastHit.collider.name);

            if (hitEffect)
            {
                GameObject effect = Instantiate(hitEffect, lastHit.point, Quaternion.identity);
                Destroy(effect, 0.5f);
            }
        }
        else
        {
            Debug.Log("NO HIT");
        }
    }

    private void OnDrawGizmos()
    {
        if (firePoint == null) return;

        Gizmos.color = Color.red;

        // If hit something
        if (lastHit.collider != null)
        {
            Gizmos.DrawLine(firePoint.position, lastHit.point);
            Gizmos.DrawSphere(lastHit.point, 0.05f);
        }
        else
        {
            // Draw full ray range
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.right * range);
        }
    }


    void AimAtPointer()
{
    Vector3 pointerPos;

    // Mouse (PC)
    if (Input.mousePresent)
    {
        pointerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    // Touch (Mobile)
    else if (Input.touchCount > 0)
    {
        pointerPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
    }
    else
    {
        return; // nothing to aim at yet
    }

    pointerPos.z = 0;

    Vector2 direction = (pointerPos - transform.position).normalized;
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    transform.rotation = Quaternion.Euler(0, 0, angle);
}
}
