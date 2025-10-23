using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class CrumblingPlatform : MonoBehaviour
{
    [Header("Triggering")]
    [SerializeField] private string playerTag = "Player";
    [Tooltip("Only crumble when the player is on top (not from the side/below).")]
    [SerializeField] private bool requireTopContact = true;

    [Header("Timing")]
    [Tooltip("Seconds after the first valid player step before the platform disappears.")]
    [SerializeField] private float disappearDelay = 1.0f;
    [Tooltip("How long the visual shake lasts at the end of the delay.")]
    [SerializeField] private float shakeDuration = 0.25f;

    [Header("Shake")]
    [Tooltip("Maximum shake offset in local units (x/y).")]
    [SerializeField] private Vector2 shakeAmplitude = new Vector2(0.05f, 0.05f);
    [Tooltip("How fast the shake jitters.")]
    [SerializeField] private float shakeFrequency = 40f;

    [Header("After Disappear")]
    [Tooltip("Optional: destroy the whole platform object after collapsing.")]
    [SerializeField] private bool destroyAfterCollapse = false;
    [SerializeField] private float destroyDelay = 1.5f;

    private Collider2D _collider;
    private bool _triggered;
    private readonly List<Transform> _visuals = new();
    private readonly List<Vector3> _originalLocalPos = new();

    void Awake()
    {
        _collider = GetComponent<Collider2D>();

        // Collect all visual children under Container (exclude the Container itself).
        foreach (Transform child in GetComponentsInChildren<Transform>(includeInactive: true))
        {
            if (child == transform) continue;
            _visuals.Add(child);
            _originalLocalPos.Add(child.localPosition);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (_triggered) return;
        if (!col.collider.CompareTag(playerTag)) return;

        if (requireTopContact)
        {
            // Only trigger if any contact normal suggests the other is on top of us.
            // For the platform's collider, a top contact typically has normal.y > 0.5.
            bool top = false;
            foreach (var cp in col.contacts)
            {
                if (cp.normal.y > 0.5f) { top = true; break; }
            }
            if (!top) return;
        }

        StartCoroutine(CrumbleRoutine());
    }

    private IEnumerator CrumbleRoutine()
    {
        _triggered = true;

        // Wait until it's time to shake (we shake for shakeDuration right before disappearing).
        float waitBeforeShake = Mathf.Max(0f, disappearDelay - shakeDuration);
        if (waitBeforeShake > 0f)
            yield return new WaitForSeconds(waitBeforeShake);

        // Shake visuals without moving the collider.
        yield return StartCoroutine(DoShake(shakeDuration));

        // Restore visual positions, then disappear: disable collider + hide visuals.
        RestoreVisualPositions();
        if (_collider) _collider.enabled = false;
        SetVisualsActive(false);

        if (destroyAfterCollapse)
            Destroy(gameObject, destroyDelay);
    }

    private IEnumerator DoShake(float duration)
    {
        float t = 0f;
        // Cache seeds so each child jitters uniquely.
        var seeds = new float[_visuals.Count];
        for (int i = 0; i < seeds.Length; i++)
            seeds[i] = Random.value * 10f;

        while (t < duration)
        {
            float s = t * shakeFrequency;
            for (int i = 0; i < _visuals.Count; i++)
            {
                if (_visuals[i] == null) continue;
                // Simple fast jitter: sin/cos with a per-child seed.
                float ox = Mathf.Sin(s + seeds[i]) * shakeAmplitude.x;
                float oy = Mathf.Cos(s * 1.13f + seeds[i]) * shakeAmplitude.y;
                _visuals[i].localPosition = _originalLocalPos[i] + new Vector3(ox, oy, 0f);
            }
            t += Time.deltaTime;
            yield return null;
        }
    }

    private void RestoreVisualPositions()
    {
        for (int i = 0; i < _visuals.Count; i++)
        {
            if (_visuals[i] == null) continue;
            _visuals[i].localPosition = _originalLocalPos[i];
        }
    }

    private void SetVisualsActive(bool active)
    {
        foreach (var t in _visuals)
        {
            if (t == null) continue;
            t.gameObject.SetActive(active);
        }
    }

    // Optional: manually force a collapse from other scripts.
    public void ForceCollapse()
    {
        if (!_triggered) StartCoroutine(CrumbleRoutine());
    }

#if UNITY_EDITOR
    // Gizmo hint for shake range
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.6f, 0.1f, 0.35f);
        var b = new Vector3(shakeAmplitude.x, shakeAmplitude.y, 0f);
        Gizmos.DrawWireCube(transform.position, b * 2f);
    }
#endif
}
