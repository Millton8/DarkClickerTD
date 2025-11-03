using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConstellationShopController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Кнопка, по которой открываем/активируем панель")]
    public Button constellationShopButton;

    [Tooltip("Панель UpgradeConstellationPanel")]
    public GameObject upgradePanel;
    private CanvasGroup panelCanvasGroup;
    private RectTransform panelRect; // добавьте это поле

    [Header("Animation")]
    [Tooltip("Длительность фейда (появление/скрытие)")]
    public float fadeDuration = 0.5f;

    // Для UI-raycast
    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    private Coroutine currentFade;

    private bool isPanelVisible = false;
    private bool isAnimating = false;

    [Header("Drag & Zoom")]
    public float zoomStep = 0.1f;       // шаг изменения масштаба
    public float minZoom = 0.1f;
    public float maxZoom = 10f;

    private bool isDragging = false;
    private Vector2 dragStartMouseLocal;
    private Vector2 panelStartPos;


    void Awake()
    {
        // Получаем CanvasGroup на панели
        panelCanvasGroup = upgradePanel.GetComponent<CanvasGroup>();
        panelRect = upgradePanel.GetComponent<RectTransform>();

        if (panelCanvasGroup == null)
        {
            Debug.LogError("UpgradeConstellationPanel должен иметь CanvasGroup!");
            return;
        }


        // Изначально панель скрыта
        upgradePanel.SetActive(false);
        panelCanvasGroup.alpha = 0f;

        // Подписываем кнопку открытия
        constellationShopButton.onClick.AddListener(TogglePanel);

        // Для определения кликов по UI
        eventSystem = EventSystem.current;
        raycaster = upgradePanel.GetComponentInParent<GraphicRaycaster>();
        if (raycaster == null)
            Debug.LogError("На Canvas с панелью нет GraphicRaycaster!");
    }

    void Update()
    {
        // 1) Начало Drag
        if (isPanelVisible && !isAnimating && Input.GetMouseButtonDown(0))
        {
            if (IsPointerInsidePanel() && !IsPointerOverStar())
            {
                StartDrag();
                // не выезжаем/скрываем
                return;
            }
        }

        // 2) Drag продолжается
        if (isDragging && Input.GetMouseButton(0))
        {
            UpdateDrag();
        }

        // 3) Конец Drag
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // 4) Проверка клика вне для скрытия (только если не начали Drag)
        if (isPanelVisible && !isAnimating && !isDragging && Input.GetMouseButtonDown(0))
        {
            if (!IsPointerInsidePanel())
                HidePanel();
        }

        // 5) Zoom колесиком, когда курсор внутри панели
        if (isPanelVisible)
        {
            if (IsPointerInsidePanel())
            {
                float scroll = Input.mouseScrollDelta.y;
                if (Mathf.Abs(scroll) > Mathf.Epsilon)
                    ApplyZoom(scroll);
            }
        }

        /*
        // Обрабатываем только если панель видна, не анимируется и клик
        if (isPanelVisible && !isAnimating && Input.GetMouseButtonDown(0))
        {
            // Проверяем, попал ли клик внутрь панели
            bool clickInsidePanel = RectTransformUtility.RectangleContainsScreenPoint(
                panelRect,
                Input.mousePosition,
                // если Canvas Screen Space Overlay, то null; 
                // иначе — ваша UI-камера, например: Camera.main
                null
            );

            if (!clickInsidePanel)
                HidePanel();
        }

        */
    }

    private void TogglePanel()
    {
        if (isAnimating) return;

        if (isPanelVisible)
            HidePanel();
        else
            ShowPanel();
    }

    private void ShowPanel()
    {
        if (isAnimating || isPanelVisible) return;

        if (currentFade != null) StopCoroutine(currentFade);
        upgradePanel.SetActive(true);
        isAnimating = true;
        currentFade = StartCoroutine(Fade(0f, 1f, () =>
        {
            isAnimating = false;
            isPanelVisible = true;
        }));
    }

    private void HidePanel()
    {
        if (isAnimating || !isPanelVisible) return;

        if (currentFade != null) StopCoroutine(currentFade);
        isAnimating = true;
        currentFade = StartCoroutine(Fade(1f, 0f, () =>
        {
            upgradePanel.SetActive(false);
            isAnimating = false;
            isPanelVisible = false;
        }));
    }

    private IEnumerator Fade(float from, float to, System.Action onComplete)
    {
        float elapsed = 0f;
        panelCanvasGroup.alpha = from;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        panelCanvasGroup.alpha = to;
        onComplete?.Invoke();
    }

    // --- Drag Helpers ---

    private void StartDrag()
    {
        isDragging = true;
        // запомним точку клика в локальных координатах родителя
        RectTransform parentRect = panelRect.parent as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, Input.mousePosition, null, out dragStartMouseLocal);
        panelStartPos = panelRect.anchoredPosition;
    }

    private void UpdateDrag()
    {
        RectTransform parentRect = panelRect.parent as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, Input.mousePosition, null, out Vector2 localMousePos);
        Vector2 delta = localMousePos - dragStartMouseLocal;
        panelRect.anchoredPosition = panelStartPos + delta;
    }

    // --- Zoom Helper ---

    private void ApplyZoom(float scrollDelta)
    {
        float scale = panelRect.localScale.x;
        float newScale = Mathf.Clamp(scale + scrollDelta * zoomStep, minZoom, maxZoom);
        panelRect.localScale = Vector3.one * newScale;
    }

    // --- Pointer Checks ---

    private bool IsPointerInsidePanel()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            panelRect, Input.mousePosition, null);
    }

    private bool IsPointerOverStar()
    {
        // Raycast UI под курсором
        var ped = new PointerEventData(eventSystem) { position = Input.mousePosition };
        var results = new List<RaycastResult>();
        raycaster.Raycast(ped, results);

        foreach (var r in results)
        {
            if (r.gameObject.GetComponentInParent<UpgradeStar>() != null)
                return true;
        }
        return false;
    }
}