using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserEffect : MonoBehaviour
{
    public float duration = 0.05f; // Время жизни эффекта лазера (секунд)
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Устанавливает начальную и конечную точки луча.
    /// </summary>
    public void Setup(Vector3 startPos, Vector3 endPos)
    {
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    void Start()
    {
        Destroy(gameObject, duration);
    }
}