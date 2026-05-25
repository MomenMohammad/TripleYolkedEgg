using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private GameObject[] player2Prefabs;
    
    private int p1Index = 0;
    private int p2Index = 0;
    private bool p1Ready = false;
    private bool p2Ready = false;

    private void Update()
    {
        // Simple polling for selection (can be improved with Input System events)
        HandleP1Selection();
        HandleP2Selection();

        if (p1Ready && p2Ready)
        {
            GameData.p1SelectedCharacter = characterPrefabs[p1Index];
            GameData.p2SelectedCharacter = player2Prefabs[p2Index];
            SceneManager.LoadScene("BattleStage");
        }
    }

    private void HandleP1Selection()
    {
        if (p1Ready) return;

        if (Keyboard.current.aKey.wasPressedThisFrame) p1Index = (p1Index - 1 + characterPrefabs.Length) % characterPrefabs.Length;
        if (Keyboard.current.dKey.wasPressedThisFrame) p1Index = (p1Index + 1) % characterPrefabs.Length;
        if (Keyboard.current.gKey.wasPressedThisFrame) p1Ready = true;
    }

    private void HandleP2Selection()
    {
        if (p2Ready) return;

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) p2Index = (p2Index - 1 + player2Prefabs.Length) % player2Prefabs.Length;
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) p2Index = (p2Index + 1) % player2Prefabs.Length;
        if (Keyboard.current.numpad1Key.wasPressedThisFrame || Keyboard.current.kKey.wasPressedThisFrame) p2Ready = true;
    }
}
