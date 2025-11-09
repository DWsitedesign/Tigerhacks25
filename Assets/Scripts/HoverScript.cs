using UnityEngine;

public class HoverScript : MonoBehaviour
{
    [Header("Sine Movement Settings")]
    [SerializeField] private float amplitude = 0.5f; // How high it moves
    [SerializeField] private float frequency = 1f;   // How fast it moves

    private Vector3 startPos;
    private float timeOffset;

    private void Awake()
    {
        startPos = transform.position; // Save initial position
        startPos.y = 1.5f;
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate vertical offset using sine
        float yOffset = Mathf.Sin(Time.time * frequency * Mathf.PI * 2f + timeOffset) * amplitude;

        // Apply new position
        transform.position = startPos + new Vector3(0f, yOffset, 0f);
    }
}
