using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : HealthComponent
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Gravity Settings")]
    public float normalGravity = 1f;   // gravity while rising or grounded
    public float fallGravity = 1.5f;     // stronger gravity when falling

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

   public UnityEngine.UI.Image healthBarFill;


    public Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // set initial gravity
        rb.gravityScale = normalGravity;
    }

    void Update()
    {
        if (isDead) return;

        healthBarFill.fillAmount = currentHealth / maxHealth;

        if (currentHealth < 1)
            KillPlayer();

        HandleMovement();
        HandleShooting();
        HandleGravity();
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

    void HandleGravity()
    {
        // Only apply stronger gravity AFTER peak (when velocity is negative)
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = fallGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
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
        Destroy(gameObject, 2f);
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

        Player player = GetComponent<Player>();
        if (player)
            player.KillPlayer();
    }
}
