using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class BreakingPlatform : MonoBehaviour
{
    [Header("Timings")]
    [SerializeField] private float disappearDelay = 1.0f;   // Time before platform disappears after touch
    [SerializeField] private float reappearDelay = 2.0f;    // Time before it reappears again

    [Header("Components")]
    [SerializeField] private Collider2D platformCollider;
    [SerializeField] private SpriteRenderer platformRenderer;

    private bool isDisappearing = false;

    void Reset()
    {
        // Auto-assign components if missing
        platformCollider = GetComponent<Collider2D>();
        platformRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDisappearing && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DisappearAndReappear());
        }
    }

    private IEnumerator DisappearAndReappear()
    {
        isDisappearing = true;

        yield return new WaitForSeconds(disappearDelay);

        // Disable platform
        platformCollider.enabled = false;
        platformRenderer.enabled = false;

        yield return new WaitForSeconds(reappearDelay);

        // Re-enable platform
        platformCollider.enabled = true;
        platformRenderer.enabled = true;

        isDisappearing = false;
    }
}

