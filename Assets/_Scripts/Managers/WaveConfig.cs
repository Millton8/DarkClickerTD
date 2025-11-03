using UnityEngine;

[System.Serializable]
public class WaveConfig
{
    [Header("Enemies Settings")]
    public GameObject[] enemyPrefabs;   // Список разных видов врагов
    public int enemiesCount = 5;        // Сколько всего врагов спавнить в этой волне
    public float spawnInterval = 1f;    // Пауза между спавном врагов

    [Header("Difficulty Modifiers")]
    public float healthMultiplier = 1.08f; // Множитель здоровья для этой волны
    public float speedMultiplier = 1.00f;  // Множитель скорости для этой волны
    public bool bossLevel = false;
}