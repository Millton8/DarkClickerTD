using UnityEngine;

[System.Serializable]
public class TowerInfo
{
    public string towerName;           // Имя башни (для отображения)
    public GameObject towerPrefab;     // Префаб башни (с настройками внешнего вида и параметрами)
    public int cost;                   // Стоимость покупки башни
}