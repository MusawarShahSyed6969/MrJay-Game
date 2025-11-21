using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class EnemyScript : HealthComponent
{

    // public float maxHealth = 100f;
    // public float currentHealth;

    public bool isDead = false;

    [Header("References")]
        public GameObject deathEffect;

    HealthComponent health;
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float waitTime = 2f;

    private bool goingToA = true;

    private void Start()
    {

        StartCoroutine(PatrolRoutine());
        health = GetComponent<HealthComponent>();
        // currentHealth = maxHealth;
    }

     private void Update() {

        if(isDead)
        {
            Destroy(this);
        }

        // Checkhealth();    
    }

    IEnumerator PatrolRoutine()
    {
        while (true)
        {
            Vector3 target = goingToA ? pointA.position : pointB.position;

            // Move toward target
            while (Vector2.Distance(transform.position, target) > 0.05f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    target,
                    speed * Time.deltaTime
                );
                yield return null;
            }

            // Snap to exact position
            transform.position = target;

            // Wait at the point
            yield return new WaitForSeconds(waitTime);

            // Switch direction
            goingToA = !goingToA;
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();

            if (player != null)
            {
                player.KillPlayer();
            }
        }
    }

       protected override void Die()
    {
        Debug.Log("Enemy died!");

        if (deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
