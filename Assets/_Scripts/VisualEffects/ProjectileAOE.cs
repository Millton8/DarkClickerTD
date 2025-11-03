using UnityEngine;

public class ProjectileAOE : MonoBehaviour
{
    public float speed = 10f;                  // Скорость движения снаряда
    public GameObject explosionEffectPrefab;   // Префаб эффекта взрыва (например, Particle System)

    private float damage;                      // Урон, наносимый снарядом
    private float explosionRadius;             // Радиус взрыва
    private Vector3 targetPosition;            // Целевая позиция, к которой летит снаряд
    private bool hasExploded = false;

    /// <summary>
    /// Инициализирует снаряд, задавая цель, урон и радиус взрыва.
    /// </summary>
    public void Initialize(Transform target, float damage, float explosionRadius)
    {
        this.damage = damage;
        this.explosionRadius = explosionRadius;
        if (target != null)
        {
            targetPosition = target.position;
        }
        else
        {
            targetPosition = transform.position;
        }
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Двигаем снаряд вперёд по его оси
        float step = speed * Time.deltaTime;
        transform.position += transform.forward * step;

        // Если снаряд приблизился к целевой позиции, запускаем взрыв
        if (!hasExploded && Vector3.Distance(transform.position, targetPosition) < 0.15f)
        {
            Explode();
        }
    }

    /// <summary>
    /// При столкновении с любым объектом (например, с препятствием или врагом) запускаем взрыв.
    /// Обязательно установите Collider с IsTrigger = true.
    /// </summary>
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
            if (!hasExploded)
            {
                Explode();
            }
    }
    

    /// <summary>
    /// Выполняет взрыв: создаёт эффект, наносит урон всем врагам в радиусе и уничтожает снаряд.
    /// </summary>
    private void Explode()
    {
        hasExploded = true;

        // Создаем эффект взрыва
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Наносим урон всем врагам в радиусе взрыва
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}