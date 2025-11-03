using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LumberShopController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Кнопка, которая открывает/закрывает панель LumberShop")]
    public Button activationButton;
    [Tooltip("RectTransform панели LumberShopPanel")]
    public RectTransform panelRect;

    [Header("Animation Settings")]
    [Tooltip("Время выезда/уезда панели (сек)")]
    public float animationDuration = 0.5f;

    // Internal  
    private Vector2 hiddenPos;
    private Vector2 visiblePos;
    private bool isPanelVisible = false;
    private bool isAnimating = false;
    private Coroutine currentRoutine;

    void Awake()
    {
        // Запомним «видимую» позицию (та, где панель в экране)
        visiblePos = panelRect.anchoredPosition;
        // Спрячем панель вправо за экран (по ширине)
        hiddenPos = visiblePos + Vector2.right * panelRect.rect.width;
        panelRect.anchoredPosition = hiddenPos;
        panelRect.gameObject.SetActive(false);

        // Подпишемся на кнопку
        activationButton.onClick.AddListener(TogglePanel);
    }

    void Update()
    {
        // Если панель видима и не в анимации — ловим клики вне её
        if (isPanelVisible && !isAnimating && Input.GetMouseButtonDown(0))
        {
            // Проверяем, попал ли клик внутрь панели
            bool inside = RectTransformUtility.RectangleContainsScreenPoint(
                panelRect,
                Input.mousePosition,
                null  // для Screen Space Overlay
            );

            if (!inside)
                HidePanel();
        }
    }

    private void TogglePanel()
    {
        if (isAnimating) return;

        if (isPanelVisible) HidePanel();
        else ShowPanel();
    }

    private void ShowPanel()
    {
        if (isAnimating || isPanelVisible) return;

        panelRect.gameObject.SetActive(true);
        isAnimating = true;
        currentRoutine = StartCoroutine(AnimatePanel(hiddenPos, visiblePos, () =>
        {
            isAnimating = false;
            isPanelVisible = true;
        }));
    }

    private void HidePanel()
    {
        if (isAnimating || !isPanelVisible) return;

        isAnimating = true;
        currentRoutine = StartCoroutine(AnimatePanel(visiblePos, hiddenPos, () =>
        {
            isAnimating = false;
            isPanelVisible = false;
            panelRect.gameObject.SetActive(false);
        }));
    }

    private IEnumerator AnimatePanel(Vector2 from, Vector2 to, Action onComplete)
    {
        float elapsed = 0f;
        panelRect.anchoredPosition = from;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            panelRect.anchoredPosition = Vector2.Lerp(from, to, elapsed / animationDuration);
            yield return null;
        }

        panelRect.anchoredPosition = to;
        onComplete?.Invoke();
    }
}


/*
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LumberShopController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("RectTransform панели LumberShopPanel")]
    public RectTransform panelRect;
    [Tooltip("Кнопка (или Image с Button), по нажатию на которую показывается/скрывается панель")]
    public Button activationButton;

    [Header("Animation Settings")]
    [Tooltip("Длительность анимации выезда/уезда")]
    public float animationDuration = 0.5f;

    private Vector2 hiddenPos;
    private Vector2 visiblePos;
    private Coroutine animationRoutine;

    void Awake()
    {
        // Запоминаем «видимую» позицию (ту, в которой панель должна стоять на экране)
        visiblePos = panelRect.anchoredPosition;
        // Считаем «скрытую» позу — полностью вправо за пределами экрана
        hiddenPos = new Vector2(visiblePos.x + panelRect.rect.width, visiblePos.y);
        // Сразу прячем панель
        panelRect.anchoredPosition = hiddenPos;

        // Подписываемся на клик
        activationButton.onClick.AddListener(TogglePanel);
    }

    /// <summary>
    /// Переключает состояние панели: если скрыта — выезжает, иначе — уезжает.
    /// </summary>
    public void TogglePanel()
    {
        // Если анимация ещё идёт — прерываем
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        // Решаем, куда анимировать
        Vector2 from = panelRect.anchoredPosition;
        Vector2 to = (from == hiddenPos) ? visiblePos : hiddenPos;

        animationRoutine = StartCoroutine(AnimatePanel(from, to));
    }

    private IEnumerator AnimatePanel(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            panelRect.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }
        // В конце ставим точно «to»
        panelRect.anchoredPosition = to;
        animationRoutine = null;
    }
}
*/