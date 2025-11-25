using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : HealthComponent
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Gravity Settings")]
    public float normalGravity = 1f;
    public float fallGravity = 1.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Death")]
    public AudioClip deathSound;
    private AudioSource audioSource;

    [Header("References")]
    public Transform BodyTransform;
    public Transform GunTransform;
    public GunScript equippedGun;
    public Image healthBarFill;
    private Animator anim;

    public Rigidbody2D rb;

    [Header("State")]
    public bool aimingUp = false;
    public bool facingRight = true;

    private float moveInput;
    private bool isDead = false;
    public bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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

        // LEFT/RIGHT movement + flipping
        if (moveInput > 0)
        {
            facingRight = true;
            anim.SetBool("isWalking",true);
             anim.SetBool("isMovingRight",true);
            BodyTransform.localRotation = Quaternion.Euler(0, 0, 0);
            equippedGun.SetFacing(true);
        }
        else if (moveInput < 0)
        {
            anim.SetBool("isWalking",true);
            anim.SetBool("isMovingRight",false);
            facingRight = false;
            BodyTransform.localRotation = Quaternion.Euler(0, 180, 0);
            equippedGun.SetFacing(false);
        }
        else
        {
             anim.SetBool("isWalking",false);
                        //   anim.SetBool("isMovingRight",true);

        }

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // AIM UP
        aimingUp = Input.GetKey(KeyCode.W);

        GunTransform.localRotation = aimingUp ? Quaternion.Euler(0, 0, 90) : Quaternion.Euler(0, 0, 0);
    }

    void HandleGravity()
    {
        rb.gravityScale = rb.linearVelocity.y < 0 ? fallGravity : normalGravity;
    }

    void HandleShooting()
    {
        bool shoot = false;

        if (Input.mousePresent && Input.GetMouseButton(0))
            shoot = true;
        else if (Input.touchCount > 0 &&
                (Input.GetTouch(0).phase == TouchPhase.Began ||
                 Input.GetTouch(0).phase == TouchPhase.Moved))
            shoot = true;

        if (shoot)
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

        Destroy(gameObject, 2f);
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    protected override void Die()
    {
        KillPlayer();
    }
}
