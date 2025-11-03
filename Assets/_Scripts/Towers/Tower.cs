using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseDamage = 20f;         // Базовый урон
    public float baseAttackCooldown = 1f;  // Базовая перезарядка (чем меньше, тем чаще атака)
    public float baseAttackRange = 5f;     // Базовая дистанция

    [Header("Upgrade Increments")]
    public float damageIncrement = 3f;         // На сколько растёт урон с апгрейдом
    public float attackCooldownDecrement = 0.05f; // На сколько уменьшается перезарядка
    public float rangeIncrement = 0.5f;          // На сколько увеличивается дистанция

    [Header("Upgrade Costs")]
    public int baseDamageUpgradeCost = 50;    // Начальная цена за апгрейд урона
    public int baseSpeedUpgradeCost = 50;     // Начальная цена за апгрейд скорости
    public int baseRangeUpgradeCost = 50;     // Начальная цена за апгрейд дистанции
    public float costMultiplier = 1.1f;       // Множитель роста стоимости

    // Текущие уровни улучшений
    private int damageLevel = 0;
    private int speedLevel = 0;
    private int rangeLevel = 0;


    // Текущие реальные параметры башни
    [HideInInspector] public float damage;
    [HideInInspector] public float attackCooldown; // чем меньше, тем быстрее стреляет
    [HideInInspector] public float attackRange;

    private float lastAttackTime;

    public GameObject projectilePrefab; // Префаб снаряда
    private GameObject towerTop;    // Пушка башни для вращения
    public Transform shootPoint; // Точка, из которой вылетает снаряд
    private Transform target;   // позиция цели для вращения
    private Quaternion newRotation;

    private void Start()
    {
        // Инициализируем текущие параметры базовыми
        damage = baseDamage;
        attackCooldown = baseAttackCooldown;
        attackRange = baseAttackRange;
        towerTop = this.transform.Find("TowerHead").gameObject;
        newRotation = towerTop.transform.rotation;
    }

    void Update()
    {
        AttackClosestEnemy();

        towerTop.transform.rotation = Quaternion.Slerp(towerTop.transform.rotation, newRotation, Time.deltaTime * 10f);
    }

    void AttackClosestEnemy()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        Enemy closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            target = closestEnemy.transform;
            
            //Vector3 point = target.position;
            //point.y = this.transform.position.y;
            //point.z = this.transform.position.z;
            //towerTop.transform.LookAt(point);
            //towerTop.transform.LookAt(target);
            //towerTop.transform.rotation=Vector3()

            newRotation = Quaternion.LookRotation(target.position - towerTop.transform.position, Vector3.forward);

            ////newRotation.z = 90f;
            ////transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 8);
            ////towerTop.transform.rotation = Quaternion.Slerp(towerTop.transform.rotation, newRotation, Time.deltaTime * 8);

            Shoot(closestEnemy);
            lastAttackTime = Time.time;
        }
    }

    Enemy FindClosestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None); ;
        Enemy closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && distance <= attackRange)
            {
                closest = enemy;
                closestDistance = distance;
            }
        }

        return closest;
    }

    void Shoot(Enemy target)
    {
        if (projectilePrefab != null && shootPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialize(target.transform, damage);
        }
    }

    // -----------------------------
    // Методы расчёта стоимости апгрейдов
    // -----------------------------
    public int GetDamageUpgradeCost()
    {
        return Mathf.RoundToInt(baseDamageUpgradeCost * Mathf.Pow(costMultiplier, damageLevel));
    }

    public int GetSpeedUpgradeCost()
    {
        return Mathf.RoundToInt(baseSpeedUpgradeCost * Mathf.Pow(costMultiplier, speedLevel));
    }

    public int GetRangeUpgradeCost()
    {
        return Mathf.RoundToInt(baseRangeUpgradeCost * Mathf.Pow(costMultiplier, rangeLevel));
    }

    // -----------------------------
    // Методы апгрейда
    // -----------------------------
    public void UpgradeDamage()
    {
        int cost = GetDamageUpgradeCost();
        if (PlayerManager.Instance.gold >= cost)
        {
            PlayerManager.Instance.gold -= cost;
            damageLevel++;
            // Увеличиваем урон (добавляем damageIncrement)
            damage = baseDamage + damageIncrement * damageLevel;
        }
        else
        {
            Debug.Log("Not enough gold for damage upgrade!");
        }
    }

    public void UpgradeSpeed()
    {
        int cost = GetSpeedUpgradeCost();
        if (PlayerManager.Instance.gold >= cost)
        {
            PlayerManager.Instance.gold -= cost;
            speedLevel++;
            // Уменьшаем cooldown
            attackCooldown = baseAttackCooldown - attackCooldownDecrement * speedLevel;
            // Чтобы не уходить в отрицательные значения, ограничиваем минимум
            attackCooldown = Mathf.Max(0.1f, attackCooldown);
        }
        else
        {
            Debug.Log("Not enough gold for speed upgrade!");
        }
    }
        public void UpgradeRange()
    {
        int cost = GetRangeUpgradeCost();
        if (PlayerManager.Instance.gold >= cost)
        {
            PlayerManager.Instance.gold -= cost;
            rangeLevel++;
            attackRange = baseAttackRange + rangeIncrement * rangeLevel;
        }
        else
        {
            Debug.Log("Not enough gold for range upgrade!");
        }
    }
}
