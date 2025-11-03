using UnityEngine;

/// <summary>
/// Управляет улучшениями клика (урон, шанс крит. удара, шанс бонус. золота)
/// </summary>

public class ClickUpgradeManager : MonoBehaviour
{
    [Header("References")]
    public PlayerManager playerManager; // Ссылка на PlayerManager (если не Singleton)

    [Header("Base Upgrade Costs")]
    public int baseDamageUpgradeCost = 50;
    public int baseCritUpgradeCost = 50;
    public int baseGoldChanceUpgradeCost = 50;

    [Header("Upgrade Multipliers")]
    // Множитель, на который умножается базовая стоимость при каждом апгрейде
    public float costMultiplier = 1.5f;

    [Header("Upgrade Increments")]
    public float damageIncrement = 0.4f;    // изменено на проценты                                                                                                                                                                                                                                                        ;       // +2 урона за уровень
    public float critChanceIncrement = 5f;   // +5% к шансу крита
    public float bonusGoldChanceIncrement = 5f; // +5% к шансу бонусного золота

    // Текущие уровни апгрейда
    private int damageLevel = 0;
    private int critLevel = 0;
    private int goldChanceLevel = 0;

    /// <summary>
    /// Рассчитываем текущую стоимость улучшения урона
    /// </summary>

    public int GetDamageUpgradeCost()
    {
        // baseDamageUpgradeCost * (1.5^damageLevel)
        return Mathf.RoundToInt(baseDamageUpgradeCost * Mathf.Pow(costMultiplier, damageLevel));
    }

    public int GetCritUpgradeCost()
    {
        return Mathf.RoundToInt(baseCritUpgradeCost * Mathf.Pow(costMultiplier, critLevel));
    }
    public int GetGoldChanceUpgradeCost()
    {
        return Mathf.RoundToInt(baseGoldChanceUpgradeCost * Mathf.Pow(costMultiplier, goldChanceLevel));
    }

    public void UpgradeDamage()
    {
        int cost = GetDamageUpgradeCost();
        if (playerManager.gold >= cost)
        {
            playerManager.gold -= cost;
            damageLevel++;

            // Увеличиваем урон клика
            playerManager.сlickDamage = playerManager.baseClickDamage + playerManager.baseClickDamage*damageIncrement*damageLevel;

            Debug.Log($"Upgraded Click Damage to {playerManager.сlickDamage} (Level: {damageLevel})");
        }
        else
        {
            Debug.Log("Not enough gold for Damage upgrade!");
        }
    }

    public void UpgradeCritChance()
    {
        int cost = GetCritUpgradeCost();
        if (playerManager.gold >= cost)
        {
            playerManager.gold -= cost;
            critLevel++;

            // Увеличиваем шанс крита
            playerManager.critChance += critChanceIncrement;

            // Ограничим шанс крита 100%
            if (playerManager.critChance > 100f)
                playerManager.critChance = 100f;

            Debug.Log($"Upgraded Crit Chance to {playerManager.critChance}% (Level: {critLevel})");
        }
        else
        {
            Debug.Log("Not enough gold for Crit upgrade!");
        }
    }
    public void UpgradeBonusGoldChance()
    {
        int cost = GetGoldChanceUpgradeCost();
        if (playerManager.gold >= cost)
        {
            playerManager.gold -= cost;
            goldChanceLevel++;

            // Увеличиваем шанс бонус. золота
            playerManager.bonusGoldChance += bonusGoldChanceIncrement;
            if (playerManager.bonusGoldChance > 100f)
                playerManager.bonusGoldChance = 100f;

            Debug.Log($"Upgraded Bonus Gold Chance to {playerManager.bonusGoldChance}% (Level: {goldChanceLevel})");
        }
        else
        {
            Debug.Log("Not enough gold for Bonus Gold upgrade!");
        }
    }
}