using UnityEngine;
using UnityEngine.UI;

public class ClickUpgradeUI : MonoBehaviour
{
    [Header("Upgrade Manager")]
    public ClickUpgradeManager upgradeManager;

    [Header("Player Upgrades Panel")]
    public GameObject playerUpgradesPanel;           // Панель с кнопками и текстами

    [Header("Text Fields")]
    public Text damageCostText;
    public Text critCostText;
    public Text goldChanceCostText;

    [Header("Buttons")]
    public Button damageUpgradeButton;
    public Button critUpgradeButton;
    public Button goldChanceUpgradeButton;

    private Castle PlayerCastle;         // Выбран замок

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerUpgradesPanel.SetActive(false); //////////////////////
        // Вешаем слушателей на кнопки
        damageUpgradeButton.onClick.AddListener(OnUpgradeDamage);
        critUpgradeButton.onClick.AddListener(OnUpgradeCrit);
        goldChanceUpgradeButton.onClick.AddListener(OnUpgradeGoldChance);

        // Первый апдейт UI
        UpdatePlayerUI();
    }
    public void ShowUI(Castle castle)
    {
        PlayerCastle = castle;
        UpdatePlayerUI();
        playerUpgradesPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerUI();
        playerUpgradesPanel.SetActive(true);
    }

    private void UpdatePlayerUI()
    {
        //if (PlayerCastle == null)
        //    return;

            damageCostText.text = "Damage Cost: " + upgradeManager.GetDamageUpgradeCost();
            critCostText.text = "Crit Cost: " + upgradeManager.GetCritUpgradeCost();
            goldChanceCostText.text = "Gold+ Cost: " + upgradeManager.GetGoldChanceUpgradeCost();
    }

    public void HidePlayerUI()
    {
        playerUpgradesPanel.SetActive(false);
    }
    private void OnUpgradeDamage()
    {
        if (upgradeManager != null)
        {
            upgradeManager.UpgradeDamage();
            UpdatePlayerUI();
        }
    }

    private void OnUpgradeCrit()
    {
        if (upgradeManager != null)
        {
            upgradeManager.UpgradeCritChance();
            UpdatePlayerUI();
        }
    }

    private void OnUpgradeGoldChance()
    {
        if (upgradeManager != null)
        {
            upgradeManager.UpgradeBonusGoldChance();
            UpdatePlayerUI();
        }
    }
}
