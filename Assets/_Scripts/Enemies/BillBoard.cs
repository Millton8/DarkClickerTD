using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Awake()
    {
        // Если камера не задана вручную, ищем основную камеру
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Делаем так, чтобы объект всегда смотрел на камеру.
            // Здесь мы просто выставляем его forward равным forward камеры.
            transform.forward = mainCamera.transform.forward;
        }
    }
}