using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LumberPanelController : MonoBehaviour
{
    public static LumberPanelController Instance;

    [Header("UI Elements")]
    [Tooltip("RectTransform панели LumberTextImage")]
    public RectTransform panelRect;
    [Tooltip("Текстовый компонент внутри панели")]
    //public Text guideText;
    public TextMeshProUGUI lumberText;

    [Header("Animation Settings")]
    [Tooltip("Время выезда/уезда панели")]
    public float animationDuration = 0.5f;
    [Tooltip("Сколько секунд панель остаётся видимой")]
    public float displayDuration = 5f;

    private Vector2 hiddenPos;
    private Vector2 visiblePos;
    private Coroutine currentRoutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Предполагаем, что RectTransform.anchorMin.y = 1, anchorMax.y = 1 (панель сверху)
        // visiblePos — например, (0, 0), hiddenPos — (0, panelHeight)
        visiblePos = panelRect.anchoredPosition;
        hiddenPos = visiblePos + Vector2.right * panelRect.rect.width * 2;

        // Изначально скрываем панель за экраном
        panelRect.anchoredPosition = hiddenPos;
    }

    /// <summary>
    /// Показывает панель с сообщением, анимационно выезжая сверху, через displayDuration скрывает.
    /// </summary>
    public void Show(string message)
    {
        //lumberText.text = message;
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ShowAndHideRoutine());
    }

    private IEnumerator ShowAndHideRoutine()
    {
        // Анимируем выезд
        yield return StartCoroutine(AnimatePosition(hiddenPos, visiblePos));
        // Ждём displayDuration
        yield return new WaitForSeconds(displayDuration);
        // Анимируем уезд
        yield return StartCoroutine(AnimatePosition(visiblePos, hiddenPos));
        currentRoutine = null;
    }

    private IEnumerator AnimatePosition(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            panelRect.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }
        panelRect.anchoredPosition = to;
    }
}