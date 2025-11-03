using UnityEngine;

/// <summary>
/// Базовый класс для башен
/// </summary>
public abstract class TowerBase : MonoBehaviour
{
    [Header("Базовые параметры")]
    public float baseDamage = 5f;         // Базовый урон
    public float baseAttackCooldown = 1f;  // Базовая перезарядка (чем меньше, тем чаще атака)
    public float baseAttackRange = 5f;     // Базовая дистанция
    public string towerName;
    public string SpeicalUpgradeText;

    [Header("Значения улучшений")]
    public float damageIncrement = 3f;         // На сколько растёт урон с апгрейдом
    //public float attackCooldownDecrement = 0.05f; // На сколько уменьшается перезарядка
    public float attackSpeedIncrement = 0.05f; // На сколько уменьшается перезарядка
    public float rangeIncrement = 0.5f;          // На сколько увеличивается дистанция

    [Header("Стоимость улучшений")]
    public int baseDamageUpgradeCost = 50;    // Начальная цена за апгрейд урона
    public int baseSpeedUpgradeCost = 50;     // Начальная цена за апгрейд скорости
    public int baseRangeUpgradeCost = 50;     // Начальная цена за апгрейд дистанции
    public int baseSpecialUpgradeCost = 50;     // Начальная цена за специальный апгрейд
    public float costMultiplier = 1.1f;       // Множитель роста стоимости

    // Текущие уровни улучшений
    private int damageLevel = 0;
    private int speedLevel = 0;
    private int rangeLevel = 0;
    protected int specialLevel = 0;

    public float specialUpgradeValue = 0;


    // Текущие реальные параметры башни

    [Header("Текущие основные параметры башни")]
    //[HideInInspector] 
    public float damage = 5f;
    //public float attackCooldown = 1f; // чем меньше, тем быстрее стреляет
    public float currentAttackCooldown = 1f; // используется для расчета времени между атаками
    public float attackRange = 3f;

    public float attackSpeed = 1f;
    //private float attackSpeedIncrement; // На сколько уменьшается перезарядка

    //private float lastAttackTime;

    public GameObject projectilePrefab; // Префаб снаряда

    //private GameObject towerTop;    Пушка башни для вращения

    //public Transform shootPoint; Точка, из которой вылетает снаряд

    //private Transform target;    позиция цели для вращения                                   
    //private Quaternion newRotation;

    [Header("Range Indicator")]
    [Tooltip("Префаб для отображения радиуса стрельбы (полупрозрачный круг).")]
    public GameObject rangeIndicatorPrefab;

    // Ссылка на созданный индикатор
    private GameObject rangeIndicatorInstance;
    private float rangeIndicatorDistance; // нужно чтобы спрайт был размером 1, иначе всё ломается
    private Vector3 rangeIndicatorPosition;

    protected bool shootOnCooldown = false;

    private void Awake()
    {
        if (rangeIndicatorPrefab != null)
        {
            rangeIndicatorDistance = attackRange;// / 2.55f;
            // Создаём индикатор на позиции башни, поворачивая его так, чтобы он лежал горизонтально (вокруг оси X на 90 градусов)
            rangeIndicatorPosition = transform.position;
            rangeIndicatorPosition.z -= 0.3f;

            rangeIndicatorInstance = Instantiate(rangeIndicatorPrefab, rangeIndicatorPosition, Quaternion.Euler(0f, 0f, 0f), transform);
            // Устанавливаем масштаб: диаметр круга = 2 * attackRange (при условии, что префаб изначально имеет размер 1)
            rangeIndicatorInstance.transform.localScale = new Vector3(rangeIndicatorDistance, rangeIndicatorDistance, 1f);
            // По умолчанию скрываем индикатор
            rangeIndicatorInstance.SetActive(false);
        }
        attackSpeed = 1f / baseAttackCooldown;
        damage = baseDamage;
        currentAttackCooldown = baseAttackCooldown;
        attackRange = baseAttackRange;
    }

    protected virtual void Start()
    {
        // Инициализируем текущие параметры базовыми
        //attackCooldown = baseAttackCooldown;
        //attackSpeedIncrement = -attackCooldownDecrement;
        //towerTop = this.transform.Find("TowerHead").gameObject;
        //newRotation = towerTop.transform.rotation;
    }

    protected void ShootCooldownCalculate()
    {
        if (shootOnCooldown == false)
        {
            Attack();
            //shootOnCooldown = true;
        }
        else
        {
            currentAttackCooldown -= Time.deltaTime;
            if (currentAttackCooldown <= 0)
            {
                shootOnCooldown = false;
                currentAttackCooldown = 1f / attackSpeed;
            }
        }
    }
    protected virtual void Update()
    {
        ShootCooldownCalculate();
        //    currentAttackCooldown -= Time.deltaTime;
        //if (currentAttackCooldown <= 0)
        //{
        //    Attack();
        //    shootOnCooldown = true;
        //   currentAttackCooldown = 1f / attackSpeed;
        //}
        //towerTop.transform.rotation = Quaternion.Slerp(towerTop.transform.rotation, newRotation, Time.deltaTime * 10f);
    }

    protected abstract void Attack(); // Метод атаки, переопределяется в дочерних классах

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

    public virtual int GetSpecialUpgradeCost()
    {
        return Mathf.RoundToInt(baseSpecialUpgradeCost * Mathf.Pow(costMultiplier, specialLevel));
        
    }
    

    public virtual void UpgradeDamage()
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

    public virtual void UpgradeSpeed()
    {
        int cost = GetSpeedUpgradeCost();
        if (PlayerManager.Instance.gold >= cost)
        {
            PlayerManager.Instance.gold -= cost;
            speedLevel++;
            // Уменьшаем cooldown
            attackSpeed = 1f / baseAttackCooldown + attackSpeedIncrement * speedLevel;
            //attackCooldown = 1f / attackSpeed;
            // Чтобы не уходить в отрицательные значения, ограничиваем минимум
            //attackCooldown = Mathf.Max(0.1f, attackCooldown);
        }
        else
        {
            Debug.Log("Not enough gold for speed upgrade!");
        }
    }
    public virtual void UpgradeRange()
    {
        int cost = GetRangeUpgradeCost();
        if (PlayerManager.Instance.gold >= cost)
        {
            PlayerManager.Instance.gold -= cost;
            rangeLevel++;
            attackRange = baseAttackRange + rangeIncrement * rangeLevel;
            rangeIndicatorDistance = attackRange;// / 2.5f;    // нужно сделать спрайт размером 1

            rangeIndicatorInstance.transform.localScale = new Vector3(rangeIndicatorDistance, rangeIndicatorDistance, 1f);
        }
        else
        {
            Debug.Log("Not enough gold for range upgrade!");
        }
    }

    public virtual void UpgradeSpecial()
    {
        //Переопределяется в конкретной башне
    }

    /// <summary>
    /// Отображает индикатор радиуса стрельбы.
    /// Обычно вызывается, когда башня выбрана и TowerUI активен.
    /// </summary>
    public void ShowRangeIndicator()
    {
        if (rangeIndicatorInstance != null)
        {
            rangeIndicatorInstance.SetActive(true);
        }
    }

    /// <summary>
    /// Скрывает индикатор радиуса стрельбы.
    /// </summary>
    public void HideRangeIndicator()
    {
        if (rangeIndicatorInstance != null)
        {
            rangeIndicatorInstance.SetActive(false);
        }
    }
}