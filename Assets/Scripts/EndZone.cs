using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndZone : MonoBehaviour
{
    [SerializeField] private TMP_Text winText;
    [SerializeField] private Camera winCamera;
    [SerializeField] private float resetDelay = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Show "You Win!"
            if (winText != null)
                winText.gameObject.SetActive(true);

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
