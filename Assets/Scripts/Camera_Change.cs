using UnityEngine;

public class Camera_Change : MonoBehaviour
{
    public Camera MainCamera; // This follows the player
    public Camera Camera; // This stays still in the room

    private void Start()
    {
        // Make sure only the main camera is enabled at the start
        if (MainCamera != null) MainCamera.enabled = true;
        if (Camera != null) Camera.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the camera switch zone.");

            // Switch to static camera
            if (MainCamera != null) MainCamera.enabled = false;
            if (Camera != null) Camera.enabled = true;
        }
    }

    private void OnTriggerExit(Collider2D  other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the camera switch zone.");

            // Switch back to main camera
            if (MainCamera != null) MainCamera.enabled = true;
            if (Camera != null) Camera.enabled = false;
        }
    }
}
