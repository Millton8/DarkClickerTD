using UnityEngine;
using UnityEngine.SceneManagement; // Для перезапуска сцены

public class Castle : MonoBehaviour
{
    [Header("Castle Health Settings")]
    public float maxHP = 100f;
    public float currentHP;

    // Здесь можно указать, какой урон Castle получает от врага за одно попадание
    public float damageFromEnemy = 5f; // пример, если нужно

    public WaveManager waveManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Изначально текущее здоровье = максимуму
        currentHP = maxHP;
        waveManager = FindFirstObjectByType<WaveManager>();
    }

    /// <summary>
    /// Вызывается, когда замок получает урон от врагов
    /// </summary>
    /// <param name="amount">Количество урона</param>
    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP < 0f)
        {
            currentHP = 0f;
        }

        // Если здоровье опускается до 0, завершаем игру
        if (currentHP <= 0)
        {
            OnCastleDestroyed();
        }
    }

    /// <summary>
    /// Обработчик, когда здоровье упало до нуля
    /// </summary>
    private void OnCastleDestroyed()
    {
        Debug.Log("Castle destroyed! Decreasing Difficulty!");
        // Можно поставить задержку или сразу перезапустить сцену
        // Здесь — мгновенный перезапуск
       // ScoreManager.Instance.ResetScore();
       // SceneManager.LoadScene(SceneManager.GetActiveScene().name);


        //waveManager.CastleDead(); // альтернативный метод, если мы не хотим умирать, а хотим уменьшить сложность.
        //currentHP = maxHP;
        GameOverMenu gameOverMenu = FindFirstObjectByType<GameOverMenu>();
        if (gameOverMenu != null)
        {
            gameOverMenu.ShowGameOverMenu();
        }
    }
}
