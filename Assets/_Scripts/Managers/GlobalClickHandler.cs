using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GlobalClickHandler : MonoBehaviour
{
    private TowerUI towerUI;

    private TowerShop towerShop;
    //private LumberShop lumberShop;

    public GameObject towerShopButton;
    public GameObject towerShopImage;
    public GameObject towerShopPanel;

    //public GameObject lumberShopButton;
    //public LumberShopController lumberShopController;

    //public GameObject lumberShopImage;
    //public GameObject lumberShopPanel;

    // Ссылка на графический Raycaster для UI (будет взят с Canvas, содержащего towerBuyImage)
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;

    void Awake()
    {
        eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogError("EventSystem не найден в сцене!");
        }
        // Автоматически ищем TowerUI (например, он реализован как Singleton или через FindObjectOfType)
        towerUI = FindFirstObjectByType<TowerUI>();
        towerShop = FindFirstObjectByType<TowerShop>();
        //lumberShopController = FindFirstObjectByType<LumberShopController>();
        //lumberShop = FindFirstObjectByType<LumberShop>();
        if (towerUI == null)
        {
            Debug.LogWarning("TowerUI не найден в сцене!");
            Debug.Log("TowerUI не найден в сцене!");
        }

        if (towerShop == null)
        {
            Debug.LogWarning("TowerShop не найден в сцене!");
            Debug.Log("TowerShop не найден в сцене!");
        }
    }

    void Start()
    {
        // Находим Canvas, к которому принадлежит towerBuyImage, и берём с него компонент GraphicRaycaster
        if (towerShopButton != null)
        {
            Canvas canvas = towerShopButton.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
                if (graphicRaycaster == null)
                {
                    Debug.LogWarning("На Canvas не найден компонент GraphicRaycaster.");
                }
            }
            else
            {
                Debug.LogWarning("towerBuyImage не принадлежит ни одному Canvas.");
            }
        }
        else
        {
            Debug.LogError("towerBuyImage не назначен в инспекторе!");
        }
        // Находим Canvas, к которому принадлежит towerBuyImage, и берём с него компонент GraphicRaycaster
        if (towerShopImage != null)
        {
            Canvas canvas = towerShopImage.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
                if (graphicRaycaster == null)
                {
                    Debug.LogWarning("На Canvas не найден компонент GraphicRaycaster.");
                }
            }
            else
            {
                Debug.LogWarning("towerBuyImage не принадлежит ни одному Canvas.");
            }
        }
        else
        {
            Debug.LogError("towerBuyImage не назначен в инспекторе!");
        }

        // Панель покупки башен изначально скрыта
        if (towerShopPanel != null)
            //towerShopPanel.SetActive(false); 
            towerShop.HideUI();
    }



    void Update()
    {
        // Проверяем клик мышью (например, левый клик)
        if (Input.GetMouseButtonDown(0))
        {
            // Настраиваем PointerEventData с позицией мыши
            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            if (graphicRaycaster != null)
            {
                graphicRaycaster.Raycast(pointerEventData, results);
            }

            //bool clickedOnTowerShopImage = false;
            bool clickedOnTowerShopButton = false;
            //bool clickedOnLumberShopImage = false;
            //bool clickedOnLumberShopButton = false;

            // Проверяем, содержится ли в результатах Raycast объект TowerBuyImage или его потомок
            foreach (RaycastResult result in results)
            {
                /*if (result.gameObject == towerShopImage || result.gameObject.transform.IsChildOf(towerShopImage.transform))
                {
                    clickedOnTowerShopImage = true;
                    break;
                }*/
                if (result.gameObject == towerShopButton || result.gameObject.transform.IsChildOf(towerShopButton.transform))
                {
                    clickedOnTowerShopButton = true;
                    break;
                }

                /*if (result.gameObject == lumberShopImage || result.gameObject.transform.IsChildOf(lumberShopImage.transform))
                {
                    clickedOnLumberShopImage = true;
                    break;
                }*/
            }

            // Если клик был по TowerBuyImage – показываем панель, иначе скрываем её

            //else
            //{
            //    if (towerShopPanel != null)
            //        towerShopPanel.SetActive(false);
            //}
            //Debug.Log("GlobalClickHandler is working!");
            towerUI = FindFirstObjectByType<TowerUI>();
            if (towerUI == null)
            {
                Debug.LogWarning("TowerUI не найден в сцене!");
                Debug.Log("TowerUI не найден в сцене!"); 
                return;
            }

            towerShop = FindFirstObjectByType<TowerShop>();
            if (towerShop == null)
            {
                Debug.LogWarning("TowerShop не найден в сцене!");
                Debug.Log("TowerShop не найден в сцене!");
                return;
            }

            /*lumberShop = FindFirstObjectByType<LumberShop>();
            if (lumberShop == null)
            {
                Debug.LogWarning("LumberShop не найден в сцене!");
                Debug.Log("LumberShop не найден в сцене!");
                return;
            }*/

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (clickedOnTowerShopButton == false && EventSystem.current.currentSelectedGameObject != null)
            {
                // Если клик по UI, ничего не делаем (не скрываем панель)
                return;
            }

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (!hit.transform.CompareTag("Tower"))
                {
                    towerUI.HideUI();
                }
            }
            /*if (clickedOnTowerShopImage)
            {
                //    if (towerShopPanel != null)
                //        towerShopPanel.SetActive(true);
                towerShop.ShowUI();
            }
            else if(!hit.transform.CompareTag("TowerShop"))
            {
                towerShop.HideUI();
            }*/

            if (clickedOnTowerShopButton)
            {
                //    if (towerShopPanel != null)
                //        towerShopPanel.SetActive(true);
                towerShop.ShowUI();
            }
            else if (!hit.transform.CompareTag("TowerShop"))
            {
                towerShop.HideUI();
            }
            
            /*if (clickedOnLumberShopButton)
            {
                //    if (towerShopPanel != null)
                //        towerShopPanel.SetActive(true);
                lumberShopController.TogglePanel();
            }
            else if (!hit.transform.CompareTag("LumberShop"))
            {
                lumberShopController.TogglePanel();
            }*/
            


        }
    }
}