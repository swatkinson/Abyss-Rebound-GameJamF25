using UnityEngine;
using System;
public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float jumpForce = 5f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    private float xPosLastFrame;

    [SerializeField] private Animator animator;

    public AudioClip jumpSound;
    public AudioClip landSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        flipCharacterX();

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            animator.SetBool("isJumping", !isGrounded);
            AudioManager.Instance.PlaySFX(jumpSound, 0.2f); // Jump SFX
        }

    }



    private void FixedUpdate()
    {
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Left/Right movement
        float moveInput = Input.GetAxis("Horizontal"); //  -1 or 1
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        animator.SetFloat("xVelocity", Math.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        if (moveInput != 0)
        {
            animator.SetBool("isRunning", true);
        }

        else
        {
            animator.SetBool("isRunning", false);
        }
    }
    
     private void flipCharacterX()
    {
        float moveInput = Input.GetAxis("Horizontal"); //  -1 or 1
        
        if (moveInput < 0 && (transform.position.x < xPosLastFrame))
        {
            spriteRenderer.flipX = true;
        }

        else if (moveInput > 0 && (transform.position.x > xPosLastFrame))
        {
            spriteRenderer.flipX = false;
        }

        xPosLastFrame = transform.position.x;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isGrounded = true;
        animator.SetBool("isJumping", !isGrounded);

        AudioManager.Instance.PlaySFX(landSound, 0.1f); // Landed SFX
    }
}
