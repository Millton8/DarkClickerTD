using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource; // Для фоновой музыки
    public AudioSource sfxSource;   // Для звуковых эффектов

    [Header("Audio Music Clips")]
    public AudioClip backgroundMusic;
    public AudioClip menuMusicSource; // Для фоновой музыки
    public AudioClip gameOverSound;

    [Header("Enemy Sounds Clips")]
    public AudioClip enemyDeathSound;
    public AudioClip enemyClickSound;

    [Header("Audio Clips for Towers")]
    public AudioClip basicTowerShootSound;
    public AudioClip MultiShotTowerShootSound;
    public AudioClip SniperTowerShootSound;
    public AudioClip IceTowerShootSound;
    public AudioClip AOETowerShootSound;
    public AudioClip ElectricTowerShootSound;

    [Header("Audio Clips for Buying Effects")]
    public AudioClip constellationBuySound;
    public AudioClip towerBuildSound;
    public AudioClip productionBuildSound;

    void Awake()
    {
        // Реализация паттерна Singleton
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);

            // Подпишемся на событие загрузки сцен
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnDestroy()
    {
        // Отпишемся, чтобы не было утечек
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // Запускаем фоновую музыку сразу после начала игры
        /*if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }*/
    }

    /// <summary>
    /// Проигрывает звуковой эффект (AudioClip) один раз.
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void StopMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.Stop();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            if (musicSource != null && backgroundMusic != null)
            {
                musicSource.clip = menuMusicSource;
                musicSource.loop = true;
                musicSource.Play();
            }
        }
        else if (scene.name == "MainGame")
        {
            if (musicSource != null && backgroundMusic != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.loop = true;
                musicSource.Play();
            }
        }
        else
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }
    }
}
