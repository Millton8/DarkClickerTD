using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;
using static UnityStandardAssets.Utility.TimedObjectActivator;

public class MenuManager : MonoBehaviour
{
    [Header("Online Leaderboard UI")]
    [Tooltip("Панель с Online Leaderboard (можно скрыть/показать)")]
    public GameObject onlineLeaderboardPanel;
    [Tooltip("10 текстовых полей для отображения топ-10 (в порядке строки)")]
    public TextMeshProUGUI[] leaderboardEntries;  // должно быть ровно 10 элементов

    [Header("Buttons")]
    public Button startGameButton;
    public Button startLevel2Button;
    public Button freshStartButton;
    public Button clearRecordsButton; // если нужна отдельная кнопка
    public Button exitButton;

    [Header("UI Texts")]
    public TextMeshProUGUI recordText;         // Ссылка на RecordText
    public TextMeshProUGUI recordHolderText;   // Ссылка на RecordHolderText

    [Header("Settings")]
    public string mainSceneName = "MainGame";
    public string level2SceneName = "HardGame";
    public int requiredScoreForLevel2 = 100000;

    DatabaseReference dbRef;

    //FirebaseLeaderboard fb;
    private void Awake()
    {
        // Подпишемся на кнопки
        startGameButton.onClick.AddListener(OnStartGame);
        startLevel2Button.onClick.AddListener(OnStartLevel2);
        freshStartButton.onClick.AddListener(OnFreshStart);
        if (clearRecordsButton != null)
            clearRecordsButton.onClick.AddListener(OnClearRecords);
        exitButton.onClick.AddListener(OnExit);

        dbRef = FirebaseDatabase.DefaultInstance.GetReference("leaderboard");
    }

    private void Start()
    {        
        // Делаем запрос: сортировка по полю "score", последние 10

        //AudioManager.Instance.PlaySFX(AudioManager.Instance.menuMusicSource);
        //AudioManager.Instance.musicSource.Stop();

        //fb = FindFirstObjectByType<FirebaseLeaderboard>();

        RefreshRecordUI();
        UpdateLevel2Button();
        LoadOnlineLeaderboard();
        //fb.PostScore("Alice", 15000);
        /*fb.GetTop(10, entries =>
        {
            foreach (var e in entries)
            {
                Debug.Log($"{e.playerName}: {e.score}");

                Top1recordHolderText.text = e.playerName.ToString();
                Top1recordText.text = e.score.ToString();
            }
        });*/
    }

    /// <summary>
    /// Запустить первую игру
    /// </summary>
    private void OnStartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainSceneName);
    }

    /// <summary>
    /// Попытаться запустить второй уровень
    /// </summary>
    private void OnStartLevel2()
    {
        int best = PlayerPrefs.GetInt("ScoreRecord", 0);
        if (best >= requiredScoreForLevel2)
        {
            SceneManager.LoadScene(level2SceneName);
        }
        else
        {
            // Можно вывести сообщение пользователю
            Debug.Log("Need at least " + requiredScoreForLevel2 + " points to unlock Level 2.");
        }
    }

    /// <summary>
    /// Сбросить все апгрейды и рекорды (fresh start)
    /// </summary>
    private void OnFreshStart()
    {
        if (PlayerPrefs.HasKey("GlobalUpgrades"))
            PlayerPrefs.DeleteKey("GlobalUpgrades");
        if (PlayerPrefs.HasKey("ScoreRecord"))
            PlayerPrefs.DeleteKey("ScoreRecord");
        if (PlayerPrefs.HasKey("RecordHolder"))
            PlayerPrefs.DeleteKey("RecordHolder");

        PlayerPrefs.Save();
        UpdateLevel2Button();
        RefreshRecordUI();
        Debug.Log("Fresh start: апгрейды и рекорды сброшены.");
    }

    /// <summary>
    /// (Опционально) Удалить только рекорды
    /// </summary>
    private void OnClearRecords()
    {
        if (PlayerPrefs.HasKey("ScoreRecord"))
            PlayerPrefs.DeleteKey("ScoreRecord");
        if (PlayerPrefs.HasKey("RecordHolder"))
            PlayerPrefs.DeleteKey("RecordHolder");

        PlayerPrefs.Save();
        UpdateLevel2Button();
        RefreshRecordUI();
        Debug.Log("Рекорды очищены.");
    }

    /// <summary>
    /// Выход из игры
    /// </summary>
    private void OnExit()
    {
        Application.Quit();
        Debug.Log("Game exited.");
    }

    /// <summary>
    /// Обновить состояние кнопки Level2 (заблокирована/разблокирована)
    /// </summary>
    private void UpdateLevel2Button()
    {
        int best = PlayerPrefs.GetInt("ScoreRecord", 0);
        bool unlocked = best >= requiredScoreForLevel2;

        startLevel2Button.interactable = unlocked;
        var txt = startLevel2Button.GetComponentInChildren<Text>();
        if (txt != null)
        {
            txt.text = unlocked
                ? "Start Level 2"
                : $"Level 2 (need {requiredScoreForLevel2})";
        }
    }
    private void RefreshRecordUI()
    {
        int record = PlayerPrefs.GetInt("ScoreRecord", 0);
        string holder = PlayerPrefs.GetString("RecordHolder", "—");

        if (recordText != null)
            recordText.text = $"Ваш рекорд: {record}";
        if (recordHolderText != null)
            recordHolderText.text = $"Игрок: {holder}";
    }

    /// <summary>
    /// Загружает топ-10 рекордов из Firebase Realtime Database
    /// и отображает их в UI. Если записей меньше 10, оставшиеся строки будут “номер. ---”.
    /// </summary>
    public void LoadOnlineLeaderboard()
    {
        onlineLeaderboardPanel.SetActive(true);

        // Сразу прячем / обнуляем все строки
        for (int i = 0; i < leaderboardEntries.Length; i++)
            leaderboardEntries[i].text = $"{i + 1}. ...";



        dbRef.OrderByChild("score")
             .LimitToLast(10)
             .GetValueAsync()
             .ContinueWithOnMainThread(task =>
             {

                 if (task.IsFaulted)
                 {
                     Debug.LogError("Не удалось получить онлайн-рекорды: " + task.Exception);
                     return;
                 }
                 
                 var snapshot = task.Result;
                 Debug.Log($"[Leaderboard] snapshot.Children count = {snapshot.ChildrenCount}");
                 // Собираем записи
                 var entries = new List<(string player, long score)>();
                 foreach (var child in snapshot.Children)
                 {
                     // Если структура Entry { playerName, score }
                     string name = child.Child("playerName").Value?.ToString() ?? "—";
                     long score = 0;
                     if (long.TryParse(child.Child("score").Value?.ToString(), out var s))
                         score = s;

                     entries.Add((name, score));
                 }
                 
                 // Firebase вернёт по возрастанию score, поэтому переворачиваем
                 entries.Sort((a, b) => b.score.CompareTo(a.score));
                 
                 for (int i = 0; i < leaderboardEntries.Length; i++)
                 {
                     if (i < entries.Count)
                     {
                         var e = entries[i];
                         leaderboardEntries[i].text = $"{i + 1}. {e.player}: {e.score}";
                     }
                     else
                     {
                         leaderboardEntries[i].text = $"{i + 1}. ---";
                     }
                 }
                  
                 // Заполняем UI-тексты

             });
        // Показываем панель (если скрыта)
    }
}