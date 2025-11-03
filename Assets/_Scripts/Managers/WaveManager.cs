using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{   
    [Header("Wave Settings")]
    public List<WaveConfig> waveConfigs;

    public GameObject enemyPrefab;       // Префаб врага
    public Transform spawnPoint;         // Точка спавна врагов
    public int baseEnemiesPerWave = 4;   // Базовое количество врагов в первой волне
    public float spawnInterval = 1f;     // Интервал между спавнами
    public float baseEnemyHealth = 40f;  // Базовое здоровье врагов
    public float baseGoldReward = 5f;
    public float pauseBetweenLevels = 4f;

    [Header("Difficulty Settings")]
    public float healthMultiplier = 1.08f; // Множитель, на который увеличиваем здоровье врагов
    public int enemiesPerWaveIncrement = 2; // На сколько врагов больше с каждой волной
    public float goldRewardMultiplier = 1.05f;  // Увеличение награды в золоте
    public float difficultyMultiplier = 1.5f;

    //private int enemiesToSpawn = 0;
    private int spawnedEnemies = 0;
    private int aliveEnemies = 0;
    public float currentGoldReward;
    public float currentLumberReward;

    private int currentWaveIndex = -1;
    private bool waveInProgress = false;

    public int difficulty = 1;
    public int difficultyUPCheck = 0;

    public float enemySpawnHP;


    public UIManager uimanager;
    //string debugMessage;

    //public Enemy enemy;

    void Start()
    {
        StartWave(); // Запускаем первую волну
        currentGoldReward = baseGoldReward;
    }

    /// <summary>
    /// Запуск новой волны
    /// </summary>
    public void StartWave()
    {
        currentWaveIndex++;


        WaveConfig currentWave = waveConfigs[currentWaveIndex % waveConfigs.Count]; /// тут может быть ошибка выхода за границы waveConfigs.Length 
        spawnedEnemies = 0;
        aliveEnemies = 0;
        waveInProgress = true;
        // Начинаем вызывать SpawnEnemy() с заданным интервалом
        InvokeRepeating(nameof(SpawnEnemy), 0f, currentWave.spawnInterval);
        Debug.Log($"Wave {currentWaveIndex + 1} started with {waveConfigs[currentWaveIndex % waveConfigs.Count].enemiesCount + difficulty} enemies!");
        uimanager.messageText.text = ($"Wave {currentWaveIndex + 1} started with {waveConfigs[currentWaveIndex % waveConfigs.Count].enemiesCount + difficulty} enemies!");

        currentGoldReward = baseGoldReward + (goldRewardMultiplier * currentWaveIndex);
        if (currentWave.bossLevel == true) 
        { 
            currentGoldReward *= 10;
            currentLumberReward += 50;
        }
        else
            currentLumberReward = 0;

        if (currentWaveIndex % 5 == 0)
            healthMultiplier *= 1.1f;   // was: healthMultiplier += 0.1f;

        if (currentWave.bossLevel == false) // требуется тестирование баланса
            if (difficultyMultiplier <= currentWave.healthMultiplier)
                difficultyMultiplier = currentWave.healthMultiplier;    // изменяем множитель сложности. Используется, только когда пройдены все волны, чтобы рестартануть ХП с учетом множителя последней волны не босса

        if (currentWaveIndex == 0)
        {
            if (GuidePanelController.Instance != null)
            {
                GuidePanelController.Instance.Show($"Для атаки кликайте на врагов!");
            }
        }
        else if (currentWaveIndex == 1) {
            if (GuidePanelController.Instance != null)
            {
                GuidePanelController.Instance.Show($"Для покупки башни нажмите на иконку башни внизу!");
            }
        }
        else if (currentWaveIndex == 2)
        {
            if (GuidePanelController.Instance != null)
            {
                GuidePanelController.Instance.Show($"Для улучшения клика нажмите на замок!");
            }
        }
        else if (currentWaveIndex == 3)
        {
            if (GuidePanelController.Instance != null)
            {
                GuidePanelController.Instance.Show($"Для улучшения башни нажмите на башню :)");
            }
        }
        else if (currentWaveIndex == 4)
        {
            if (GuidePanelController.Instance != null)
            {
                GuidePanelController.Instance.Show($"Скоро будет босс. Будьте осторожны!");
            }
        }
        else if (currentWaveIndex == 5)
        {
            if (GuidePanelController.Instance != null)
            {
                GuidePanelController.Instance.Show($"За дерево можно приобретать производящие здания!");
            }
        }
        else if (currentWaveIndex == 6)
        {
            if (GuidePanelController.Instance != null)
            {
                GuidePanelController.Instance.Show($"Производящие здания производят дерево. Ого!");
            }
        }
        else if (currentWaveIndex == 7)
        {
            if (GuidePanelController.Instance != null)
            {
                GuidePanelController.Instance.Show($"Когда у вас будет много дерева, то можно будет покупать глобальные улучшения!");
            }
        }
        else if (currentWaveIndex == 8)
        {
            if (GuidePanelController.Instance != null)
            {
                GuidePanelController.Instance.Show($"Глобальные улучшения будут работать только после рестарта игры!");
            }
        }

        if (difficultyUPCheck == waveConfigs.Count)
        {
            difficultyUPCheck -= waveConfigs.Count;

            // Все волны прошли — либо начать заново, либо конец игры
            Debug.Log("All waves completed! Restarting!");
            uimanager.messageText.text = currentWaveIndex + " уровней пройдено. Повышаем сложность!";

            if (GuidePanelController.Instance != null)
            {
                GuidePanelController.Instance.Show(currentWaveIndex + " уровней пройдено. Повышаем сложность!");
            }

            difficulty++;
            //currentWaveIndex = 0;
            healthMultiplier += difficultyMultiplier; // was healthMultiplier * difficultyMultiplier;
            healthMultiplier += difficulty;
            uimanager.messageText.text = currentWaveIndex + " уровней пройдено. Повышаем сложность! Health multiplier now: " + healthMultiplier + "Difficulty: " + difficulty;

            //return;
        }
        else
        {
            difficultyUPCheck++;
        }
    }

    /// <summary>
    /// Спавн одного врага
    /// </summary>
    private void SpawnEnemy()
    {
        // Получаем текущую волну
        WaveConfig currentWave = waveConfigs[currentWaveIndex % waveConfigs.Count];
        if (spawnedEnemies >= waveConfigs[currentWaveIndex % waveConfigs.Count].enemiesCount + difficulty) /// Сколько врагов заспавнить. Чтобы постоянно увеличивалось waveConfigs[currentWaveIndex % waveConfigs.Length].enemiesCount + difficulty
        {                                                            /// Если хотим босса, то currentWave.enemiesCount
            // Все враги этой волны заспавнены - останавливаем Invoke
            CancelInvoke(nameof(SpawnEnemy));
            return;
        }

        // Выбираем случайный префаб из списка текущей волны
        GameObject enemyPrefab = currentWave.enemyPrefabs[Random.Range(0, currentWave.enemyPrefabs.Length)];

        // Добавляем небольшое случайное смещение по оси Y
        //float randomY = Random.Range(-5f, 5f);
        //Vector3 randomSpawnPos = spawnPoint.position;// + new Vector3(0, randomY, 0);

        // Создаём врага
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        //GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);//, Quaternion.identity);

        // Инициализируем врага с учётом множителей волны
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemySpawnHP = enemy.baseHealth *  currentWave.healthMultiplier * healthMultiplier;
            //float newHealth = baseEnemyHealth * Mathf.Pow(healthMultiplier, (currentWaveIndex - 1));
            //enemy.InitializeEnemy(currentWave.healthMultiplier * healthMultiplier, currentWave.speedMultiplier);
            enemy.InitializeEnemy(enemySpawnHP, currentWave.speedMultiplier);
            //Debug.Log($"Enemy Spawned with Hp: {enemy.baseHealth * (currentWave.healthMultiplier * healthMultiplier)}!");
            Debug.Log($"Enemy Base hp: {enemy.baseHealth}, CW.multiplier: {currentWave.healthMultiplier}, multiplier: {healthMultiplier}!");
            Debug.Log($"Enemy Spawned with Hp: {enemySpawnHP}!");
            
            // Сообщаем, что WaveManager — "хозяин" врага
            // (чтобы враг мог вернуть колбэк при смерти, если нужно)
            enemy.SetWaveManager(this);
        }

        spawnedEnemies++;
        aliveEnemies++;

        if (currentWave.bossLevel == true)
        {
            uimanager.messageText.text = "БОСС! Уровень: " + (currentWaveIndex+1);
            Debug.Log("Boss level!");
            CancelInvoke(nameof(SpawnEnemy));
            return;
        }
    }

    /// <summary>
    /// Вызывается из Enemy, когда враг умирает
    /// </summary>
    public void OnEnemyKilled()
    {
        WaveConfig currentWave = waveConfigs[currentWaveIndex % waveConfigs.Count];
        aliveEnemies--;

        // Если это был последний враг волны, запускаем новую
        if ((aliveEnemies <= 0 && waveInProgress && spawnedEnemies >= waveConfigs[currentWaveIndex % waveConfigs.Count].enemiesCount + difficulty) || currentWave.bossLevel)
        {
            waveInProgress = false;
            Debug.Log($"Wave {currentWaveIndex + 1} cleared!");
            uimanager.messageText.text = ($"Волна {currentWaveIndex + 1} пройдена!");

            // Здесь можно сделать задержку перед стартом следующей волны:
            Invoke(nameof(StartWave), pauseBetweenLevels); // 2 секунды пауза
        } 
    }
    public int GetCurrentWave()
    {
        return currentWaveIndex + 1;
    }

    public void CastleDead()
    {
        if (currentWaveIndex > 10)
            { 
            currentWaveIndex -= 10;
            difficulty--;
            }
        else
            currentWaveIndex = 0;
    }

    public bool IsBossLevel() {

        // Получаем текущую волну
        WaveConfig currentWave = waveConfigs[currentWaveIndex % waveConfigs.Count];

        if (currentWave.bossLevel == true)
            return true;
        else
            return false;
    }
}
