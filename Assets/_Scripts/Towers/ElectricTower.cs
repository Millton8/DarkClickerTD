using System.Collections.Generic;
using UnityEngine;

public class ElectricTower : TowerBase
{
    [Header("Electric Tower Settings")]
    [Tooltip("Базовое количество целей в цепи (включая первичную)")]
    public int baseChainCount = 2;
    [Tooltip("Коэффициент уменьшения урона для каждого следующего врага (например, 0.7 означает, что следующий получит 70% от предыдущего)")]
    public float chainDamageReduction = 0.7f;
    [Tooltip("Максимальное расстояние, на котором может происходить прыжок цепи от предыдущей цели")]
    public float chainRange = 5f;
    [Tooltip("Префаб для визуального эффекта цепного электрического удара (с компонентом LineRenderer)")]
    public GameObject electricEffectPrefab;
    private GameObject towerTop; 
    private Quaternion newRotation;

    public AudioSourceEffects effects;

    // Текущее число целей в цепи; его можно увеличивать при апгрейде
    private int currentChainCount;

    protected override void Start()
    {
        effects = FindFirstObjectByType<AudioSourceEffects>();
        //base.Start();
        // Изначально цепь будет охватывать baseChainCount врагов (первичная цель + дополнительные)
        currentChainCount = baseChainCount;
        towerTop = this.transform.Find("TowerHead").gameObject;

        getGlobalUpgrades();
    }
    protected override void Update()
    {
        ShootCooldownCalculate();
        /*
        currentAttackCooldown -= Time.deltaTime;
        if (currentAttackCooldown <= 0)
        {
            Attack();
            currentAttackCooldown = 1f / attackSpeed;
        }*/
        towerTop.transform.rotation = Quaternion.Slerp(towerTop.transform.rotation, newRotation, Time.deltaTime * 5f);
    }
    protected override void Attack()
    {
        // Находим первичную цель — ближайшего врага в пределах зоны атаки башни.
        Enemy primaryTarget = FindPrimaryTarget();
        if (primaryTarget == null)
            return;

        shootOnCooldown = true;
        // Собираем цепь: добавляем первичную цель и последовательно ищем до currentChainCount-1 дополнительных целей.
        List<Enemy> chainTargets = new List<Enemy>();
        chainTargets.Add(primaryTarget);

        Enemy lastTarget = primaryTarget;
        // Цепь будет содержать не более currentChainCount врагов (включая первичную цель).
        for (int i = 1; i < currentChainCount; i++)
        {
            Enemy nextTarget = FindClosestEnemyExcluding(lastTarget.transform.position, chainTargets, chainRange);
            if (nextTarget == null)
                break;
            chainTargets.Add(nextTarget);
            lastTarget = nextTarget;
        }

        // Наносим урон по цепи:
        // Первая цель получает полный урон (damage), каждый следующий — уменьшенный на chainDamageReduction.
        float currentDamage = damage;
        foreach (Enemy enemy in chainTargets)
        {
            enemy.TakeDamage(currentDamage);
            currentDamage = (int)(chainDamageReduction * currentDamage);
        }

        // Отображаем визуальный эффект цепного удара.
        ShowElectricEffect(chainTargets);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.ElectricTowerShootSound); // sound effect playing

