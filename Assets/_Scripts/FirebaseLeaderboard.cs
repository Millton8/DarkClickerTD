using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseLeaderboard : MonoBehaviour
{
    private const string DatabaseUrl = "https://darkclickertd-default-rtdb.europe-west1.firebasedatabase.app/";

    DatabaseReference dbRef;

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // Всегда указываем URL, даже в билде
                FirebaseApp.GetInstance(DatabaseUrl);
            }
            else
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
        });

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(dep =>
        {
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        });
    }

    // Структура записи
    public class Entry
    {
        public string playerName;
        public int score;
        public long timestamp;
        public Entry() { }
        public Entry(string name, int score)
        {
            this.playerName = name;
            this.score = score;
            this.timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }

    // Отправить свой рекорд
    public Task PostScore(string player, int score)
    {
        string key = dbRef.Child("leaderboard").Push().Key;
        var entry = new Entry(player, score);
        string json = JsonUtility.ToJson(entry);
        return dbRef.Child("leaderboard").Child(key).SetRawJsonValueAsync(json);
    }

    // Получить топ-N
    public Task GetTop(int topN, System.Action<List<Entry>> callback)
    {
        return dbRef.Child("leaderboard")
            .OrderByChild("score")
            .LimitToLast(topN)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                var list = new List<Entry>();
                if (task.Result.Exists)
                {
                    foreach (var child in task.Result.Children)
                    {
                        var e = JsonUtility.FromJson<Entry>(child.GetRawJsonValue());
                        list.Add(e);
                    }
                    // Firebase вернёт по возрастанию score, перевёрнём:
                    list.Sort((a, b) => b.score.CompareTo(a.score));
                }
                callback?.Invoke(list);
            });
    }

    /// <summary>
    /// Запрос и колбэк для получения единственной записи с наибольшим score.
    /// </summary>
    public void LoadTopRecord(System.Action<string /*player*/, long /*score*/> callback)
    {
        var dbRef = FirebaseDatabase.DefaultInstance.GetReference("leaderboard");
        dbRef
            .OrderByChild("score")
            .LimitToLast(1)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || !task.Result.Exists)
                {
                    Debug.LogWarning("Не удалось получить топ-рекорд или он отсутствует");
                    callback?.Invoke("—", 0);
                    return;
                }

                // В snapshot будет ровно одна запись — лучшая
                var enumChild = task.Result.Children.GetEnumerator();
                enumChild.MoveNext();
                var child = enumChild.Current;

                string player = child.Child("playerName").Value?.ToString() ?? "—";
                long score = 0;
                long.TryParse(child.Child("score").Value?.ToString(), out score);

                callback?.Invoke(player, score);
            });
    }
}