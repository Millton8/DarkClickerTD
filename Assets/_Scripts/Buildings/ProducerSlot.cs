using UnityEngine;
using UnityEngine.EventSystems;

public class ProducerSlot : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public bool isOccupied = false;
    [HideInInspector] public ProducerBuilding building;
    public ProducerShop shop; // назначьте в Инспекторе

    public void OnPointerClick(PointerEventData eventData)
    {
        shop.TryPlaceOrUpgradeBuilding(this);
    }
}