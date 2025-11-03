using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;       // Если вы используете TextMeshPro вместо стандартного Text
// Подключайте в зависимости от того, какой UI-компонент используется.

public class TowerShop : MonoBehaviour
{
    [Header("Tower Shop Settings")]
    public TowerInfo[] availableTowers;       // Список башен, доступных для покупки
    public GameObject shopPanel;              // Панель магазина (UI)
    public Button[] towerButtons;             // Кнопки для покупки (их можно создать заранее в UI)

    [Header("Placement Settings")]
    public Transform towerParent;             // Родительский объект для установленных башен (для организации иерархии)
    public TowerSlot[] towerSlots;            // Массив фиксированных слотов для установки башен

    private TowerInfo selectedTowerInfo = null;

    public UIManager manager;
    string debugMessage;

    private int towerBought = 0;
    private bool lastTowerSelected = true;
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
    void Start()
    {
        // Инициализация кнопок магазина
        for (int i = 0; i < towerButtons.Length; i++)
        {
            int index = i;
            if (index < availableTowers.Length)
            {
                towerButtons[index].GetComponentInChildren<Text>().text =
                    $"{availableTowers[index].towerName}\n Cost: {availableTowers[index].cost}"; // $"{availableTowers[index].towerName}\nCost: {availableTowers[index].cost}";
                towerButtons[index].onClick.AddListener(() => OnTowerButtonClicked(index));
            }
        }
        for (int i = 1; i < towerButtons.Length; i++)
        {
            int index = i;
            if (index < availableTowers.Length)
            {
                towerButtons[index].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Обработчик нажатия кнопки покупки башни.
    /// Если золота достаточно, сохраняем выбранную башню и ждём выбора свободного слота.
    /// </summary>
    void OnTowerButtonClicked(int index)
    {
        TowerInfo info = availableTowers[index];
        if (PlayerManager.Instance.gold >= info.cost)
        {
            selectedTowerInfo = info;
            Debug.Log($"Selected tower: {info.towerName}. Теперь выберите слот для размещения.");
            debugMessage = ($"Selected tower: {info.towerName}. Теперь выберите слот для размещения.");
            // Можно подсветить свободные слоты или показать инструкцию.
            if (index == towerBought)
                lastTowerSelected = true;
            else
                lastTowerSelected = false;
        }
        else
        {
            Debug.Log($"Недостаточно золота для покупки {info.towerName}!");
            debugMessage = ($"Недостаточно золота для покупки {info.towerName}!");
        }
        manager.messageText.text = debugMessage;
    }

    /// <summary>
    /// Пытаемся разместить выбранную башню в указанном слоте.
    /// Вызывается из TowerSlot при клике.
    /// </summary>
    public void TryPlaceTowerAtSlot(TowerSlot slot)
    {
        if (selectedTowerInfo == null)
        {
            Debug.Log("Башня не выбрана для размещения.");
            debugMessage = ("Башня не выбрана для размещения.");
            return;
        }

        if (slot.isOccupied)
        {
            Debug.Log("Этот слот уже занят.");
            debugMessage = ("Этот слот уже занят.");
            return;
        }

        if (PlayerManager.Instance.gold < selectedTowerInfo.cost)
        {
            Debug.Log("Недостаточно золота для покупки выбранной башни.");
            debugMessage = ("Недостаточно золота для покупки выбранной башни.");
            selectedTowerInfo = null;
            return;
        }
        manager.messageText.text = debugMessage;

        // Списываем золото
        PlayerManager.Instance.gold -= selectedTowerInfo.cost;

        // Размещаем башню: позиция берётся из слота, родитель — towerParent
        GameObject towerObj = Instantiate(selectedTowerInfo.towerPrefab, slot.transform.position, Quaternion.identity, towerParent);


        AudioManager.Instance.PlaySFX(AudioManager.Instance.towerBuildSound);  // sound effect playing

        // Если в префабе башни есть скрипт Tower, сохраняем его ссылку в слоте
        TowerBase tower = towerObj.GetComponent<TowerBase>();
        if (tower != null)
        {
            if (lastTowerSelected == true && towerBought < towerButtons.Length)
            {
                towerBought++; // надо проверять куплена ли последняя башня
                towerButtons[towerBought].gameObject.SetActive(true);
            }
            slot.isOccupied = true;
            slot.tower = tower;
            //slot.transform.localScale = Vector3.zero;
            //slot.enabled = false;
            Debug.Log($"Башня {selectedTowerInfo.towerName} установлена.");
            debugMessage = ($"Башня {selectedTowerInfo.towerName} установлена.");
        }
        else
        {
            Debug.LogWarning("В префабе отсутствует компонент Tower.");
            debugMessage = ("В префабе отсутствует компонент Tower.");
        }
        manager.messageText.text = debugMessage;
        // Сбрасываем выбранную башню
        selectedTowerInfo = null;
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
    private void UpdateUI() { }
}