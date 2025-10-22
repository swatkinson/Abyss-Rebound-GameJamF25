using UnityEngine;

public class FollowPlayerDownLatched : MonoBehaviour
{
    [Header("References")]
    public Transform player;              // Player Transform
    public LayerMask groundLayer;         // Ground layer

    [Header("Movement Settings")]
    public float moveSpeed = 5f;          // Downward move speed
    public float yOffset = 0f;            // Final offset from player's Y
    public float maxDropDistance = 10f;   // If player is more than this below, don't move
    public float stopEpsilon = 0.005f;    // Precision for stopping

    [Header("Ground Check")]
    public float groundCheckRadius = 0.1f;
    public Vector2 groundCheckOffset = new Vector2(0f, -1f);

    private bool loweringActive = false;
    private float targetY;

    void Update()
    {
        if (!player) return;

        // Check if player is grounded
        bool isGrounded = Physics2D.OverlapCircle(
            player.position + (Vector3)groundCheckOffset,
            groundCheckRadius,
            groundLayer
        );

        // If player is grounded, record new target if valid
        if (isGrounded)
        {
            float candidateY = player.position.y + yOffset;

            // Only latch if candidate is below current position
            // and within allowed drop distance
            if (candidateY < transform.position.y &&
                transform.position.y - candidateY <= maxDropDistance &&
                (!loweringActive || candidateY < targetY))
            {
                targetY = candidateY;
                loweringActive = true;
            }
        }

        // Move down if we’re lowering
        if (loweringActive)
        {
            float newY = Mathf.MoveTowards(transform.position.y, targetY, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            if (Mathf.Abs(transform.position.y - targetY) <= stopEpsilon)
            {
                transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
                loweringActive = false;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!player) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position + (Vector3)groundCheckOffset, groundCheckRadius);
    }
}
