using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndZone : MonoBehaviour
{
    [SerializeField] private TMP_Text winText;
    [SerializeField] private TMP_Text runTimeText;     // assign via Inspector
    [SerializeField] private Camera winCamera;
    [SerializeField] private float resetDelay = 5f;
    [SerializeField] private float runTime = 0f;

    private bool running = true;

    public AudioClip winSound;

    private void Update()
    {
        if (running)
        {
            runTime += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!running) return;

        if (other.CompareTag("Player"))
        {
            running = false;

            // Show "You Win!"
            if (winText != null)
                winText.gameObject.SetActive(true);

            // Display final run time
            if (runTimeText != null)
            {
                int minutes = (int)(runTime / 60);
                float seconds = runTime % 60;
                runTimeText.text = string.Format("Run Time: {0:0}:{1:00.00}", minutes, seconds);
            }

            // Play sound
            AudioManager.Instance.PlaySFX(winSound, 0.1f);

            // Switch cameras
            if (winCamera != null)
            {
                Camera mainCam = Camera.main;
                if (mainCam != null)
                    mainCam.gameObject.SetActive(false);

                winCamera.gameObject.SetActive(true);
            }

            // Restart scene after a delay
            Invoke(nameof(RestartGame), resetDelay);
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
