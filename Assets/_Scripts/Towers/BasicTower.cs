using UnityEngine;
using UnityEngine.UIElements;

public class BasicTower : TowerBase
{
    private GameObject towerTop;    
    private Quaternion newRotation;

    public AudioSourceEffects effects;


    protected override void Start()
    {
        //effects = AudioSourceEffects.Instance;
        effects = FindFirstObjectByType<AudioSourceEffects>();
        towerTop = this.transform.Find("TowerHead").gameObject;
        newRotation = towerTop.transform.rotation;

        if (GlobalUpgradeManager.Instance.IsUnlocked("BasicTower_damage_1"))
        {
            
            GuidePanelController.Instance.Show($"Улучшение базовой башни сработало. Уровень улучшения 1! "+ damageIncrement.ToString());
        }

        getGlobalUpgrades();

    }

    
    protected override void Update()
    {
        ShootCooldownCalculate();

        towerTop.transform.rotation = Quaternion.Slerp(towerTop.transform.rotation, newRotation, Time.deltaTime * 10f);
    }
    

    protected override void Attack()
    {
        Enemy target = FindTarget();

        if (target != null)
        {
            // Получаем базовый урон
            float finalDamage = damage;

            // Если апгрейд Tower_damage_1 куплен, увеличиваем урон
            shootOnCooldown = true;
            GameObject projectile = Instantiate(projectilePrefab, towerTop.transform.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialize(target.transform, finalDamage);


            AudioManager.Instance.PlaySFX(AudioManager.Instance.basicTowerShootSound); // sound effect playing
            effects.gameObject.transform.position = towerTop.transform.position;
            
        }
    }

    private Enemy FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                newRotation = Quaternion.LookRotation(enemy.gameObject.transform.position - towerTop.transform.position, Vector3.forward);
                return enemy;
            }
        }
        return null;
    }

    private void getGlobalUpgrades()
    {
        var gm = GlobalUpgradeManager.Instance;

        if (gm != null && gm.IsUnlocked("BasicTowerDamage_1"))
        {
            float upgradeValue = gm.GetUpgradeValue("BasicTowerDamage_1");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение базовой башни сработало. Уровень улучшения 1! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона базовой башни сработало. Уровень улучшения 1! " + damageIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("BasicTowerDamage_2"))
        {
            float upgradeValue = gm.GetUpgradeValue("BasicTowerDamage_2");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение базовой башни сработало. Уровень улучшения 2! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона базовой башни сработало. Уровень улучшения 2! " + damageIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("BasicTowerDamage_3"))
        {
            float upgradeValue = gm.GetUpgradeValue("BasicTowerDamage_3");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение базовой башни сработало. Уровень улучшения 3! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона базовой башни сработало. Уровень улучшения 3! " + damageIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("BasicTowerDamage_4"))
        {
            float upgradeValue = gm.GetUpgradeValue("BasicTowerDamage_4");
            damageIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение базовой башни сработало. Уровень улучшения 4! " + damageIncrement.ToString());
            Debug.Log($"Улучшение урона базовой башни сработало. Уровень улучшения 4! " + damageIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("BasicTowerSpeed_1"))
        {
            float upgradeValue = gm.GetUpgradeValue("BasicTowerSpeed_1");
            attackSpeedIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение скорости базовой башни сработало. Уровень улучшения 1! " + attackSpeedIncrement.ToString());
            Debug.Log($"Улучшение скорости базовой башни сработало. Уровень улучшения 1! " + attackSpeedIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("BasicTowerSpeed_2"))
        {
            float upgradeValue = gm.GetUpgradeValue("BasicTowerSpeed_2");
            attackSpeedIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение скорости базовой башни сработало. Уровень улучшения 2! " + attackSpeedIncrement.ToString());
            Debug.Log($"Улучшение скорости базовой башни сработало. Уровень улучшения 2! " + attackSpeedIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("BasicTowerSpeed_3"))
        {
            float upgradeValue = gm.GetUpgradeValue("BasicTowerSpeed_3");
            attackSpeedIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение скорости базовой башни сработало. Уровень улучшения 3! " + attackSpeedIncrement.ToString());
            Debug.Log($"Улучшение скорости базовой башни сработало. Уровень улучшения 3! " + attackSpeedIncrement.ToString());
        }
        if (gm != null && gm.IsUnlocked("BasicTowerSpeed_4"))
        {
            float upgradeValue = gm.GetUpgradeValue("BasicTowerSpeed_4");
            attackSpeedIncrement *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение скорости базовой башни сработало. Уровень улучшения 4! " + attackSpeedIncrement.ToString());
            Debug.Log($"Улучшение скорости базовой башни сработало. Уровень улучшения 4! " + attackSpeedIncrement.ToString());
        }
    }
}