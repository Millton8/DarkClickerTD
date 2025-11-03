using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTower : TowerBase
{
    [Header("Ice Tower Settings")]
    public float baseSlow = 0.1f;       // Сила замедления: 10% по умолчанию
    public float slowDuration = 2f;       // Длительность замедления (в секундах)
    public float slowUpgradeIncrement = 0.05f; // Приращение замедления за апгрейд (5% за уровень)

    private GameObject towerTop;
    private Quaternion newRotation;

    private int slowLevel = 0;
    public float currentSlow = 0.1f;

    protected override void Start()
    {
        towerTop = this.transform.Find("TowerHead").gameObject;
        //newRotation = towerTop.transform.rotation;
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
        towerTop.transform.Rotate(Vector3.up * 10f * Time.deltaTime);
    }

    // Переопределяем метод атаки для IceTower
    protected override void Attack()
    {
        Enemy target = FindRandomTarget();
        if (target != null)
        {
            shootOnCooldown = true;
            GameObject projectile = Instantiate(projectilePrefab, towerTop.transform.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialize(target.transform, damage);
            // При желании можно нанести урон (если IceTower должна и наносить урон)
            //target.TakeDamage(damage);

            // Применяем эффект замедления к выбранной цели
            target.ApplySlowEffect(currentSlow, slowDuration);

            AudioManager.Instance.PlaySFX(AudioManager.Instance.IceTowerShootSound); // sound effect playing
            // Здесь можно добавить визуальные эффекты (например, частицы льда) – не реализовано в этом примере.
        }
    }

    /// <summary>
    /// Находит случайного врага в пределах зоны атаки.
    /// </summary>
    private Enemy FindRandomTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        List<Enemy> enemies = new List<Enemy>();
        foreach (Collider hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead())
            {
                enemies.Add(enemy);
            }
        }
        if (enemies.Count > 0)
        {
            int index = Random.Range(0, enemies.Count);
            return enemies[index];
        }
        return null;
    }


    //public override int GetSpecialUpgradeCost()
    //{
    //    return Mathf.RoundToInt(baseSlow * Mathf.Pow(costMultiplier, slowLevel));
    //}


    /// <summary>
    /// Метод для улучшения силы замедления.
    /// При каждом апгрейде slowFactor увеличивается, но не может превышать 0.5 (50%).
    /// </summary>
    public override void UpgradeSpecial()
    {
        int cost = GetSpecialUpgradeCost();
        if (PlayerManager.Instance.gold >= cost)
        {
            PlayerManager.Instance.gold -= cost;
            slowLevel++;
            specialLevel = slowLevel;
            currentSlow = baseSlow + slowUpgradeIncrement * slowLevel;
        }
        else
        {
            Debug.Log("Not enough gold for range upgrade!");
        }
        if (currentSlow > 0.75f)
            currentSlow = 0.75f;

        specialUpgradeValue = currentSlow;
        // slowFactor += slowUpgradeIncrement;

    }
}