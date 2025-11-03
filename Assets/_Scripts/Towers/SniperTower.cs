//using UnityEditor.PackageManager;
using UnityEngine;

public class SniperTower : TowerBase
{
    private GameObject towerTop;
    [Header("Sniper Tower Settings")]
    // Коэффициент увеличения радиуса атаки для снайперской башни
    //public float enhancedRangeMultiplier = 1.5f;
    // Приращение урона при каждом выстреле по одной и той же цели
    public float baseDamageIncrease = 1.5f;

    private Enemy currentTarget;   // Текущая зафиксированная цель
    private float currentDamage;   // Текущий урон, который увеличивается с каждым выстрелом

    public GameObject laserEffectPrefab;
    private Quaternion newRotation;
    //private Enemy enemy;

    protected override void Start()
    {
        towerTop = this.transform.Find("TowerHead").gameObject;
        newRotation = towerTop.transform.rotation;
        // Применяем увеличенный радиус атаки
        //attackRange *= enhancedRangeMultiplier;
        // Инициализируем начальный урон для цели
        currentDamage = damage;

        getGlobalUpgrades();
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
        // Если цель уже установлена, проверяем, что она всё ещё валидна (жива и в зоне)
        if (currentTarget != null)
        {
            if (currentTarget.IsDead() || Vector3.Distance(towerTop.transform.position, currentTarget.transform.position) > attackRange)
            {
                // Если цель недействительна, сбрасываем цель и урон
                currentTarget = null;
                currentDamage = damage;

            }
        }

        // Если цели нет, ищем новую
        if (currentTarget == null)
        {
            currentTarget = FindTarget();
            currentDamage = damage; // сбрасываем урон для новой цели
        }

        // Если цель найдена, производим атаку
        if (currentTarget != null)
        {
            shootOnCooldown = true;
            // Наносим урон цели
            currentTarget.TakeDamage(currentDamage);
            // Увеличиваем урон для следующего выстрела по этой же цели
            currentDamage += baseDamageIncrease;
            Vector3 targetPostion = currentTarget.transform.position;
            Vector3 towerTopPosition = towerTop.transform.position;
            //targetPostion.y = towerTopPosition.y;
            //targetPostion.z = towerTopPosition.z;
            targetPostion.z -= 0.5f;
            //newRotation = Quaternion.LookRotation(currentTarget.transform.position - towerTop.transform.position, Vector3.forward);

            newRotation = Quaternion.LookRotation(targetPostion - towerTopPosition, Vector3.forward);

            ShowLaserEffect(towerTop.transform.position, currentTarget.transform.position);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.SniperTowerShootSound); // sound effect playing
        }
    }

    /// <summary>
    /// Ищет ближайшего врага в пределах attackRange.
    /// </summary>
    private Enemy FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(towerTop.transform.position, attackRange);
        Enemy closest = null;
        float closestDistance = Mathf.Infinity;
        foreach (Collider hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>(); 
            
            if (enemy != null && !enemy.IsDead())
            {
                float dist = Vector3.Distance(towerTop.transform.position, enemy.transform.position);
                if (dist < closestDistance)
                {
                    //newRotation = Quaternion.LookRotation(enemy.gameObject.transform.position - towerTop.transform.position, Vector3.forward);
                    closest = enemy;
                    closestDistance = dist;
                }
            }
        }
        return closest;
    }

    private void ShowLaserEffect(Vector3 startPos, Vector3 endPos)
    {
        if (laserEffectPrefab != null)
        {
            GameObject laserObj = Instantiate(laserEffectPrefab, startPos, Quaternion.identity);
            LaserEffect effect = laserObj.GetComponent<LaserEffect>();
            if (effect != null)
            {
                effect.Setup(startPos, endPos);
            }
        }
    }

    private void getGlobalUpgrades()
    {
        var gm = GlobalUpgradeManager.Instance;

        if (gm != null && gm.IsUnlocked("SniperTowerDamage_1"))
        {
            float upgradeValue = gm.GetUpgradeValue("SniperTowerDamage_1");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение снайперской башни сработало. Уровень улучшения 1! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона снайперской башни сработало. Уровень улучшения 1! " + damageIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("SniperTowerDamage_2"))
        {
            float upgradeValue = gm.GetUpgradeValue("SniperTowerDamage_2");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение снайперской башни сработало. Уровень улучшения 2! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона снайперской башни сработало. Уровень улучшения 2! " + damageIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("SniperTowerDamage_3"))
        {
            float upgradeValue = gm.GetUpgradeValue("SniperTowerDamage_3");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение снайперской башни сработало. Уровень улучшения 3! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона снайперской башни сработало. Уровень улучшения 3! " + damageIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("SniperTowerDamage_4"))
        {
            float upgradeValue = gm.GetUpgradeValue("SniperTowerDamage_4");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение снайперской башни сработало. Уровень улучшения 4! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона снайперской башни сработало. Уровень улучшения 4! " + damageIncrement.ToString());
        }
    }
}