using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/GlobalUpgradesConfig")]
public class GlobalUpgradesConfig : ScriptableObject
{
    public GlobalUpgradeDefinition[] allUpgrades;
}

/*using UnityEngine;

public class GlobalUpgradesConfig : MonoBehaviour
{
    [Header("Global Upgrades Settings")]
    public float BasicTowerDamageUpgradeCoefficient = 1.0f;
    public float BasicTowerSpeedUpgradeCoefficient = 1.0f;
    public float BasicTowerRangeUpgradeCoefficient = 1.0f;

    public float MultishotTowerDamageUpgradeCoefficient = 1.0f;
    public float MultishotTowerSpeedUpgradeCoefficient = 1.0f;
    public float MultishotTowerRangeUpgradeCoefficient = 1.0f;

    public float SniperTowerDamageUpgradeCoefficient = 1.0f;
    public float SniperTowerSpeedUpgradeCoefficient = 1.0f;
    public float SniperTowerRangeUpgradeCoefficient = 1.0f;

    //public float IceTowerDamageUpgradeCoefficient = 1.0f;
    //public float IceTowerSpeedUpgradeCoefficient = 1.0f;
    //public float IceTowerRangeUpgradeCoefficient = 1.0f;

    public float IceTowerSlowdownUpgradeCoefficient = 1.0f;
    public float IceTowerSlowTimeUpgradeCoefficient = 1.0f;

    public float AOETowerDamageUpgradeCoefficient = 1.0f;
    public float AOETowerSpeedUpgradeCoefficient = 1.0f;
    public float AOETowerRangeUpgradeCoefficient = 1.0f;

    public float ElectricTowerDamageUpgradeCoefficient = 1.0f;
    public float ElectricTowerSpeedUpgradeCoefficient = 1.0f;
    public float ElectricTowerRangeUpgradeCoefficient = 1.0f;

    public float PlayerDamageUpgradeCoefficient = 1.0f;
    public float PlayerCritChanceUpgradeCoefficient = 1.0f;
    public float PlayerGoldGainUpgradeCoefficient = 1.0f;
}
*/