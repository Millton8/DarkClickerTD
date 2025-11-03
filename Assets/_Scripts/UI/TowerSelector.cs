using UnityEngine;
using UnityEngine.EventSystems; // Для проверки кликов по UI

//[RequireComponent(typeof(TowerUI))]

public class TowerSelector : MonoBehaviour, IPointerClickHandler//, IPointerUpHandler, IPointerDownHandler
{
    //private TowerBase tower;

    //public TowerUI towerUI;
    public TowerUI towerUI;
    //public GameObject towerCanvas;
    //public GameObject uiPanel;
    //public GameObject towerUI;


    void Awake()
    {
        if (towerUI == null)
        {
            towerUI = FindFirstObjectByType<TowerUI>();
            if (towerUI == null)
                Debug.LogWarning("TowerUI не найден в сцене!");
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (towerUI == null)
        {
            towerUI = FindFirstObjectByType<TowerUI>();
            if (towerUI == null)
                Debug.LogWarning("TowerUI не найден в сцене!");
        }
        // Получаем объект, по которому кликнули (это гарантированно башня, если TowerSelector висит на ней)
        TowerBase tower = eventData.pointerCurrentRaycast.gameObject.GetComponent<TowerBase>()
                          ?? eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<TowerBase>();
        if (tower != null)
        {
            towerUI.ShowUI(tower);
        }
    }
}