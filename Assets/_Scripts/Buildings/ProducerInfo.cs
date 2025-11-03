using UnityEngine;

[System.Serializable]
public class ProducerInfo
{
    public string name;               // Название здания
    public GameObject prefab;         // Префаб (должен содержать компонент ProducerBuilding)
    public int baseCost = 50;         // Цена первой покупки
    public float baseRate = 1f;       // Древесины в секунду при уровне 1
    public float rateIncrement = 0.5f;// Доп. скорость производства при каждом уровне
    public float costMultiplier = 2f;// Множитель роста стоимости при апгрейде
}