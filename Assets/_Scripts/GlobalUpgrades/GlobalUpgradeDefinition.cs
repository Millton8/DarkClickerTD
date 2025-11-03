using UnityEngine;

public enum GlobalUpgradeType
{
    PlayerDamage,
    PlayerCritChance,
    TowerDamage,
    TowerSpeed,
    TowerRange,
    // ...
}

[CreateAssetMenu(menuName = "Upgrades/GlobalUpgradeDefinition")]
public class GlobalUpgradeDefinition : ScriptableObject
{
    [Header("Identity")]
    public string id;                     // "tower_damage_1"
    public string upgradeName;            // "Tower Damage I"
    [TextArea] public string description;

    public GlobalUpgradeType type;
    public int cost;                      // цена в Wood
    public float value;                   // на что влияет (например +0.1 → +10%)

    [Header("Prerequisites")]
    public GlobalUpgradeDefinition[] prerequisites;
}