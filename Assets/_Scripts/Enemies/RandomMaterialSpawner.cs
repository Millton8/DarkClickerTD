using UnityEngine;

public class RandomMaterialSpawner : MonoBehaviour
{
    // Префаб, у которого в иерархии есть дочерний объект с рендерером
    [SerializeField] private GameObject prefab;

    // Массив материалов, заполните его в инспекторе
    [SerializeField] private Material[] materials;

    // Имя дочернего объекта (необязательно, можно найти любой рендерер GetComponentInChildren)
    [SerializeField] private string childName = "TargetChild";

    public void SpawnAt(Vector3 position, Quaternion rotation)
    {
        // Создаём копию префаба
        GameObject go = Instantiate(prefab, position, rotation);

        // 1) Поиск по имени дочернего трансформа
        //Transform child = go.transform.Find(childName);
        //if (child != null)
        //{
        // Пытаемся получить любой рендерер на найденном объекте
        //Renderer rend = child.GetComponent<Renderer>();
        Renderer rend = go.GetComponentInChildren<SkinnedMeshRenderer>();
        if (rend != null)
            {
                // Выбираем случайный материал
                Material mat = materials[Random.Range(0, materials.Length)];

                // Присваиваем материал. 
                // rend.material создаёт экземпляр материала для этого рендера,
                // а rend.sharedMaterial — заменит материал у всех объектов, 
                // которые на него ссылаются в проекте.
                rend.material = mat;
            }
            else
            {
                Debug.LogWarning($"У {childName} нет Renderer’а");
            }
        //}
        //else
        //{
        //    Debug.LogWarning($"Дочерний объект \"{childName}\" не найден");
        //}
    }

    // Для теста — спавним по нажатию клавиши
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnAt(Vector3.zero, Quaternion.identity);
        }
    }
}
