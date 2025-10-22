using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Footsteps : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private AudioClip[] footstepClips;

    [Header("Tuning")]
    [SerializeField] private float stepDistance = 0.7f; // meters per step at walk
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;
    [SerializeField] private float volume = 0.15f;

    [Header("Movement/Grounding")]
    [SerializeField] private Rigidbody2D rb;            // assign in Inspector
    [SerializeField] private LayerMask groundMask;      // set your ground layer
    [SerializeField] private float groundCheckRadius = 0.05f;
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.6f);

    private AudioSource source;
    private Vector2 lastPos;
    private float distanceAccum;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; // 2D sound
        lastPos = transform.position;
    }

    void Update()
    {
        bool grounded = Physics2D.OverlapCircle((Vector2)transform.position + groundCheckOffset, groundCheckRadius, groundMask);
        float horizontalSpeed = Mathf.Abs(rb != null ? rb.linearVelocity.x : 0f);

        // Only count distance when moving on ground
        if (grounded && horizontalSpeed > 0.05f)
        {
            distanceAccum += Vector2.Distance(transform.position, lastPos);

            if (distanceAccum >= stepDistance)
            {
                PlayFootstep();
                distanceAccum = 0f;
            }
        }
        else
        {
            // reset so we don't instantly step when landing
            distanceAccum = Mathf.Min(distanceAccum, stepDistance * 0.5f);
        }

        lastPos = transform.position;
    }

    private void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;

        var clip = footstepClips[Random.Range(0, footstepClips.Length)];
        source.pitch = Random.Range(minPitch, maxPitch);
        source.PlayOneShot(clip, volume);
    }

    // optional: visualize ground check in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + groundCheckOffset, groundCheckRadius);
    }
}
