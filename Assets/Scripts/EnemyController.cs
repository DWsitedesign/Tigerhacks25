
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    private Transform[] waypoints;
    private int currentIndex = 0;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float closeDetection = 3f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float critChance = 0.1f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackRange = 10f;
    private float lastAttackTime;

    [SerializeField] private int health = 50;
    [SerializeField] private int maxHealth = 50;

    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private Transform pathParent;
    private Rigidbody rb;
    private Vector3 lastPosition;
    private int directionInt = 1; // 1 for forward, -1 for backward
    private Transform playerTarget;
    private float timeSinceLastSeen = Mathf.Infinity;
    public float loseSightDelay = 2f;
    private Vector3 lastKnownPlayerPos;

    private void Awake()
    {
        lastAttackTime = Time.time;
        // Get all children of pathParent as waypoints
        int count = pathParent.childCount;
        waypoints = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            waypoints[i] = pathParent.GetChild(i);
        }
        Debug.Log("Waypoints count: " + waypoints.Length);
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        worldCanvas = GameObject.FindWithTag("WorldCanvas").GetComponent<Canvas>();
    }

    private void Update()
    {
        checkForPlayer();
        if (playerTarget != null || timeSinceLastSeen < loseSightDelay)
        {
            ChasePlayer();
        }
        else if (waypoints.Length != 0)
        {
            Patrol();
        }

        Vector2 moveDelta = new Vector2(transform.position.x - lastPosition.x, transform.position.z - lastPosition.z);
        if (moveDelta.sqrMagnitude > 0.0001f)
        {
            lastPosition = transform.position;
        }

        timeSinceLastSeen += Time.deltaTime;

    }

    private void checkForPlayer()
    {
        Vector2 dir2D = new Vector2(
    transform.position.x - lastPosition.x,
    transform.position.z - lastPosition.z
).normalized;


        // TODO: add the ability to change these for detection patterns
        int numRays = 40;
        float minAngle = -45f;
        float maxAngle = 45f;

        for (int i = 0; i < numRays; i++)
        {
            // Interpolate angle across the range
            float t = i / (float)(numRays - 1);
            float angle = Mathf.Lerp(minAngle, maxAngle, t);

            // Rotate the direction vector around Y axis
            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * new Vector3(dir2D.x, 0f, dir2D.y);

            // Debug: draw ray
            Debug.DrawRay(transform.position, direction * detectionRange, Color.red);

            // Raycast to detect player
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, detectionRange, playerLayer))
            {
                // Debug.Log("Player detected at angle " + angle + ": " + hit.collider.name);
                // TODO: attack logic
                playerTarget = hit.transform;
                lastKnownPlayerPos = playerTarget.position;
                timeSinceLastSeen = 0f;
                return; // stop checking once we see the player
            }
        }
        Collider[] hits = Physics.OverlapSphere(transform.position, closeDetection, playerLayer);

        if (hits.Length > 0)
        {
            // Player is within close range
            playerTarget = hits[0].transform;
            lastKnownPlayerPos = playerTarget.position;
            timeSinceLastSeen = 0f;


            // Debug.Log("Player within close detection radius: " + playerTarget.name);
            return;
        }

        // If no rays hit, lose the player
        playerTarget = null;
    }

    private void ChasePlayer()
    {
        Vector3 targetPos = (playerTarget != null) ? playerTarget.position : lastKnownPlayerPos;

        // Vector2 dir2D = new Vector2(targetPos.x - transform.position.x, targetPos.z - transform.position.z).normalized;
        // Vector3 move = new Vector3(dir2D.x, 0f, dir2D.y) * moveSpeed;
        // rb.MovePosition(transform.position + move * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance <= attackRange * 3 / 4)
        {
            // Stop movement
            rb.linearVelocity = Vector3.zero;

            // Attack if enough time has passed
            if (Time.time - lastAttackTime > attackCooldown)
            {
                lastAttackTime = Time.time;
                AttackPlayer();
            }

            return; // Don’t move closer while attacking
        }

        // Otherwise, keep moving toward the player
        Vector2 dir2D = new Vector2(targetPos.x - transform.position.x, targetPos.z - transform.position.z).normalized;
        Vector3 move = new Vector3(dir2D.x, 0f, dir2D.y) * moveSpeed;
        rb.MovePosition(transform.position + move * Time.deltaTime);
    }

    private void Patrol()
    {
        if (currentIndex < 0 || currentIndex >= waypoints.Length)
        {
            Debug.LogWarning("Current index out of bounds: " + currentIndex);
            return;
        }
        Transform target = waypoints[currentIndex];
        Vector2 dir2D = new Vector2(
    target.position.x - transform.position.x,
    target.position.z - transform.position.z
).normalized;

        // Convert to 3D movement vector (assuming Z is vertical or unused)
        Vector3 move = new Vector3(dir2D.x, 0f, dir2D.y) * moveSpeed;
        lastPosition = transform.position;

        // Apply using linearVelocity
        rb.MovePosition(transform.position + move * Time.deltaTime);

        // Debug.Log("Enemy moving towards waypoint " + currentIndex + " at position " + target.position + "distance: " + Vector3.Distance(transform.position, target.position));
        // Switch to next waypoint if close enough
        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            currentIndex += directionInt;
            // tempCoroutine(.2f);
            if (currentIndex >= waypoints.Length)
            {
                currentIndex = waypoints.Length - 2; // step back into bounds
                directionInt = -1;
            }
            else if (currentIndex < 0)
            {
                currentIndex = 1; // step forward into bounds
                directionInt = 1;
            }
        }
    }
    private IEnumerator tempCoroutine(float WaitForSeconds)
    {
        yield return new WaitForSeconds(WaitForSeconds);
    }

    public void takeDamage(int damage)
    {
        health -= damage;
        if (damageTextPrefab != null)
        {
            GameObject dmgText = Instantiate(damageTextPrefab, transform.position + Vector3.up, Quaternion.identity, worldCanvas.transform);
            dmgText.GetComponent<FloatingDamageText>().Initialize(damage);
        }
        if (health <= 0)
        {
            health = 0;
            Debug.Log("Enemy is dead.");
            Destroy(gameObject);
        }
        Debug.Log("Enemy took damage. Current health: " + health);
    }

    private void AttackPlayer()
    {
        Debug.Log("Enemy attacks player!");
        // TODO: deal damage, play animation, etc.
        if (playerTarget != null)
        {
            PlayerStates playerStates = playerTarget.GetComponent<PlayerStates>();
            if (playerStates != null)
            {
                bool isCrit = Random.value < critChance;
                int totalDamage = isCrit ? damage * 2 : damage;
                playerStates.TakeDamage(totalDamage);
                Debug.Log("Dealt " + totalDamage + (isCrit ? " (Critical Hit!)" : "") + " damage to player.");
            }
        }
    }

}
