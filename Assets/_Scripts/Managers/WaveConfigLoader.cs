using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class WaveConfigLoader : MonoBehaviour
{
    public string csvFileName = "Waves"; // имя файла (без расширения) в папке Resources, например Resources/waves.csv
    public List<WaveConfig> waveConfigs; // Для отладки можно сохранить список здесь
    //public WaveManager waveManager;


    void Awake()
    {
        waveConfigs = LoadWaveConfigs();
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.waveConfigs = waveConfigs;
            Debug.Log("WaveConfigs успешно загружены в WaveManager.");
        }
        else
        {
            Debug.LogError("WaveManager не найден в сцене!");
        }
    }

    List<WaveConfig> LoadWaveConfigs()
    {
        List<WaveConfig> configs = new List<WaveConfig>();

        // Загружаем CSV-файл из папки Resources
        TextAsset csvData = Resources.Load<TextAsset>(csvFileName);
        if (csvData == null)
        {
            Debug.LogError("CSV file not found: " + csvFileName);
            return configs;
        }

        StringReader reader = new StringReader(csvData.text);
        bool isHeader = true;
        while (true)
        {
            string line = reader.ReadLine();
            if (line == null)
                break;

            // Пропускаем строку заголовка
            if (isHeader)
            {
                isHeader = false;
                continue;
            }

            // Разбиваем строку по запятым
            string[] values = line.Split(',');
            if (values.Length < 6)
            {
                Debug.LogWarning("Invalid CSV line: " + line);
                continue;
            }

            WaveConfig config = new WaveConfig();

            // enemyPrefabs: имена префабов, разделенные символом ';'
            string prefabNamesStr = values[0].Trim();
            string[] prefabNames = prefabNamesStr.Split(';');
            List<GameObject> prefabsList = new List<GameObject>();
            foreach (string prefabName in prefabNames)
            {
                // Загружаем префаб из папки Resources/Prefabs/EnemyPrefabs/
                GameObject prefab = Resources.Load<GameObject>("Prefabs/EnemyPrefabs/" + prefabName.Trim());
                if (prefab != null)
                {
                    prefabsList.Add(prefab);
                }
                else
                {
                    Debug.LogWarning("Prefab not found in Prefabs/EnemyPrefabs/: " + prefabName);
                }
            }
            config.enemyPrefabs = prefabsList.ToArray();

            // enemiesCount
            if (int.TryParse(values[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int enemiesCount))
            {
                config.enemiesCount = enemiesCount;
            }
            else
            {
                Debug.LogWarning("Invalid enemiesCount: " + values[1]);
            }

            // spawnInterval
            if (float.TryParse(values[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float spawnInterval))
            {
                config.spawnInterval = spawnInterval;
            }
            else
            {
                Debug.LogWarning("Invalid spawnInterval: " + values[2]);
            }

            // healthMultiplier
            if (float.TryParse(values[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float healthMultiplier))
            {
                config.healthMultiplier = healthMultiplier;
            }
            else
            {
                Debug.LogWarning("Invalid healthMultiplier: " + values[3]);
            }

            // speedMultiplier
            if (float.TryParse(values[4], NumberStyles.Float, CultureInfo.InvariantCulture, out float speedMultiplier))
            {
                config.speedMultiplier = speedMultiplier;
            }
            else
            {
                Debug.LogWarning("Invalid speedMultiplier: " + values[4]);
            }

            // bossLevel
            if (bool.TryParse(values[5].Trim(), out bool bossLevel))
            {
                config.bossLevel = bossLevel;
            }
            else
            {
                // Если используется числовое значение, например 0/1
                config.bossLevel = (values[5].Trim() == "1");
            }

            configs.Add(config);
        }
        return configs;
    }
}