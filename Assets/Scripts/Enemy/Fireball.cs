using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 5f;          // fireball movement speed
    public float lifetime = 4f;       // destroy after seconds
    private Vector2 direction;        // direction toward player

    void Start()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Vector2 targetPos = player.transform.position;
            direction = (targetPos - (Vector2)transform.position).normalized;

            Debug.Log("Fireball direction set toward player: " + direction);
        }
        else
        {
            Debug.LogError("ERROR: No Player found - Fireball cannot aim!");
            direction = Vector2.right; // default fallback
        }

        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }
}
