using UnityEngine;

public class FunctionableObject : MonoBehaviour
{
    private Player playerRef;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player")
        {
            playerRef = other.GetComponentInParent<Player>();
            if (playerRef != null)
            {
                Debug.Log("Found");
                playerRef.TakeDamage(100);
            }
           
        }
    }
}
