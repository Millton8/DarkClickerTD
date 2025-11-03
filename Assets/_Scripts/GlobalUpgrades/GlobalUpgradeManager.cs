using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalUpgradeManager : MonoBehaviour
{
    public static GlobalUpgradeManager Instance;

    [Header("Config")]
    public GlobalUpgradesConfig config;

    // id → definition
    private Dictionary<string, GlobalUpgradeDefinition> defs;
    private HashSet<string> unlocked = new();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        defs = config.allUpgrades.ToDictionary(u => u.id, u => u);
        Load();
    }

    public bool IsUnlocked(string id) => unlocked.Contains(id);

    public bool CanUnlock(string id)
    {
        if (!defs.ContainsKey(id)) return false;
        var d = defs[id];
        return d.prerequisites.All(p => unlocked.Contains(p.id));
    }

    public bool TryUnlock(string id)
    {
        if (!defs.ContainsKey(id) || unlocked.Contains(id)) return false;
        var def = defs[id];
        if (!CanUnlock(id)) return false;
        if (!ResourceManager.Instance.CanAfford(def.cost)) return false;

        ResourceManager.Instance.SpendWood(def.cost);
        unlocked.Add(id);

        AudioManager.Instance.PlaySFX(AudioManager.Instance.constellationBuySound);  // sound effect playing

        Save();
        return true;
    }

    private void Save()
    {
        PlayerPrefs.SetString("GlobalUpgrades", JsonUtility.ToJson(
            new SaveData { unlockedIds = unlocked.ToArray() }
        ));
        PlayerPrefs.Save();
    }

    private void Load()
    {
        var json = PlayerPrefs.GetString("GlobalUpgrades", "");
        if (string.IsNullOrEmpty(json)) return;
        var data = JsonUtility.FromJson<SaveData>(json);
        unlocked = new HashSet<string>(data.unlockedIds);
    }

    [System.Serializable]
    private class SaveData { public string[] unlockedIds; }


    /// <summary>
    /// Возвращает значение поля 'value' для данного апгрейда, или 0, если апгрейд не найден/не куплен.
    /// </summary>
    public float GetUpgradeValue(string id)
    {
        if (unlocked.Contains(id) && defs.TryGetValue(id, out var def))
            return def.value;
        return 0f;
    }

    /// <summary>
    /// Удобный геттер для самой дефиниции (если нужно).
    /// </summary>
    public GlobalUpgradeDefinition GetDefinition(string id)
    {
        defs.TryGetValue(id, out var def);
        return def;
    }
}