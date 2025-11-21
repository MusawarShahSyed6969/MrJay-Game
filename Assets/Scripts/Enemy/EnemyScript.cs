using UnityEngine;
using UnityEngine.SceneManagement;


public class EnemyScript : HealthComponent
{

    // public float maxHealth = 100f;
    // public float currentHealth;

    public bool isDead = false;

    [Header("References")]
    public GameObject deathEffect;

    public Player player;

    HealthComponent health;


    private void Start()
    {


        health = GetComponent<HealthComponent>();
        // currentHealth = maxHealth;
    }

    private void Update()
    {

        if (isDead)
        {
            Destroy(this);
        }

        // Checkhealth();    
    }




    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!player)
            {
                Player player = other.gameObject.GetComponent<Player>();
            }

            if (player != null)
            {
                // player.KillPlayer();
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
