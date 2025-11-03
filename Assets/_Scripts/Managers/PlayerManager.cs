using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [Header("Base Click Stats")]
    public float baseClickDamage = 5f;        // Базовый урон от клика
    public float critChance = 0f;            // Шанс крит. удара (в процентах)
    public float bonusGoldChance = 0f;       // Шанс получить доп. золото (в процентах)
    public float gold = 0;                     // Текущее золото игрока
    public float lumber = 0;                     // Текущее дерево игрока

    // Пример: множитель урона при крите
    public float critMultiplier = 2f;
    public float сlickDamage = 5f;        // Текущий урон от клика

    //public float clickDamage = 10f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }

    private void Start()
    {
        var gm = GlobalUpgradeManager.Instance;

        if (gm != null && gm.IsUnlocked("PlayerClickDamage_1"))
        {
            float upgradeValue = gm.GetUpgradeValue("PlayerClickDamage_1");
            baseClickDamage *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение клика сработало. Уровень улучшения 1! Базовый урон: " + baseClickDamage.ToString());
            Debug.Log($"Улучшение клика сработало. Уровень улучшения 1! Базовый урон: " + baseClickDamage.ToString());
        }

        if (gm != null && gm.IsUnlocked("PlayerClickDamage_2"))
        {
            float upgradeValue = gm.GetUpgradeValue("PlayerClickDamage_2");
            baseClickDamage *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение клика сработало. Уровень улучшения 2! Базовый урон: " + baseClickDamage.ToString());
            Debug.Log($"Улучшение клика сработало. Уровень улучшения 2! Базовый урон: " + baseClickDamage.ToString());
        }

        if (gm != null && gm.IsUnlocked("PlayerClickDamage_3"))
        {
            float upgradeValue = gm.GetUpgradeValue("PlayerClickDamage_3");
            baseClickDamage *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение клика сработало. Уровень улучшения 3! Базовый урон: " + baseClickDamage.ToString());
            Debug.Log($"Улучшение клика сработало. Уровень улучшения 3! Базовый урон: " + baseClickDamage.ToString());
        }

        if (gm != null && gm.IsUnlocked("PlayerClickDamage_4"))
        {
            float upgradeValue = gm.GetUpgradeValue("PlayerClickDamage_4");
            baseClickDamage *= upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение клика сработало. Уровень улучшения 4! Базовый урон: " + baseClickDamage.ToString());
            Debug.Log($"Улучшение клика сработало. Уровень улучшения 4! Базовый урон: " + baseClickDamage.ToString());
        }


        if (gm != null && gm.IsUnlocked("GoldBaseAmount_1"))
        {
            float upgradeValue = gm.GetUpgradeValue("GoldBaseAmount_1");
            gold = upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение золота сработало. Уровень улучшения 1! Количество золота: " + upgradeValue.ToString());
            Debug.Log($"Улучшение золота сработало. Уровень улучшения 1! Количество золота: " + upgradeValue.ToString());
        }

        if (gm != null && gm.IsUnlocked("GoldBaseAmount_2"))
        {
            float upgradeValue = gm.GetUpgradeValue("GoldBaseAmount_2");
            gold = upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение золота сработало. Уровень улучшения 2! Количество золота: " + upgradeValue.ToString());
            Debug.Log($"Улучшение золота сработало. Уровень улучшения 2! Количество золота: " + upgradeValue.ToString());
        }

        if (gm != null && gm.IsUnlocked("GoldBaseAmount_3"))
        {
            float upgradeValue = gm.GetUpgradeValue("GoldBaseAmount_3");
            gold = upgradeValue;

            GuidePanelController.Instance.Show($"Улучшение золота сработало. Уровень улучшения 3! Количество золота: " + upgradeValue.ToString());
            Debug.Log($"Улучшение золота сработало. Уровень улучшения 3! Количество золота: " + upgradeValue.ToString());
        }

        if (gm != null && gm.IsUnlocked("GoldBaseAmount_4"))
        {
            float upgradeValue = gm.GetUpgradeValue("GoldBaseAmount_4");

            GuidePanelController.Instance.Show($"Улучшение золота сработало. Уровень улучшения 4! Количество золота: " + upgradeValue.ToString());
            Debug.Log($"Улучшение золота сработало. Уровень улучшения 4! Количество золота: " + upgradeValue.ToString());
            gold = upgradeValue;
        }

        сlickDamage = baseClickDamage;
    }
    void Update()
    {
        HandleMouseClick();
    }

    /// <summary>
    /// Метод, вызывающийся при клике на врага
    /// </summary>
    public void ClickEnemy(Enemy enemy)
    {
        float damageToDeal = сlickDamage;

        // Рассчитываем критический удар
        // critChance хранится, например, в процентах (0..100)
        float roll = Random.Range(0f, 100f);
        if (roll < critChance)
        {
            damageToDeal *= critMultiplier;
            Debug.Log("CRITICAL HIT!");
        }
        enemy.TakeDamage(damageToDeal);

        // Если враг умер (health <= 0) внутри TakeDamage,
        // в Enemy есть логика Die(), которая добавляет золото.
        // Дополнительно проверяем шанс получить бонусное золото:
        if (enemy.IsDead()) // нужно расширить Enemy для проверки
        {
            float goldRoll = Random.Range(0f, 100f);
            if (goldRoll < bonusGoldChance)
            {
                // Например, +50 золота за бонус
                int bonus = 50;
                AddGold(bonus);
                Debug.Log($"Bonus gold: +{bonus}!");
            }
        }
    }

    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // Левый клик мыши
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    ClickEnemy(enemy);
                }
            }
        }
    }

    public void AddGold(float amount)
    {
        gold += amount;
    }

    public void AddLumber(float amount)
    {
        lumber += amount;
    }
}