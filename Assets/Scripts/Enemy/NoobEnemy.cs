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
    public bool isWalkable = false;
    public float playerJumpSpeed = 2;
    public float shootingRange = 5f;
    public float shootingCooldown = 1.5f;
    public float speed = 2f;
    public float waitTime = 2f;

    public float DamageAmount = 15f;

    [Header("Continuous Damage")]
    public float damageInterval = 2f;   // Damage every 2 seconds after first hit

    private bool goingToA = true;
    private Animator anim;

    // Damage system
    private Player playerRef;
    private float damageTimer = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (!anim)
            Debug.LogWarning("Animator not found!");

        if (playerpos == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                playerpos = p.transform;
        }

        StartCoroutine(PatrolRoutine());
        StartCoroutine(ShootingRoutine());
    }

    // ---------------------- HELPER: FIND PLAYER ---------------------
    private Player GetPlayerFromCollider(Component col)
    {
        return col.GetComponent<Player>()
            ?? col.GetComponentInParent<Player>()
            ?? col.GetComponentInChildren<Player>();
    }

    // ------------------------- SHOOTING ------------------------------
    IEnumerator ShootingRoutine()
{
    // Initial delay before shooting starts
    yield return new WaitForSeconds(2f);

    while (true)
    {
        if (playerpos != null)
        {
            float dist = Vector2.Distance(transform.position, playerpos.position);

            if (dist <= shootingRange)
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

    // ------------------------- PATROL -------------------------------
    IEnumerator PatrolRoutine()
    {
        if(isWalkable)
        {
            while (true)
        {
            Vector3 target = goingToA ? pointA.position : pointB.position;

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

            transform.position = target;
            anim.SetBool("isWalking", false);

            yield return new WaitForSeconds(waitTime);

            goingToA = !goingToA;
        }
        }
    }

    // -------------------- DAMAGE ON COLLISION -----------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // Find the real Player script on parent
        playerRef = GetPlayerFromCollider(collision.collider);

        if (playerRef != null)
        {
            // FIRST INSTANT DAMAGE
            playerRef.TakeDamage(DamageAmount);

            // Reset cooldown timer for next hits
            damageTimer = 0f;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (playerRef == null) return;

        damageTimer += Time.deltaTime;

        // Damage after cooldown
        if (damageTimer >= damageInterval)
        {
            playerRef.TakeDamage(DamageAmount);
            damageTimer = 0f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRef = null;
            damageTimer = 0f; // reset timer
        }
    }

    // ------------------ KNOCK-UP TRIGGER ----------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player player = GetPlayerFromCollider(collision);
        if (!player) return;

        player.rb.linearVelocity = new Vector2(
            player.rb.linearVelocity.x,
            player.jumpForce * playerJumpSpeed
        );
    }
}
