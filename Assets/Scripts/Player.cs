using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : HealthComponent
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Death")]
    public AudioClip deathSound;
    private AudioSource audioSource;

    [Header("References")]
    public Transform spriteTransform; // child sprite to flip
    public GunScript equippedGun;

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

        HandleMovement();
        HandleShooting();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void HandleMovement()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Flip sprite + tell gun to flip
        if (moveInput > 0)
        {
            spriteTransform.localScale = new Vector3(1, 1, 1);
            if (equippedGun) equippedGun.SetFacing(true);
        }
        else if (moveInput < 0)
        {
            spriteTransform.localScale = new Vector3(-1, 1, 1);
            if (equippedGun) equippedGun.SetFacing(false);
        }

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void HandleShooting()
    {
        bool shoot = false;

        // PC
        if (Input.mousePresent && Input.GetMouseButton(0))
            shoot = true;

        // Mobile
        else if (Input.touchCount > 0 &&
                 (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved))
            shoot = true;

        if (shoot && equippedGun != null)
            equippedGun.TryShoot();
    }

    public void KillPlayer()
    {
        if (isDead) return;
        isDead = true;
        rb.linearVelocity = Vector2.zero;

        if (deathSound && audioSource)
        {
            audioSource.PlayOneShot(deathSound);
            Invoke("RestartScene", 4f);
        }
        else
        {
            RestartScene();
        }
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    protected override void Die()
    {
        Debug.Log("Player died!");

        // Example: disable controls
        Player player = GetComponent<Player>();
        if (player)
            player.KillPlayer();

        // Or restart level
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
