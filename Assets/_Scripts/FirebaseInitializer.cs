using Firebase;
using Firebase.Database;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    // URL вашей БД в Firebase, без пути «/…»
    private const string DatabaseUrl = "https://darkclickertd-default-rtdb.europe-west1.firebasedatabase.app/";

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // Для Editor-only: принудительно указываем URL
#if UNITY_EDITOR
                FirebaseApp.GetInstance(DatabaseUrl);
#else
                FirebaseApp.GetInstance(DatabaseUrl);
                // В билде отсюда URL подтянется из google-services.json / plist
#endif

                // Теперь можно безопасно получить инстанс
                var db = FirebaseDatabase.DefaultInstance;
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
            }
        });
    }
}