using System.Resources;
using UnityEngine;

public class ProducerBuilding : MonoBehaviour
{
    [HideInInspector] public ProducerInfo info;
    [HideInInspector] public int level = 1;

    private float timer = 0f;
    private float productionRate; // текущее производство в секунду

    void Start()
    {
        productionRate = info.baseRate;
    }

    void Update()
    {
        // копим врем€ и каждые 1с добавл€ем дерево
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            ResourceManager.Instance.AddWood(Mathf.RoundToInt(productionRate));
            timer -= 1f;
        }
    }

    // –ассчитывает текущую стоимость следующего уровн€
    public int GetUpgradeCost()
    {
        return Mathf.RoundToInt(info.baseCost * Mathf.Pow(info.costMultiplier, level - 1));
    }

    // јпгрейд: тратим дерево, увеличиваем уровень и скорость
    public bool Upgrade()
    {
        int cost = GetUpgradeCost();
        if (!ResourceManager.Instance.CanAfford(cost)) return false;
        ResourceManager.Instance.SpendWood(cost);
        level++;
        productionRate += info.rateIncrement;
        return true;
    }
}
