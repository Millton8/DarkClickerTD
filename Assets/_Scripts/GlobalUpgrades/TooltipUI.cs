using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    [Header("UI References")]
    public CanvasGroup canvasGroup;    // для управления видимостью
    public TextMeshProUGUI tooltipText;           // сам текст подсказки
    public RectTransform background;   // фон подсказки (чтобы менять размер и позицию)

    private RectTransform canvasRect;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Отключаем подсказку изначально
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Предположим, что этот объект — ребёнок Canvas
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    /// <summary>
    /// Показывает подсказку с текстом description рядом с указателем.
    /// </summary>
    public void Show(string description, Vector2 screenPosition)
    {
        tooltipText.text = description;

        // Подгоняем фон под размер текста
        LayoutRebuilder.ForceRebuildLayoutImmediate(background);

        // Преобразуем экранные координаты в локальные координаты Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPosition, null, out Vector2 localPoint);

        background.anchoredPosition = localPoint + new Vector2(background.rect.width / 2, -background.rect.height / 2);

        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Скрывает подсказку.
    /// </summary>
    public void Hide()
    {
        canvasGroup.alpha = 0f;
    }
}