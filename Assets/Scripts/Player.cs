using UnityEngine;
using System;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float jumpForce = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Rendering / Animation")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;

    public bool isOnIce;

    [SerializeField] private float jumpCooldown = 0.12f;
    private float lastJumpTime = -999f;
    private bool airborneJumpConsumed = false;

    private bool keyboardEnabled = false;

    // Input mixing (keyboard OR one-shot gesture nudge)
    private float moveInput = 0f;
    private float keyboardInput = 0f;
    private Coroutine resetCoroutine;

    // Runtime
    private Rigidbody2D rb;
    private bool grounded;         // current
    private bool groundedPrev;     // previous (to detect land)

    // Tunables
    private const float flipDeadzone = 0.05f;
    private const float fallThreshold = -0.1f; // helps pick Fall vs Jump near 0

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponent<Animator>();
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            var s = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            UnityEngine.SceneManagement.SceneManager.LoadScene(s.name);
        }

        // enable keyboard
        if (Input.GetKeyDown(KeyCode.K))
            keyboardEnabled = true;

        // keyboard jump
        if (Input.GetKeyDown(KeyCode.Space) && keyboardEnabled)
            Jump();

        // keyboard horizontal
        if (keyboardEnabled)
            keyboardInput = Input.GetAxisRaw("Horizontal");

        // flip logic off current horizontal velocity
        float vx = rb.linearVelocity.x;
        if (vx > flipDeadzone) spriteRenderer.flipX = false;
        else if (vx < -flipDeadzone) spriteRenderer.flipX = true;

        // animator parameters (set every frame)
        animator.SetFloat("Speed", Mathf.Abs(vx));
        animator.SetFloat("Y", rb.linearVelocity.y);
        animator.SetBool("Grounded", grounded);
    }

    private void FixedUpdate()
    {
        // ground probe
        groundedPrev = grounded;
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (grounded) airborneJumpConsumed = false;

        // landing SFX (only when transitioning from air -> ground)
        if (!groundedPrev && grounded && landSound)
            AudioManager.Instance.PlaySFX(landSound, 0.4f);

        // pick input source: gesture nudge takes priority for its brief window
        float input = (Mathf.Abs(moveInput) > 0.001f) ? moveInput : keyboardInput;

        // move
        rb.linearVelocity = new Vector2(input * moveSpeed, rb.linearVelocity.y);
    }

    public bool IsGrounded() => grounded;

    public void Jump()
    {
        if (!grounded) return;                              // must be on ground
        if (airborneJumpConsumed) return;                   // already jumped this airtime
        if (Time.time - lastJumpTime < jumpCooldown) return; // cooldown guard

        lastJumpTime = Time.time;
        airborneJumpConsumed = true;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        grounded = false; // optimistic; FixedUpdate will confirm
        animator.SetBool("Grounded", false);
        if (jumpSound) AudioManager.Instance.PlaySFX(jumpSound, 0.6f);
    }


    // Gesture hooks (one-shot nudges with auto-reset)
    public void MoveLeft()
    {
        moveInput = -1f;
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(ResetMoveInputAfterDelay(0.30f));
    }

    public void MoveRight()
    {
        moveInput = 1f;
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(ResetMoveInputAfterDelay(0.30f));
    }

    private IEnumerator ResetMoveInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        moveInput = 0f;
        resetCoroutine = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
#endif
}
