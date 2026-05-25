using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        CharacterController2D controller = other.GetComponent<CharacterController2D>();
        if (controller != null)
        {
            GameManager.Instance.OnPlayerKilled(controller.PlayerNumber, other.gameObject);
        }
    }
}
