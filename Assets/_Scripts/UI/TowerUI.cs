using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{

    [Header("UI Panel")]
    public GameObject uiPanel;           // Панель с кнопками и текстами

    [Header("Text Fields")]
    public Text damageCostText;          // Текстовая метка для цены улучшения урона
    public Text speedCostText;           // Текстовая метка для цены улучшения скорости
    public Text rangeCostText;           // Текстовая метка для цены улучшения дистанции
    public Text specialCostText;           // Текстовая метка для цены улучшения специального апгрейда


    [Header("Tower Current Parameters Fields")]
    public TextMeshProUGUI currentDamageText;
    public TextMeshProUGUI currentSpeedText;
    public TextMeshProUGUI currentRangeText;
    public TextMeshProUGUI currentSpecialText;      

    public TextMeshProUGUI towerName;

    [Header("Buttons")]
    public Button damageUpgradeButton;
    public Button speedUpgradeButton;
    public Button rangeUpgradeButton;
    public Button specialUpgradeButton;

    private TowerBase selectedTower;         // Текущая башня, которую мы улучшаем

    public CanvasGroup canvasGroup;
    private GameObject rangeIndicatorInstance;

    void Awake()
    {
        // Если CanvasGroup не задан в инспекторе, попробуем найти его на этом объекте
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        // Скрываем UI, но объект остаётся активным в сцене
        HideUI();
    }

    void Start()
    {
        // Скрываем панель UI по умолчанию
        /////////////uiPanel.SetActive(false);
        //uiPanel.transform.localScale = Vector3.zero;

        // Назначаем методы на кнопки
        damageUpgradeButton.onClick.AddListener(OnUpgradeDamage);
        speedUpgradeButton.onClick.AddListener(OnUpgradeSpeed);
        rangeUpgradeButton.onClick.AddListener(OnUpgradeRange);
        specialUpgradeButton.onClick.AddListener(OnUpgradeSpecial);
    }

    public void ShowUI(TowerBase tower)
    {
        selectedTower = tower;
        tower.ShowRangeIndicator();
        UpdateUI();
        // Делаем UI видимым и интерактивным
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        //rangeIndicatorInstance = FindFirstObjectByType<>
    }

    /// <summary>
    /// Скрывает UI, но оставляет объект активным.
    /// </summary>
    public void HideUI()
    {
        if (selectedTower != null)
        {
            selectedTower.HideRangeIndicator();
        }
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        selectedTower = null;
    }

    private void OnUpgradeDamage()
    {
        if (selectedTower != null)
        {
            selectedTower.UpgradeDamage();
            UpdateUI();
        }
    }

    private void OnUpgradeSpeed()
    {
        if (selectedTower != null)
        {
            selectedTower.UpgradeSpeed();
            UpdateUI();
        }
    }

    private void OnUpgradeRange()
    {
        if (selectedTower != null)
        {
            selectedTower.UpgradeRange();
            UpdateUI();
        }
    }

    private void OnUpgradeSpecial()
    {
        if (selectedTower != null)
        {
            selectedTower.UpgradeSpecial();
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (selectedTower == null)
            return;

        damageCostText.text = "Damage Cost: " + selectedTower.GetDamageUpgradeCost();
        speedCostText.text = "Speed Cost: " + selectedTower.GetSpeedUpgradeCost();
        rangeCostText.text = "Range Cost: " + selectedTower.GetRangeUpgradeCost();
        if (selectedTower.SpeicalUpgradeText != "")
        {
            specialCostText.text = selectedTower.SpeicalUpgradeText + " Cost: " + selectedTower.GetSpecialUpgradeCost();
            specialUpgradeButton.interactable = true;
        }
        else
        {
            specialCostText.text = "No Upgrades";
            specialUpgradeButton.interactable = false;
        }
        towerName.text = selectedTower.towerName;


        currentDamageText.text = "" + selectedTower.damage.ToString("0");
        currentSpeedText.text = "" + selectedTower.attackSpeed.ToString("0.00"); ;
        currentRangeText.text = "" + selectedTower.attackRange.ToString("0.00"); ;
        if (selectedTower.specialUpgradeValue == 0)
            currentSpecialText.text = "-";
        else
            currentSpecialText.text = "" + selectedTower.specialUpgradeValue.ToString("0.00");
    }
}
