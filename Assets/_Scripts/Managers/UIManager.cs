using UnityEngine;
//using UnityEngine.UI; // Для Text
using TMPro;       // Если вы используете TextMeshPro вместо стандартного Text
// Подключайте в зависимости от того, какой UI-компонент используется.

public class UIManager : MonoBehaviour
{
    // Ссылки на объекты UI (Text или TextMeshPro)
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI castleHPText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI healthText;

    // Ссылки на менеджеры, если они не Singleton
    // Если менеджеры - Singleton, то можно получать доступ через PlayerManager.Instance, и т.п.
    public PlayerManager playerManager;
    public WaveManager waveManager;
    public Castle castle;
    public ScoreManager scoreManager;

    string debugMessage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerManager = PlayerManager.Instance;
        //waveManager   = FindObjectOfType<WaveManager>();
        //castle        = FindObjectOfType<Castle>();
        waveManager = FindAnyObjectByType<WaveManager>();
        castle = FindAnyObjectByType<Castle>();
        scoreManager  = ScoreManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();

    }

    void UpdateUI()
    {
        // Проверяем, что ссылки не пустые
        if (goldText != null && playerManager != null)
        {
            goldText.text = "Gold: " + (int)playerManager.gold;
        }

        if (waveText != null && waveManager != null)
        {
            waveText.text = "Wave: " + waveManager.GetCurrentWave();
            // Или waveManager.currentWave, если переменная публичная
        }

        if (castleHPText && castle)
        {
            castleHPText.text = $"Castle HP: {castle.currentHP}";// / {castle.maxHP}";
        }

        if (scoreText && ScoreManager.Instance)
        {
            scoreText.text = "Score: " + (int)ScoreManager.Instance.score;
        }

        if (healthText && ScoreManager.Instance)
        {
            healthText.text = "Multiplier: " + waveManager.healthMultiplier.ToString("0.00") + " HP: " + waveManager.enemySpawnHP.ToString("0.00");
        }
        //messageText.text = debugMessage;
    }
}
