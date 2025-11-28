using UnityEngine;

public class AdvanceSpike : MonoBehaviour
{
    [Header("References")]
    private Player playerRef;
    public GameObject[] enemies;
    public Transform spikeTrans;
    public int enemyNumber = 3;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerRef = other.GetComponentInParent<Player>();
            if (playerRef != null)
            {
                int aliveCount = 0;
                foreach (GameObject enemy in enemies)
                {
                    if (enemy != null && enemy.activeInHierarchy)
                        aliveCount++;
                }

                if (aliveCount >= enemyNumber)
                {
                    spikeTrans.localScale = new Vector3(1, 3, 1);
                    spikeTrans.localPosition = new Vector3(20, -1, 0);
                      playerRef.TakeDamage(100);
                }
                
               
            }
            else
            {

            }
        }
    }
}
