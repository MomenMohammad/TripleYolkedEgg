using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        ProcessCollision(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        ProcessCollision(other);
    }

    private void ProcessCollision(Collider2D other)
    {
        // Try to find the controller on the root or parent object
        CharacterController2D controller = other.GetComponentInParent<CharacterController2D>();
        
        // Only trigger if we found a controller and the object is currently active
        if (controller != null && controller.gameObject.activeInHierarchy)
        {
            if (GameManager.Instance != null)
            {
                Debug.Log($"KillZone {name} triggered by {controller.gameObject.name} (Player {controller.PlayerNumber})");
                GameManager.Instance.OnPlayerKilled(controller.PlayerNumber, controller.gameObject);
            }
            else
            {
                Debug.LogError($"KillZone {name} triggered, but GameManager.Instance is NULL!");
                // Fallback: just deactivate the player so they don't fall forever
                controller.gameObject.SetActive(false);
            }
        }
    }
}
