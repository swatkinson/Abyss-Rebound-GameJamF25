using UnityEngine;

public class IceZoneTrigger : MonoBehaviour
{
    [Header("Settings")]
    public string playerTag = "Player";
    public float speedMultiplier = 1.5f;      // Boost player speed
    public bool enableSliding = true;       // Toggle sliding on/off

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerInput player = other.GetComponent<PlayerInput>();
            if (player != null)
            {
                // Boost speed
                player.moveSpeed *= speedMultiplier;

                // Enable sliding
                if (enableSliding)
                {
                    player.isOnIce = true;
                }

                Debug.Log("[IceZoneTrigger] Player entered ice zone! Sliding enabled, speed boosted.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerInput player = other.GetComponent<PlayerInput>();
            if (player != null)
            {
                // Optional: reset speed and sliding if leaving ice zone
                player.moveSpeed /= speedMultiplier;
                // player.isOnIce = false;

                Debug.Log("[IceZoneTrigger] Player exited ice zone.");
            }
        }
    }
}