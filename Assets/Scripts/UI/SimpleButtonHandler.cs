using UnityEngine;

public class SimpleButtonHandler : MonoBehaviour
{
    public void OnRematchClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartMatch();
        }
    }
}
