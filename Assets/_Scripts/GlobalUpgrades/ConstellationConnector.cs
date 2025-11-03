using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ConstellationConnector : MonoBehaviour
{
    [Header("Line Settings")]
    [Tooltip("UI-префаб: простой Image со спрайтом белого пикселя")]
    public GameObject linePrefab;
    public float lineThickness = 2f;
    public Color activeColor = Color.yellow;
    public Color inactiveColor = Color.gray;

    // Временный список созданных линий
    private readonly List<GameObject> lines = new();

    void Start()
    {
        RefreshConnections();
    }

    /// <summary>
    /// Перерисовывает все связи между UpgradeStar на панели.
    /// </summary>
    public void RefreshConnections()
    {
        // Удаляем старые линии
        foreach (var l in lines) Destroy(l);
        lines.Clear();

        // Собираем все звёздочки в словарь по ID
        var stars = GetComponentsInChildren<UpgradeStar>();
        var dict = stars.ToDictionary(
            s => s.definition.id,
            s => s.GetComponent<RectTransform>()
        );

        foreach (var star in stars)
        {
            var toRect = star.GetComponent<RectTransform>();
            foreach (var prereq in star.definition.prerequisites)
            {
                // Если у нас есть звезда-предок
                if (!dict.TryGetValue(prereq.id, out var fromRect))
                    continue;

                // Создаём линию
                var line = Instantiate(linePrefab, transform);
                line.transform.SetAsFirstSibling();
                lines.Add(line);

                // Выставляем имя для отладки
                line.name = $"Line_{prereq.id}_to_{star.definition.id}";

                // Выбираем цвет: если prerequisite открыт — активный, иначе — неактивный
                var img = line.GetComponent<Image>();
                bool prereqUnlocked = GlobalUpgradeManager.Instance.IsUnlocked(prereq.id);
                img.color = prereqUnlocked ? activeColor : inactiveColor;

                // Расчёт позиции и поворота
                var rt = line.GetComponent<RectTransform>();
                Vector2 p1 = fromRect.anchoredPosition;
                Vector2 p2 = toRect.anchoredPosition;
                Vector2 dir = p2 - p1;
                float dist = dir.magnitude;
                Vector2 mid = p1 + dir * 0.5f;

                rt.sizeDelta = new Vector2(dist, lineThickness);
                rt.anchoredPosition = mid;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                rt.localRotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}