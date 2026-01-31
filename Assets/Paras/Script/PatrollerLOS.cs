using UnityEngine;
using System.Collections;

public class PatrollerLOS : EnemyBase
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float waitTime = 1f;

    [Header("Detection Settings")]
    public float viewDistance = 5f;
    public LayerMask obstacleLayer; // Should include 'Ground' but NOT 'Player'
    public float detectionRefreshRate = 0.1f;

    [Header("Audio Settings")]
    public AudioClip attackSound; // Drag your attack sound here in Inspector
    public AudioSource audioSource;

    private Transform targetPoint;
    private Transform player;
    private bool isWaiting = false;
    private bool isChasing = false;
    public PlayerCombat playercombat;

    protected override void Awake()
    {
        base.Awake();
        targetPoint = pointB;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // 1. Look for player
        CheckLineOfSight();

        if (isChasing)
        {
            ChasePlayer();
        }
        else if (!isWaiting)
        {
            Patrol();
        }
        if (Input.GetKeyDown(KeyCode.K))
            DebugTakeDamage();
    }

    void CheckLineOfSight()
    {
        if (player == null) return;

        // Calculate direction based on where the enemy is facing
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Cast a ray forward
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, viewDistance, obstacleLayer | (1 << player.gameObject.layer));

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Debug.Log(hit);
            if (!isChasing) // Only play sound when first detecting player
            {
                PlayAttackSound();
            }
            isChasing = true;
        }
        else
        {
            // lose interest when player hides
            // isChasing = false; 
        }
    }

    void Patrol()
    {
        MoveTowards(targetPoint.position, patrolSpeed);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            StartCoroutine(SwitchTarget());
        }
    }

    void ChasePlayer()
    {
        // Move towards player on X axis only (Ground Patroller logic)
        Vector2 playerPosOnLevel = new Vector2(player.position.x, transform.position.y);
        MoveTowards(playerPosOnLevel, chaseSpeed);

        // Stop chasing if player gets too far away
        if (Vector2.Distance(transform.position, player.position) > viewDistance * 1.5f)
        {
            isChasing = false;
            targetPoint = GetNearestPoint();
        }
    }

    void MoveTowards(Vector2 destination, float currentSpeed)
    {
        float step = currentSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, destination, step);

        // Flip based on movement
        if (destination.x > transform.position.x && transform.localScale.x < 0) Flip();
        else if (destination.x < transform.position.x && transform.localScale.x > 0) Flip();
    }

    Transform GetNearestPoint()
    {
        return Vector2.Distance(transform.position, pointA.position) < Vector2.Distance(transform.position, pointB.position) ? pointA : pointB;
    }

    IEnumerator SwitchTarget()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        targetPoint = (targetPoint == pointA) ? pointB : pointA;
        isWaiting = false;
    }

    void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void DebugTakeDamage()
    {
        EnemyBase[] allEnemies = GameObject.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

        foreach (EnemyBase enemy in allEnemies)
            enemy.TakeDamage(20);

        Debug.Log("Debug: Damaged all enemies!");
    }

    private void OnDrawGizmos()
    {
        // Patrol Path
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }

        // Sight Line
        Gizmos.color = isChasing ? Color.red : Color.cyan;
        Vector3 direction = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        Gizmos.DrawRay(transform.position, direction * viewDistance);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"{gameObject.name} hit the player!");

            // Play attack sound on contact
            PlayAttackSound();

            playercombat.TakeDamage(contactDamage, transform.position);
        }
    }
}