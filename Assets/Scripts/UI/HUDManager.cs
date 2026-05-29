using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Match UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winText;

    [Header("Player 1 UI")]
    [SerializeField] private TextMeshProUGUI p1DamageText;
    [SerializeField] private TextMeshProUGUI p1StockText;
    [SerializeField] private Slider p1HealthBar;

    [Header("Player 2 UI")]
    [SerializeField] private TextMeshProUGUI p2DamageText;
    [SerializeField] private TextMeshProUGUI p2StockText;
    [SerializeField] private Slider p2HealthBar;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (GameManager.Instance != null)
        {
            if (p1StockText != null) p1StockText.text = "Stocks: " + GameManager.Instance.GetStocks(1);
            if (p2StockText != null) p2StockText.text = "Stocks: " + GameManager.Instance.GetStocks(2);
            UpdateTimer(GameManager.Instance.GetCurrentTime());
        }
    }

    private void UpdateTimer(float time)
    {
        if (timerText == null) return;
        
        // Ensure time doesn't go negative for display
        float displayTime = Mathf.Max(0, time);
        int minutes = Mathf.FloorToInt(displayTime / 60);
        int seconds = Mathf.FloorToInt(displayTime % 60);
        
        // Fix: Changed {1:02} to {1:00} for correct seconds padding
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateDamage(int playerNum, float damage)
    {
        float maxDisplayDamage = 300f;
        float healthPercent = Mathf.Clamp01(1f - (damage / maxDisplayDamage));
        
        // Color shifting logic
        Color barColor = Color.Lerp(Color.red, Color.green, healthPercent);
        if (healthPercent < 0.2f) barColor = Color.Lerp(new Color(0.5f, 0, 0), Color.red, healthPercent * 5f); // Dark red for high damage

        if (playerNum == 1)
        {
            p1DamageText.text = damage.ToString("F1") + "%";
            if (p1HealthBar != null)
            {
                p1HealthBar.value = healthPercent;
                p1HealthBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = barColor;
            }
        }
        else
        {
            p2DamageText.text = damage.ToString("F1") + "%";
            if (p2HealthBar != null)
            {
                p2HealthBar.value = healthPercent;
                p2HealthBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = barColor;
            }
        }
    }

    public void ShowWinScreen(int winnerNum)
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (winText != null) winText.text = "Player " + winnerNum + " Wins!";
        }
    }
}
