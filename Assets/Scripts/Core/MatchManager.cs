using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    [SerializeField] private Transform p1Spawn;
    [SerializeField] private Transform p2Spawn;

    [SerializeField] private UnityEngine.InputSystem.InputActionAsset p1Actions;
    [SerializeField] private UnityEngine.InputSystem.InputActionAsset p2Actions;

    private void Start()
{
        SpawnPlayers();
        GameManager.Instance.StartMatch(2);
    }

    private void SpawnPlayers()
    {
        GameObject p1Prefab = GameData.p1SelectedCharacter != null ? GameData.p1SelectedCharacter : player1Prefab;
        GameObject p2Prefab = GameData.p2SelectedCharacter != null ? GameData.p2SelectedCharacter : player2Prefab;

        if (p1Prefab != null && p1Spawn != null)
        {
            GameObject p1 = Instantiate(p1Prefab, p1Spawn.position, Quaternion.identity);
            var controller = p1.GetComponent<CharacterController2D>();
            controller.SetPlayerNumber(1);
            
            var input = p1.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (input != null && p1Actions != null) input.actions = p1Actions;
        }

        if (p2Prefab != null && p2Spawn != null)
        {
            GameObject p2 = Instantiate(p2Prefab, p2Spawn.position, Quaternion.identity);
            var controller = p2.GetComponent<CharacterController2D>();
            controller.SetPlayerNumber(2);

            var input = p2.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (input != null && p2Actions != null) input.actions = p2Actions;
        }
    }
}
