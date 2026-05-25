using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string characterSelectScene = "CharacterSelect"; 

    public void StartGame()
{
        SceneManager.LoadScene(characterSelectScene);
    }

    public void OpenSettings()
    {
        Debug.Log("Settings Opened");
        // TODO: Implement settings panel
    }

    public void QuitGame()
    {
        Debug.Log("Game Quitting");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
