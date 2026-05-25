using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CharacterSelectionManager : MonoBehaviour
{
    [System.Serializable]
    public class CharacterEntry
    {
        public string characterName;
        public GameObject prefab;
        public Sprite characterIcon;
        public string description;
    }

    [Header("Character Data")]
    [SerializeField] private List<CharacterEntry> characterList;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI p1NameText;
    [SerializeField] private Image p1PreviewIcon;
    [SerializeField] private TextMeshProUGUI p2NameText;
    [SerializeField] private Image p2PreviewIcon;
    [SerializeField] private Button startBattleButton;
    [SerializeField] private Image p1ReadyBtnImage;
    [SerializeField] private Image p2ReadyBtnImage;

    private int p1Index = 0;
private int p2Index = 0;
    private bool p1Ready = false;
    private bool p2Ready = false;

    private void Start()
    {
        UpdateUI();
        startBattleButton.interactable = false;
        if (p1ReadyBtnImage != null) p1ReadyBtnImage.color = Color.gray;
        if (p2ReadyBtnImage != null) p2ReadyBtnImage.color = Color.gray;
    }

    public void P1Next()
{
        if (p1Ready) return;
        p1Index = (p1Index + 1) % characterList.Count;
        UpdateUI();
    }

    public void P1Prev()
    {
        if (p1Ready) return;
        p1Index = (p1Index - 1 + characterList.Count) % characterList.Count;
        UpdateUI();
    }

    public void P2Next()
    {
        if (p2Ready) return;
        p2Index = (p2Index + 1) % characterList.Count;
        UpdateUI();
    }

    public void P2Prev()
    {
        if (p2Ready) return;
        p2Index = (p2Index - 1 + characterList.Count) % characterList.Count;
        UpdateUI();
    }

    public void P1ToggleReady()
    {
        p1Ready = !p1Ready;
        if (p1ReadyBtnImage != null) p1ReadyBtnImage.color = p1Ready ? Color.green : Color.gray;
        CheckStartCondition();
    }

    public void P2ToggleReady()
    {
        p2Ready = !p2Ready;
        if (p2ReadyBtnImage != null) p2ReadyBtnImage.color = p2Ready ? Color.green : Color.gray;
        CheckStartCondition();
    }

    private void UpdateUI()
    {
        p1NameText.text = characterList[p1Index].characterName + (p1Ready ? " (READY)" : "");
        p1PreviewIcon.sprite = characterList[p1Index].characterIcon;

        p2NameText.text = characterList[p2Index].characterName + (p2Ready ? " (READY)" : "");
        p2PreviewIcon.sprite = characterList[p2Index].characterIcon;
    }

    private void CheckStartCondition()
    {
        UpdateUI();
        startBattleButton.interactable = p1Ready && p2Ready;
    }

    public void StartBattle()
    {
        GameData.p1SelectedCharacter = characterList[p1Index].prefab;
        GameData.p2SelectedCharacter = characterList[p2Index].prefab;
        SceneManager.LoadScene("BattleStage");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
