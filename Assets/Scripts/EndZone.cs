using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndZone : MonoBehaviour
{
    [SerializeField] private GameObject winText;  // Assign a UI Text or Canvas element
    [SerializeField] private float resetDelay = 3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (winText != null)
                winText.SetActive(true);

            // Restart the current scene after delay
            Invoke(nameof(RestartGame), resetDelay);
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
