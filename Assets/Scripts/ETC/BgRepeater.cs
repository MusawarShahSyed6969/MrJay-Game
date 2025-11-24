using UnityEngine;

public class BgRepeater : MonoBehaviour
{
 [Header("References")]
    public Transform player; // player transform
    public float parallaxSpeed = 0.5f;

    private float spriteWidth; // width of one background piece
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;

        // Assuming the background has a SpriteRenderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            spriteWidth = sr.bounds.size.x;
    }

    void Update()
    {
        float deltaX = player.position.x * parallaxSpeed;
        transform.position = startPos + Vector3.right * deltaX;

        // Repeat background
        if (player.position.x > transform.position.x + spriteWidth)
        {
            startPos += Vector3.right * spriteWidth;
        }
        else if (player.position.x < transform.position.x - spriteWidth)
        {
            startPos -= Vector3.right * spriteWidth;
        }
    }
}