        effects.gameObject.transform.position = towerTop.transform.position;
    }

    /// <summary>
    /// Ищет ближайшего врага в пределах attackRange башни.
    /// </summary>
    private Enemy FindPrimaryTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        Enemy closest = null;
        float bestDistance = Mathf.Infinity;
        foreach (Collider col in hits)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead())
            {
                float d = Vector3.Distance(transform.position, enemy.transform.position);
                if (d < bestDistance)
                {
                    bestDistance = d;
                    closest = enemy;
                    newRotation = Quaternion.LookRotation(enemy.transform.position - towerTop.transform.position, Vector3.forward);
                }
            }
        }
        return closest;
    }

    /// <summary>
    /// Ищет ближайшего врага к указанной позиции, который не содержится в списке исключаемых, и находится в пределах указанного range.
    /// </summary>
    private Enemy FindClosestEnemyExcluding(Vector3 referencePosition, List<Enemy> excludedEnemies, float range)
    {
        Collider[] hits = Physics.OverlapSphere(referencePosition, range);
        Enemy best = null;
        float bestDistance = Mathf.Infinity;
        foreach (Collider col in hits)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead() && !excludedEnemies.Contains(enemy))
            {
                float d = Vector3.Distance(referencePosition, enemy.transform.position);
                if (d < bestDistance)
                {
                    bestDistance = d;
                    best = enemy;
                }
            }
        }
        return best;
    }

    /// <summary>
    /// Создаёт визуальный эффект цепного электрического удара с помощью LineRenderer.
    /// Линии проводятся от башни к первичной цели и по цепочке до всех последующих целей.
    /// </summary>
    private void ShowElectricEffect(List<Enemy> chainTargets)
    {
        if (electricEffectPrefab == null || chainTargets.Count == 0)
            return;

        // Создаём эффект (предполагается, что prefab имеет компонент LineRenderer)
        GameObject effectObj = Instantiate(electricEffectPrefab, towerTop.transform.position, Quaternion.identity);
        LineRenderer lr = effectObj.GetComponent<LineRenderer>();
        if (lr != null)
        {
            // Точек будет на одну больше, чем количество целей (начало — позиция башни, затем позиции целей)
            int pointCount = chainTargets.Count + 1;
            lr.positionCount = pointCount;
            lr.SetPosition(0, towerTop.transform.position);
            for (int i = 0; i < chainTargets.Count; i++)
            {
                lr.SetPosition(i + 1, chainTargets[i].transform.position);
            }
        }
        // Уничтожаем эффект спустя короткое время (например, 0.5 секунд)
        Destroy(effectObj, 0.15f);
    }

    /// <summary>
    /// Метод для улучшения: увеличивает число целей в цепи.
    /// Вызывается, например, через кнопку апгрейда в TowerUI.
    /// </summary>
    public void UpgradeChainCount()
    {
        currentChainCount++;
        // При желании можно установить верхний предел цепи.
    }


    // Апгрейд увеличения количество мобов в цепи
    public override void UpgradeSpecial()
    {
        int cost = GetSpecialUpgradeCost();
        if (PlayerManager.Instance.gold >= cost)
        {
            PlayerManager.Instance.gold -= cost;
            currentChainCount++;
            specialLevel++;

            specialUpgradeValue = currentChainCount;
            //specialLevel = explosionRadiusLevel;
            //explosionRadius = baseExplosionRadius + explosionRadiusUpgradeIncrement * explosionRadiusLevel;
        }
        else
        {
            Debug.Log("Not enough gold for electric chain upgrade!");
        }
        //if (explosionRadius > 10f)
        //    explosionRadius = 10f;

        // slowFactor += slowUpgradeIncrement;

    }
    private void getGlobalUpgrades()
    {
        var gm = GlobalUpgradeManager.Instance;

        if (gm != null && gm.IsUnlocked("ElectricTowerDamage_1"))
        {
            float upgradeValue = gm.GetUpgradeValue("ElectricTowerDamage_1");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение электрической башни сработало. Уровень улучшения 1! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона электрической башни сработало. Уровень улучшения 1! " + damageIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("ElectricTowerDamage_2"))
        {
            float upgradeValue = gm.GetUpgradeValue("ElectricTowerDamage_2");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение электрической башни сработало. Уровень улучшения 2! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона электрической башни сработало. Уровень улучшения 2! " + damageIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("ElectricTowerDamage_3"))
        {
            float upgradeValue = gm.GetUpgradeValue("ElectricTowerDamage_3");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение электрической башни сработало. Уровень улучшения 3! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона электрической башни сработало. Уровень улучшения 3! " + damageIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("ElectricTowerDamage_4"))
        {
            float upgradeValue = gm.GetUpgradeValue("ElectricTowerDamage_4");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение электрической башни сработало. Уровень улучшения 4! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона электрической башни сработало. Уровень улучшения 4! " + damageIncrement.ToString());
        }
    }
}