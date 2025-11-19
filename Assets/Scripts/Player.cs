using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Death Settings")]
    public AudioClip deathSound;
    private AudioSource audioSource;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isDead) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }


    public void KillPlayer()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;

        // Play sound
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
            // float soundLength = deathSound.length;
            Invoke("RestartScene", 4f); // wait EXACTLY the sound length
        }
        else
        {
            RestartScene();
        }
    }

    void RestartScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
