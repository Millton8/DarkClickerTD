using UnityEngine;
using UnityEngine.SceneManagement;

public class EscToMenu : MonoBehaviour
{
    [Tooltip("Имя сцены главного меню")]
    public string mainMenuSceneName = "MainMenu";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Если вы где-то ставили Time.timeScale = 0 при паузе,
            // то перед выходом в меню нужно вернуть нормальный ход времени:
            Time.timeScale = 1f;

            // Загружаем сцену главного меню
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}