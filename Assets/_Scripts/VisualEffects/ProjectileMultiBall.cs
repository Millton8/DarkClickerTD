using UnityEngine;
using UnityEngine.EventSystems;

public class ProjectileMultiBall : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;
    private float damage;
    private Vector3 moveDirection;

    // Флаг, чтобы снаряд не нанес урон несколько раз
    private bool hasHit = false;

    public float lifetime = 5f;     // Максимальное время жизни снаряда (в секундах)
    public float maxRange = 10f;    // Максимальное расстояние, которое снаряд может пролететь
    private float timeAlive = 0f;   // Счётчик времени существования
    private Vector3 startPosition;  // Начальная позиция снаряда

    private void Start()
    {
        startPosition = transform.position;
    }
    /// <summary>
    /// Альтернативный метод инициализации снаряда по цели.
    /// Вычисляет направление от позиции снаряда до позиции цели.
    /// </summary>
    public void Initialize(Transform target, float damage)
    {
        if (target != null)
        {
            moveDirection = (target.position - transform.position).normalized;
        }
        this.damage = damage;
    }

    public void InitializeDirection(Vector3 direction, float damage)
    {
        moveDirection = direction.normalized;
        this.damage = damage;
    }


    void Update()
    {
        //transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        // Двигаем снаряд по заданному направлению
        transform.position += moveDirection * speed * Time.deltaTime;


        // Проверяем, если снаряд существует дольше, чем lifetime, или пролетел больше, чем maxRange
        if (timeAlive >= lifetime || Vector3.Distance(startPosition, transform.position) >= maxRange)
        {
            Destroy(gameObject);
        }

        //if (Vector3.Distance(transform.position, target.position) < 0.5f)
        //{
        //    HitTarget();
        //}
    }


    private void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;

        // Пытаемся получить компонент Enemy
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            HitTarget(enemy);
        }
    }


    /// <summary>
    /// Наносит урон врагу и уничтожает снаряд.
    /// </summary>
    private void HitTarget(Enemy enemy)
    {
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        hasHit = true;
        Destroy(gameObject);
    }
}
