using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 2f; // upward movement speed
    [SerializeField] private float duration = 1f;   // how long it stays
    private TextMeshProUGUI textMesh;
    private float timer = 0f;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(int damage)
    {
        textMesh.text = damage.ToString();
        timer = 0f;
    }

    private void Update()
    {
        // Float upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Destroy after duration
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
