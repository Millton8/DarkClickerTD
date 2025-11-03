using UnityEngine;

public class MultiShotTower : TowerBase
{
    private GameObject towerTop;
    public Transform shootPoint;
    private Quaternion newRotation;

    public int projectileCount = 3; // Количество снарядов в веере
    public float spreadAngle = 60f; // Угол разлета снарядов

    protected override void Start()
    {
        towerTop = this.transform.Find("TowerHead").gameObject;
        newRotation = towerTop.transform.rotation;
    }

    protected override void Update()
    {
        ShootCooldownCalculate();
        /*currentAttackCooldown -= Time.deltaTime;
        if (currentAttackCooldown <= 0)
        {
            Attack();
            currentAttackCooldown = 1f / attackSpeed;
        }*/
        towerTop.transform.rotation = Quaternion.Slerp(towerTop.transform.rotation, newRotation, Time.deltaTime * 10f);
    }

    protected override void Attack()
    {
        Enemy target = FindTarget();
        if (target != null)
        {
            shootOnCooldown = true;
            Vector3 baseDirection = (target.transform.position - shootPoint.position).normalized;

            for (int i = 0; i < projectileCount; i++)
            {
                // Распределяем снаряды по угловому диапазону spreadAngle
                float angleOffset = -spreadAngle / 2f;
                if (projectileCount > 1)
                {
                    angleOffset += (spreadAngle / (projectileCount - 1)) * i;
                }
                // Вычисляем новое направление с поворотом на angleOffset градусов вокруг оси Z
                Vector3 shotDirection = Quaternion.Euler(0, 0, angleOffset) * baseDirection;
                Quaternion shotRotation = Quaternion.LookRotation(shotDirection);

                // Создаём снаряд с заданной ориентацией
                GameObject projectileMultiBall = Instantiate(projectilePrefab, shootPoint.position, shotRotation);

                // Передаём снаряду рассчитанное направление и урон
                // Для этого изменяем метод инициализации снаряда, чтобы принимать вектор направления, а не Transform цели
                projectileMultiBall.GetComponent<ProjectileMultiBall>().InitializeDirection(shotDirection, damage);


                AudioManager.Instance.PlaySFX(AudioManager.Instance.MultiShotTowerShootSound); // sound effect playing
            }
        }
    }
    private Enemy FindTarget()
    {
        // Простой поиск цели: ищем ближайшего врага в радиусе attackRange
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        Enemy closest = null;
        float closestDistance = Mathf.Infinity;
        foreach (Collider hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < closestDistance)
                {
                    newRotation = Quaternion.LookRotation(enemy.gameObject.transform.position - towerTop.transform.position, Vector3.forward);
                    closest = enemy;
                    closestDistance = dist;
                }
            }
        }
        return closest;
    }
}