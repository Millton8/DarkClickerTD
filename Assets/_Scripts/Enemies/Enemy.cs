using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.AI;
//using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform waypoint;
    private NavMeshAgent agent;
    [SerializeField]
    public float baseHealth = 40f;
    public float baseSpeed = 2f;

    private float currentHealth;
    private float currentSpeed;

    public int goldReward = 10;
    public float enemyDamage = 3f;
    public int enemyScore = 1;

    public GameObject damagePopupPrefab; // Префаб всплывающего текста урона
    public Castle castle;

    private Transform target;
    //private WaveManager waveManager;     // Ссылка на WaveManager
    public WaveManager waveManager;

    // Для управления движением через Rigidbody
    private Rigidbody rb;

    //Для замедления врагов
    private Renderer enemyRenderer;
    private Color originalColor;

    // Enemy HP Bar Visualisation
    [Header("Health Settings")]
    public float maxHealth;// = 100f;      // Максимальное здоровье врага
    //public float currentHealth;         // Текущее здоровье (изменяется при получении урона)

    [Header("HP Bar UI")]
    [Tooltip("Ссылка на Image, используемый для отображения полоски ХП. " +
             "Убедитесь, что Image настроен как Filled (Fill Method: Horizontal).")]
    public Image hpBar;

    [Header("Animation")]
    public Animator animator;           // Ссылка на компонент Animator

    // Массив материалов, заполните его в инспекторе
    [SerializeField] private Material[] materials;

    [SerializeField] private Material[] materialOptions;

    [Range(0, 1)]
    public float freezeAmount = 0.2f;  // 0 — без изменений, 1 — полностью синий

    private MaterialPropertyBlock _mpb;
    private Renderer _renderer;
    private Color _baseColor;

    // Имена триггеров/состояний, настроенных в AnimatorController:
    private const string STATE_WALK = "WalkFWD";
    private const string STATE_IDLE = "IdleNormal";
    private const string TRIGGER_ATTACK = "Attack01";
    private const string TRIGGER_GET_HIT = "GetHit";
    private const string TRIGGER_DIE = "Die";

    private float enemyAttacked = 2f;
    private bool enemyIsAttacked = false;

    private int matIndex;

    void Awake()
    {
        _renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _mpb = new MaterialPropertyBlock();

        _renderer.GetPropertyBlock(_mpb);
        _baseColor = _mpb.GetColor("_BaseColor");

        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        // Отключаем гравитацию, если враг не должен падать 
        rb.useGravity = false;
        // Можно заморозить вращения, чтобы не заваливался
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        castle = FindFirstObjectByType<Castle>();
        waveManager = FindFirstObjectByType<WaveManager>();

        waypoint = GameObject.FindWithTag("Waypoint").transform;
        target = GameObject.FindWithTag("Castle").transform;



        //GameObject HpBar = transform.Find("HpBar").gameObject;
        //hpBar = HpBar.GetComponent("image");
        //hpBar = this.gameObject.GetComponent("HpBar");
        //hpBar = hpBar.image;

        if (hpBar == null)
        {
            hpBar = GetComponentInChildren<Image>();
        }
            
        currentSpeed = baseSpeed;
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            //if (enemyRenderer.material.HasProperty("_Color"))
                originalColor = enemyRenderer.material.color;
        }

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    
            // По умолчанию запускаем Idle (или WalkForward, если враг сразу начинает двигаться)
        if(animator != null)
        {
            //animator.Play(STATE_IDLE);
            animator.SetBool(STATE_IDLE, true);
            animator.SetBool(STATE_WALK, true);
            animator.SetBool(TRIGGER_ATTACK, false);
            animator.SetBool(TRIGGER_GET_HIT, false);
            animator.SetBool(TRIGGER_DIE, false);
        }

        // Присваиваем случайный материал
        Renderer rend = this.GetComponentInChildren<SkinnedMeshRenderer>();
        if (rend != null)
        {
            // Выбираем случайный материал
            matIndex = Random.Range(0, materials.Length);
            Material mat = materials[matIndex];

            // Присваиваем материал. 
            // rend.material создаёт экземпляр материала для этого рендера,
            // а rend.sharedMaterial — заменит материал у всех объектов, 
            // которые на него ссылаются в проекте.
            rend.material = mat;
        }
    }

    void Start()
    {
        agent.SetDestination(waypoint.position);
        // castle = FindObjectOfType<Castle>();
        // waveManager   = FindObjectOfType<WaveManager>();
        //waypoint = FindFirstObjectByType<Waypoint>();
        maxHealth = currentHealth;
        UpdateHPBar();
    }

    void Update()
    {
        /*MoveToCastle();
        
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            castle.TakeDamage(enemyDamage);
            waveManager.OnEnemyKilled();
            Destroy(gameObject);
        }*/
        agent.speed = currentSpeed;
        /*if (animator != null)
        {
            animator.SetBool(STATE_IDLE, false);
            animator.SetBool(STATE_WALK, true);
            animator.SetBool(TRIGGER_ATTACK, false);
            animator.SetBool(TRIGGER_GET_HIT, false);
            animator.SetBool(TRIGGER_DIE, false);
        }*/
    }



    void FixedUpdate()
    {
        if (target == null) return;

        // Двигаемся к цели через Rigidbody.MovePosition 
        // (учитывая возможные коллизии)

        //Vector3 direction = (target.position - transform.position).normalized;
        //Vector3 newPos = transform.position + direction * currentSpeed * Time.fixedDeltaTime;
        //rb.MovePosition(newPos);
        if (enemyIsAttacked == true)
        {
            enemyAttacked -= Time.deltaTime;
        }
        if (enemyAttacked <= 0)
        {
            enemyIsAttacked = false;
        }
        if (enemyIsAttacked == false)
        {
            if (animator != null)
            {
                animator.SetBool(STATE_IDLE, true);
                animator.SetBool(STATE_WALK, true);
                animator.SetBool(TRIGGER_GET_HIT, false);
            }
        }

        if (Vector3.Distance(transform.position, waypoint.position) < 0.35f)
        {
            if (animator != null)
            {
                animator.SetBool(STATE_IDLE, false);
                animator.SetBool(STATE_WALK, false);
                animator.SetBool(TRIGGER_ATTACK, true);
                animator.SetBool(TRIGGER_GET_HIT, false);
                animator.SetBool(TRIGGER_DIE, false);
            }
            if(waveManager.IsBossLevel())
                castle.TakeDamage(enemyDamage * 5);
            else
                castle.TakeDamage(enemyDamage);
            waveManager.OnEnemyKilled();
            //Destroy(gameObject.GetComponentInParent<Enemy>().gameObject);
            Destroy(gameObject);
        }

    }


    void MoveToCastle()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, currentSpeed * Time.deltaTime);
        }
    }

    public void InitializeEnemy(float healthMultiplier, float speedMultiplier)
    {
        //currentHealth = baseHealth * healthMultiplier;
        currentHealth = healthMultiplier;
        currentSpeed = baseSpeed * speedMultiplier;
    }

    /// <summary>
    /// Ставим ссылку на WaveManager
    /// </summary>
    public void SetWaveManager(WaveManager manager)
    {
        waveManager = manager;
    }

    public void TakeDamage(float damage)
    {

        currentHealth -= damage;
        ShowDamagePopup(damage);


        AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyClickSound);  // sound effect playing

        UpdateHPBar();

        if (animator != null)
        {
            animator.SetBool(STATE_IDLE, false);
            animator.SetBool(STATE_WALK, false);
            animator.SetBool(TRIGGER_ATTACK, false);
            animator.SetBool(TRIGGER_GET_HIT, true);
            animator.SetBool(TRIGGER_DIE, false);
        }

        enemyIsAttacked = true;
        enemyAttacked = 2f;

        if (currentHealth <= 0)
        {
            if (animator != null)
            {
                animator.SetBool(STATE_IDLE, false);
                animator.SetBool(STATE_WALK, false);
                animator.SetBool(TRIGGER_ATTACK, false);
                animator.SetBool(TRIGGER_GET_HIT, false);
                animator.SetBool(TRIGGER_DIE, true);
            }
            Die();
        }
    }

    void ShowDamagePopup(float damage)
    {
        if (damagePopupPrefab != null)
        {
            GameObject popup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
            // Передаём число в DamagePopup
            DamagePopup dmgPopup = popup.GetComponent<DamagePopup>();
            if (dmgPopup != null)
                dmgPopup.Initialize(damage);
        }
    }

    void Die()
    {
        // Начисляем золото игроку
        PlayerManager.Instance.AddGold(waveManager.currentGoldReward);
        PlayerManager.Instance.AddGold(waveManager.currentLumberReward);
        ScoreManager.Instance.AddScore(waveManager.GetCurrentWave() * enemyScore);

        // if boss killed
        ResourceManager.Instance.AddWood(Mathf.RoundToInt(waveManager.currentLumberReward));

        // Сообщаем WaveManager, что этот враг уничтожен
        if (waveManager != null)
        {
            waveManager.OnEnemyKilled();
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyDeathSound);  // sound effect playing

        Destroy(gameObject);
        

    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Castle"))
        {
            Castle castle = other.GetComponent<Castle>();
            if (castle != null)
            {
                castle.TakeDamage(enemyDamage); // Например, 10 урона
            }
            // Уничтожаем врага, раз он добрался
            ScoreManager.Instance.AddScore(waveManager.GetCurrentWave() * enemyScore);
            Destroy(gameObject);
        }
    }*/


    /// <summary>
    /// Обработка столкновений с препятствиями и другими врагами
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        //Столкновение с замком - нанесение урона, уничтожение врага
        if (collision.gameObject.CompareTag("Castle"))
        {
            //Castle castle = collision.gameObject.GetComponent<Castle>();
            if (castle != null)
            {
                castle.TakeDamage(enemyDamage); // Например, 10 урона
            }
            // Уничтожаем врага, раз он добрался
            //ScoreManager.Instance.AddScore(waveManager.GetCurrentWave() * enemyScore);
            if (waveManager != null)
            {
                waveManager.OnEnemyKilled();
            }
            Destroy(gameObject);
            //waveManager.aliveEnemies--;
        }

        // Если столкнулись с препятствием, 
        // можно дополнительно изменить траекторию, нанести урон и т.п.
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Пример: немного уменьшаем скорость
            currentSpeed = Mathf.Max(currentSpeed - 0.1f, 0.1f);
            Debug.Log("Enemy hit obstacle! Speed reduced.");

        }

        // Если сталкиваемся с другим врагом, 
        // можно, к примеру, оттолкнуться друг от друга
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Пример: отталкиваемся друг от друга, если скорости разные.
            Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
            if (otherRb != null)
            {
                Vector3 pushDir = (transform.position - collision.transform.position).normalized;
                rb.AddForce(pushDir * 25f);     // Толкаем себя
                otherRb.AddForce(-pushDir * 25f); // Толкаем противника
            }
        }
    }
    
    public bool IsDead()
    {
        if (currentHealth > 0)
            return false;
        else 
            return true;
    }


    /// <summary>
    /// Применяет эффект замедления: уменьшает скорость на заданный процент на duration секунд,
    /// а также изменяет цвет материала на темно-синий для визуализации эффекта.
    /// </summary>
    public void ApplySlowEffect(float slowPercent, float duration)
    {
        //Color target = Color.Lerp(_baseColor, Color.blue, freezeAmount);
        //_mpb.SetColor("_BaseColor", target);
        //_renderer.SetPropertyBlock(_mpb);
        StartCoroutine(SlowCoroutine(slowPercent, duration));
    }


    private IEnumerator SlowCoroutine(float slowPercent, float duration)
    {
        float originalSpeed = currentSpeed;
        currentSpeed = currentSpeed * (1 - slowPercent);

        Material currentMaterial = _renderer.material;
        /*if (matIndex == 0)
        {
            Color prevColor = _baseColor;
            Color target = Color.Lerp(_baseColor, Color.blue, slowPercent);
            _mpb.SetColor("_BaseColor", target);
            _renderer.SetPropertyBlock(_mpb);

            yield return new WaitForSeconds(duration);

            _mpb.SetColor("_BaseColor", prevColor);
            _renderer.SetPropertyBlock(_mpb);
        }
        else
        {*/
            _renderer.material = materialOptions[matIndex];

            yield return new WaitForSeconds(duration);
            _renderer.material = currentMaterial;
        //}
        /*
        else if (matIndex == 0)
        {
            _renderer.material = materialOptions[0];
        }
        else if (currentMaterial.name == "PAMaskTint02")
        {
            _renderer.material = materialOptions[1];
        }
        else if (currentMaterial.name == "PAMaskTint03")
        {
            _renderer.material = materialOptions[2];
        }*/




        /*if (rend != null)
        {
            // Сохраняем исходный цвет и устанавливаем темно-синий
            if (rend.material.HasProperty("_Color"))
            {
                Color prevColor = rend.material.color;
                rend.material.color = new Color(prevColor.r, prevColor.g, prevColor.b*3f, prevColor.a);
            }
        }*/

        //if (rend != null)
        //{
        //    rend.material.color = originalColor;
        //}
        currentSpeed = originalSpeed;
    }

    /// <summary>
    /// Обновляет визуальное отображение полоски ХП.
    /// Если враг полностью здоров, полоска скрывается.
    /// В противном случае отображается процент оставшегося здоровья.
    /// </summary>
    void UpdateHPBar()
    {
        if (hpBar != null)
        {
            float fillAmount = currentHealth / maxHealth;
            hpBar.fillAmount = fillAmount;
             

            // Если здоровье полное, скрываем индикатор, иначе показываем
            if (fillAmount >= 1f)
            {
                hpBar.gameObject.SetActive(false);
            }
            else
            {
                hpBar.gameObject.SetActive(true);
            }
        }
    }
}