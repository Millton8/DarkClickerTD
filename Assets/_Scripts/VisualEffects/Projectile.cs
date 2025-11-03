using UnityEngine;
using UnityEngine.EventSystems;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;
    private float damage;

    public void Initialize(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // ”ничтожить снар€д, если цель исчезла
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        //transform.position += moveDirection * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        Destroy(gameObject); // ”ничтожить снар€д после попадани€
    }
}
