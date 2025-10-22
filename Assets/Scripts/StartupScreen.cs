using UnityEngine;

public class TwoStageSplash : MonoBehaviour
{
    [Header("Groups")]
    public CanvasGroup root;   // LoadingScreen (parent)
    public CanvasGroup frame1; // "Now presenting"
    public CanvasGroup frame2; // "<Game Name>"

    [Header("Timings")]
    public float frame1Hold = 2.0f;   // how long to show frame 1
    public float crossfade = 0.75f;   // fade 1 -> 2
    public float frame2Hold = 1.5f;   // how long to show frame 2
    public float fadeOut = 1.0f;      // fade to game

    // survives scene reloads for this app run
    private static bool shownOnce = false;

    void Awake()
    {
        if (shownOnce)
        {
            // Hide immediately on restarts
            if (root) root.alpha = 0f;
            gameObject.SetActive(false);
        }
        else
        {
            // Ensure initial state
            if (root) root.alpha = 1f;
            if (frame1) frame1.alpha = 1f;
            if (frame2) frame2.alpha = 0f;
        }
    }

    void OnEnable()
    {
        if (!shownOnce) StartCoroutine(RunSequence());
    }

    System.Collections.IEnumerator RunSequence()
    {
        shownOnce = true;

        // Hold frame 1
        yield return new WaitForSecondsRealtime(frame1Hold);

        // Crossfade 1 -> 2
        float t = 0f;
        while (t < crossfade)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / crossfade);
            if (frame1) frame1.alpha = 1f - k;
            if (frame2) frame2.alpha = k;
            yield return null;
        }
        if (frame1) frame1.alpha = 0f;
        if (frame2) frame2.alpha = 1f;

        // Hold frame 2
        yield return new WaitForSecondsRealtime(frame2Hold);

        // Fade everything out (reveal game)
        t = 0f;
        while (t < fadeOut)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeOut);
            if (root) root.alpha = 1f - k;
            yield return null;
        }
        if (root) root.alpha = 0f;

        gameObject.SetActive(false);
        Destroy(gameObject); // optional cleanup
    }
}
