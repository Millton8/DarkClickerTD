using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;  // Singleton

    [Header("Current Player Score")]
    public int score = 0; // Текущее количество очков

    private void Awake()
    {
        // Реализация Singleton-паттерна
        if (Instance == null)
        {
            Instance = this;
            // Чтобы объект не разрушался при смене сцены (если вам это нужно)
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Увеличивает счёт игрока на указанное значение
    /// </summary>
    /// <param name="amount">Сколько очков добавить</param>
    public void AddScore(int amount)
    {
        score += amount;
    }

    /// <summary>
    /// Сбрасывает счёт в ноль (например, при перезапуске игры)
    /// </summary>
    public void ResetScore()
    {
        score = 0;
    }
}
