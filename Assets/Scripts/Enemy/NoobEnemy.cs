using UnityEngine;
using System.Collections;

public class NoobEnemy : EnemyScript
{
    [Header("References")]
    public Transform playerpos;            // Player transform
    public GameObject shootObject;
    public Transform pointA;
    public Transform pointB;

    [Header("Settings")]
    public float shootingRange = 5f;
    public float shootingCooldown = 1.5f;
    public float speed = 2f;
    public float waitTime = 2f;

    private bool goingToA = true;

    void Start()
    {
        // Debug: check for player
        if (playerpos == null)
        {

            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p != null)
            {
                playerpos = p.transform;
               
            }
           
        }
      

        // Start routines
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
        if (shootObject == null)
        {

            return;
        }

        GameObject shoot = Instantiate(shootObject, transform.position, Quaternion.identity);
        Destroy(shoot, 4f);
        
    }

    IEnumerator PatrolRoutine()
    {
        while (true)
        {
            Vector3 target = goingToA ? pointA.position : pointB.position;
           

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

    if (!player)
    {
      
        return;
    }

  

    player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, player.jumpForce * 3);
}

}
