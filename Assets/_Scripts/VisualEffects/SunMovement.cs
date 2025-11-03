using UnityEngine;

[ExecuteAlways]
public class SunMovement : MonoBehaviour
{
    [Tooltip("ƒлительность Ђсутокї в секундах")]
    public float dayDuration = 1200f;

    [Tooltip("Ќачальный угол солнца (в градусах) Ч обычно примерно Ц90 дл€ восхода)")]
    public float sunriseAngle = -90f;

    [Tooltip(" онечный угол солнца (в градусах) Ч обычно примерно 270 дл€ полного оборота)")]
    public float sunsetAngle = 270f;

    // ¬нутренний счЄтчик времени (0Е1)
    [Range(0f, 1f)]
    public float timeOfDay = 0f;

    private void Update()
    {
        if (Application.isPlaying)
        {
            // ѕрибавл€ем долю Ђсутокї за кадр
            timeOfDay += Time.deltaTime / dayDuration;
            if (timeOfDay > 1f) timeOfDay -= 1f;
        }

        // ¬ычисл€ем текущий угол по линейной интерпол€ции
        float currentAngle = Mathf.Lerp(sunriseAngle, sunsetAngle, timeOfDay);

        // ѕримен€ем поворот Ч ось X отвечает за подъЄм/опускание солнца
        transform.rotation = Quaternion.Euler(currentAngle, transform.rotation.eulerAngles.x, 15f);
    }
}