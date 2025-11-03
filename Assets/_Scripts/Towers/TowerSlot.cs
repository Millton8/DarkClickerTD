using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSlot : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector]
    public bool isOccupied = false;
    [HideInInspector]
    public TowerBase tower; // Установленная башня

    public TowerShop towerShop; // Ссылка на менеджер магазина (назначается в инспекторе)

    public void OnPointerClick(PointerEventData eventData)
    {
        if (towerShop != null)
        {
            towerShop.TryPlaceTowerAtSlot(this);
        }
    }
}