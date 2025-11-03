using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("Wood UI")]
    public int Wood = 0;
    public TextMeshProUGUI woodText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (woodText != null)
            woodText.text = Wood.ToString();
    }

    public bool CanAfford(int cost) => Wood >= cost;

    public void SpendWood(int amount)
    {
        Wood = Mathf.Max(0, Wood - amount);
    }

    public void AddWood(int amount)
    {
        Wood += amount;
    }
}