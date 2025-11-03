using UnityEngine;

public class LumberShop : MonoBehaviour
{

    public CanvasGroup canvasGroup;

    void Awake()
    {
        // Если CanvasGroup не задан в инспекторе, попробуем найти его на этом объекте
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        // Скрываем UI, но объект остаётся активным в сцене
        //HideUI();
    }
    public void ShowUI()
    {
        //selectedTower = tower;
        //tower.ShowRangeIndicator();
        //UpdateUI();
        // Делаем UI видимым и интерактивным
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        //rangeIndicatorInstance = FindFirstObjectByType<>
    }
    public void HideUI()
    {
        //if (selectedTower != null)
        //{
        //    selectedTower.HideRangeIndicator();
        //}
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        //selectedTower = null;
    }
}
