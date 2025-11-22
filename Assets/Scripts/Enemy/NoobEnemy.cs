using UnityEngine;
using System.Collections;

public class NoobEnemy : EnemyScript
{
    [Header("References")]
    public Transform playerpos;
    public GameObject shootObject;
    public Transform pointA;
    public Transform pointB;

    [Header("Settings")]
    public float playerJumpSpeed = 2;
    public float shootingRange = 5f;
    public float shootingCooldown = 1.5f;
    public float speed = 2f;
    public float waitTime = 2f;

    private bool goingToA = true;

    // ANIM -----------------------------
    private Animator anim;
    // ----------------------------------

    void Start()
    {
        // ANIM
        anim = GetComponent<Animator>();
        if (!anim)
        {
            Debug.Log("Animator not found");
        }

        if (playerpos == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p != null)
                playerpos = p.transform;
        }

        StartCoroutine(PatrolRoutine());
        StartCoroutine(ShootingRoutine());
    }

    IEnumerator ShootingRoutine()
    {
        while (true)
        {
            if (playerpos == null)
            {
                yield return new WaitForSeconds(shootingCooldown);
                continue;
            }

            float dist = Vector2.Distance(transform.position, playerpos.position);

            if (dist <= shootingRange)
            {
                ShootToPlayer();
            }

            yield return new WaitForSeconds(shootingCooldown);
        }
    }

    void ShootToPlayer()
    {
        if (shootObject == null) return;

        GameObject shoot = Instantiate(shootObject, transform.position, Quaternion.identity);
        Destroy(shoot, 4f);
    }

    IEnumerator PatrolRoutine()
    {
        while (true)
        {
            Vector3 target = goingToA ? pointA.position : pointB.position;

            // ANIM: start walking
            anim.SetBool("isWalking", true);

            while (Vector2.Distance(transform.position, target) > 0.05f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    target,
                    speed * Time.deltaTime
                );

                yield return null;
            }

            // Arrived → idle
            transform.position = target;
            anim.SetBool("isWalking", false); // ANIM: idle
            yield return new WaitForSeconds(waitTime);

            goingToA = !goingToA;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        player = collision.GetComponent<Player>()
              ?? collision.GetComponentInParent<Player>()
              ?? collision.GetComponentInChildren<Player>();

        if (!player) return;

        player.rb.linearVelocity = new Vector2(
            player.rb.linearVelocity.x,
            player.jumpForce * playerJumpSpeed
        );
    }
}
