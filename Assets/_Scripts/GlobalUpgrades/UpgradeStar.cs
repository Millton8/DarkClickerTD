using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeStar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Definition")]
    public GlobalUpgradeDefinition definition;

    [Header("UI Elements")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public Image iconImage; // по желанию

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        Refresh();
    }

    public void Refresh()
    {
        bool unlocked = GlobalUpgradeManager.Instance.IsUnlocked(definition.id);
        bool canUnlock = GlobalUpgradeManager.Instance.CanUnlock(definition.id);

        nameText.text = definition.upgradeName;
        costText.text = unlocked ? "Unlocked"
                        : canUnlock
                            ? $"Cost: {definition.cost}"
                            : "Locked";

        button.interactable = !unlocked && canUnlock;
        // Пример визуала: изменяем цвет звезды
        GetComponent<Image>().color = unlocked
            ? Color.yellow
            : canUnlock
                ? Color.white
                : Color.gray;
    }

    private void OnClick()
    {
        if (GlobalUpgradeManager.Instance.TryUnlock(definition.id))
        {
            Refresh();
            // После разблокирования нужно обновить ВСЕ звёзды, 
            // чтобы открылись зависимые апгрейды:
            foreach (var star in FindObjectsByType<UpgradeStar>(FindObjectsSortMode.None))
                star.Refresh();
        }

        // найдем контроллер и обновим линии
        //var connector = FindObjectOfType<ConstellationConnector>();
        var connector = FindFirstObjectByType<ConstellationConnector>();
        if (connector != null)
            connector.RefreshConnections();
    }

    // Когда курсор навёлся — показываем описание
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null)
            TooltipUI.Instance.Show(definition.description, eventData.position);
    }

    // Когда курсор ушёл — скрываем подсказку
    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null)
            TooltipUI.Instance.Hide();
    }
}