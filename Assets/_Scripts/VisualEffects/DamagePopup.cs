using UnityEngine;
using TMPro;
using System.Data.SqlTypes;

public class DamagePopup : MonoBehaviour
{
    public float lifetime = 1f;
    public float floatSpeed = 1f;

    private TextMesh textMesh;

    public void Initialize(float damage)
    {
        textMesh = GetComponent<TextMesh>();
        textMesh.text = Mathf.RoundToInt(damage).ToString();
        
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        lifetime -= Time.deltaTime;

        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
