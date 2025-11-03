using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public GlobalUpgradesConfig globalUpgradesConfig;

    [Header("UI Элементы")]
    public GameObject gameOverPanel; // Панель меню проигрыша (должна быть активна в сцене)
    public Text messageText;         // Текст с сообщением о поражении
   
    [Header("Кнопки меню")]
    public Button restartButton;     // Кнопка для перезапуска игры
    public Button quitButton;        // Кнопка для выхода из игры
    public Button confirmButton;       // Кнопка подтверждения ввода имени

    [Header("UI для рекордов")]
    public InputField playerNameInput; // Поле для ввода имени игрока
    public Text currentScoreText;         // Текст с сообщением о поражении
    public Text yourRecordScoreText;         // Текст с вашим рекордом
    public Text globalRecordScoreText;         // Текст с глобальным рекордом

    private int currentScore;

    public UIManager uimanager;
    public WaveManager waveManager;
    public ScoreManager scoreManager;

    public string mainSceneName = "MainGame";
    public string menuSceneName = "MainMenu";

    private bool saveFlag = false;

    // get leaderboard
    FirebaseLeaderboard fb;

    // records from database
    private long globalRecordScore = 0;
    private string globalRecordHolder = "";

    // current records
    private long yourRecordScore = 0;
    private string yourRecordHolder = "";

    private void Awake()
    {
        //playerManager = PlayerManager.Instance;
        //waveManager   = FindObjectOfType<WaveManager>();
        //castle        = FindObjectOfType<Castle>();
        waveManager = FindAnyObjectByType<WaveManager>();
        scoreManager = ScoreManager.Instance;

        // Панель должна быть скрыта при запуске игры, но объект сам остаётся активным
        gameOverPanel.SetActive(false);

        // Если кнопки не назначены через Inspector, можно подписаться на события программно
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);

        fb = FindFirstObjectByType<FirebaseLeaderboard>();
    }

    private void Start()
    {
        FindFirstObjectByType<FirebaseLeaderboard>().LoadTopRecord((fbRecordHolder, fbRecordScore) =>
        {
            Debug.Log($"Топ 1: {fbRecordHolder} — {fbRecordScore}");
            globalRecordScore = fbRecordScore;
            globalRecordHolder = fbRecordHolder;
        }
        );
    }

    /// <summary>
    /// Показывает меню проигрыша, останавливая игру
    /// </summary>
    public void ShowGameOverMenu()
    {
        // Останавливаем игру
        Time.timeScale = 0f;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.gameOverSound);
        AudioManager.Instance.musicSource.Stop();

        // maybe we need to stop background music;
        //AudioManager.Instance.StopMusic();

        // Получаем текущий результат из ScoreManager (предполагается, что ScoreManager реализован как Singleton)
        currentScore = ScoreManager.Instance.score;

        //int recordScore = PlayerPrefs.GetInt("ScoreRecord", 0);
        //string recordHolder = PlayerPrefs.GetString("RecordHolder", "Никто");

        yourRecordScore = PlayerPrefs.GetInt("ScoreRecord", 0);
        yourRecordHolder = PlayerPrefs.GetString("RecordHolder", "Никто");

        globalRecordScoreText.text = "Глобальный рекорд: " + globalRecordScore.ToString() + "(" + globalRecordHolder + ")";

        currentScoreText.text = "Ваш счёт: " + currentScore.ToString();
        yourRecordScoreText.text = "Ваш рекорд: " + yourRecordScore.ToString() + " (" + (yourRecordHolder == "" ? "не установлен" : yourRecordHolder) + ")";

        // Выводим сообщение о поражении
        messageText.text = "Поражение! Ваш замок разрушен.";

        gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// Обработчик кнопки перезапуска игры.
    /// Сбрасывает Time.timeScale и загружает текущую сцену заново.
    /// </summary>
    public void OnRestartButtonClicked()
    {
        // Восстанавливаем нормальное течение времени
        Time.timeScale = 1f;
        // Загружаем текущую сцену заново
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        //SceneManager.LoadScene(mainSceneName);
        SceneManager.LoadScene(menuSceneName);
    }

    /// <summary>
    /// Обработчик кнопки выхода из игры.
    /// </summary>
    public void OnQuitButtonClicked()
    {
        // Для сборки приложения
        Application.Quit();
        // Если запускаете в редакторе, Application.Quit() не закрывает Unity.
        Debug.Log("Выход из игры.");
    }

    /// <summary>
    /// Обработчик нажатия кнопки подтверждения ввода имени.
    /// Сохраняет имя игрока как рекордсмена, если установлен новый рекорд.
    /// </summary>
    private void OnConfirmButtonClicked()
    {
        if (saveFlag == true)
            return;

        saveFlag = true;
        string playerName = playerNameInput.text.Trim();
        if (!string.IsNullOrEmpty(playerName))
        {
            fb.PostScore(playerName, currentScore);

            //int recordScore = PlayerPrefs.GetInt("ScoreRecord", 0);

            if (currentScore > yourRecordScore)
            {
                yourRecordScoreText.text = "Ваш рекорд: " + currentScore.ToString() + " (" + playerName + ")";
                // Сохраняем имя рекордсмена
                PlayerPrefs.SetString("RecordHolder", playerName);
                PlayerPrefs.SetInt("ScoreRecord", currentScore);
                PlayerPrefs.Save();

                //AudioManager.Instance.musicSource.Stop();
                //AudioManager.Instance.PlaySFX(AudioManager.Instance.gameOverSound);

                // Обновляем отображение рекорда

            }
        }
    }
}