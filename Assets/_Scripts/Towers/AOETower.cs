//using UnityEditor.PackageManager;
using UnityEngine;

public class AOETower : TowerBase
{
    [Header("AOE Tower Settings")]
    public float baseExplosionRadius = 3f;                      // Базовый радиус взрыва
    public float explosionRadiusUpgradeIncrement = 0.5f;    // Приращение радиуса взрыва при апгрейде
    public GameObject aoeProjectilePrefab;                  // Префаб снаряда для взрывного выстрела

    private GameObject towerTop;
    private Quaternion newRotation;
    private float explosionRadius = 3f;                      // Текущий радиус взрыва
    private int explosionRadiusLevel = 1;                      // Текущий радиус взрыва


    protected override void Start()
    {
        explosionRadius = baseExplosionRadius;
        towerTop = this.transform.Find("TowerHead").gameObject;
        newRotation = towerTop.transform.rotation;

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
        towerTop.transform.rotation = Quaternion.Slerp(towerTop.transform.rotation, newRotation, Time.deltaTime * 10f);
    }
    protected override void Attack()
    {
        // Ищем цель в пределах зоны атаки (используем простой алгоритм поиска ближайшего врага)
        Enemy target = FindTarget();
        if (target != null)
        {
            shootOnCooldown = true;
            // Рассчитываем направление для снаряда по направлению к цели
            Vector3 direction = (target.transform.position - towerTop.transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);

            // Создаём снаряд на позиции башни
            GameObject projectileObj = Instantiate(aoeProjectilePrefab, towerTop.transform.position, rotation);
            ProjectileAOE aoeProj = projectileObj.GetComponent<ProjectileAOE>();
            if (aoeProj != null)
            {
                // Передаём параметры снаряду: цель (или её позицию), урон и радиус взрыва
                aoeProj.Initialize(target.transform, damage, explosionRadius);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.AOETowerShootSound); // sound effect playing
            }
        }
    }

    /// <summary>
    /// Находит ближайшего врага в пределах attackRange.
    /// </summary>
    private Enemy FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        Enemy closest = null;
        float closestDistance = Mathf.Infinity;
        foreach (Collider col in hits)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead())
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < closestDistance)
                {
                    closest = enemy;
                    closestDistance = dist;
                    newRotation = Quaternion.LookRotation(enemy.transform.position - towerTop.transform.position, Vector3.forward);
                }
            }
        }
        return closest;
    }

    /// <summary>
    /// Улучшает радиус взрыва башни, увеличивая его на фиксированное значение,
    /// но не позволяет его бесконечно нарастать.
    /// </summary>
    public override void UpgradeSpecial()
    {
        int cost = GetSpecialUpgradeCost();
        if (PlayerManager.Instance.gold >= cost)
        {
            PlayerManager.Instance.gold -= cost;
            explosionRadiusLevel++;
            specialLevel = explosionRadiusLevel;
            explosionRadius = baseExplosionRadius + explosionRadiusUpgradeIncrement * explosionRadiusLevel;
        }
        else
        {
            Debug.Log("Not enough gold for explosion upgrade!");
        }
        if (explosionRadius > 10f)
            explosionRadius = 10f;

        specialUpgradeValue = explosionRadius;
        // slowFactor += slowUpgradeIncrement;

    }

    private void getGlobalUpgrades()
    {
        var gm = GlobalUpgradeManager.Instance;

        if (gm != null && gm.IsUnlocked("AOETowerDamage_1"))
        {
            float upgradeValue = gm.GetUpgradeValue("AOETowerDamage_1");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение аое башни сработало. Уровень улучшения 1! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона аое башни сработало. Уровень улучшения 1! " + damageIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("AOETowerDamage_2"))
        {
            float upgradeValue = gm.GetUpgradeValue("AOETowerDamage_2");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение аое башни сработало. Уровень улучшения 2! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона аое башни сработало. Уровень улучшения 2! " + damageIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("AOETowerDamage_3"))
        {
            float upgradeValue = gm.GetUpgradeValue("AOETowerDamage_3");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение аое башни сработало. Уровень улучшения 3! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона аое башни сработало. Уровень улучшения 3! " + damageIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("AOETowerDamage_4"))
        {
            float upgradeValue = gm.GetUpgradeValue("AOETowerDamage_4");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение аое башни сработало. Уровень улучшения 4! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона аое башни сработало. Уровень улучшения 4! " + damageIncrement.ToString());
        }
    }
}