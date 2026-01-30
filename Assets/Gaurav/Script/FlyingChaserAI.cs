using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class FlyingChaserAI : EnemyBase
{
    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;
    public float waitTime = 1f;

    [Header("Chase Settings")]
    public float detectionRadius = 5f;
    public float chaseSpeed = 4f;

    [Header("Attack & Bounce")]
    public float bounceForce = 7f;
    public float bounceDuration = 0.4f;

    private Transform player;
    private Transform targetPoint;
    private bool isWaiting = false;
    private bool isChasing = false;
    private bool isBouncing = false;

    protected override void Awake()
    {
        base.Awake(); // Gets rb and sprite from EnemyBase

        // Setup initial patrol
        targetPoint = pointB;

        // Auto-find player
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // Ensure Rigidbody is set up for flying
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 1. If we are currently bouncing back from an attack, skip AI logic
        if (isBouncing) return;

        // 2. Decide if we should chase or patrol
        CheckDetection();

        if (isChasing)
        {
            ChasePlayer();
        }
        else if (!isWaiting)
        {
            Patrol();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            DebugTakeDamage();
        }
    }

    void CheckDetection()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Switch to chasing
        if (dist < detectionRadius)
        {
            isChasing = true;
        }
        // Return to patrol if player escapes
        else if (isChasing && dist > detectionRadius)
        {
            isChasing = false;
            rb.linearVelocity = Vector2.zero; // Kill momentum
            FindNearestPatrolPoint();
        }
    }

    void Patrol()
    {
        MoveTowards(targetPoint.position, patrolSpeed);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            StartCoroutine(WaitAndSwitch());
        }
    }

    void ChasePlayer()
    {
        MoveTowards(player.position, chaseSpeed);
    }

    void MoveTowards(Vector2 destination, float currentSpeed)
    {
        transform.position = Vector2.MoveTowards(transform.position, destination, currentSpeed * Time.deltaTime);

        // Flip sprite based on movement direction
        if (destination.x > transform.position.x && transform.localScale.x < 0) Flip();
        else if (destination.x < transform.position.x && transform.localScale.x > 0) Flip();
    }

    void FindNearestPatrolPoint()
    {
        float distToA = Vector2.Distance(transform.position, pointA.position);
        float distToB = Vector2.Distance(transform.position, pointB.position);
        targetPoint = (distToA < distToB) ? pointA : pointB;
    }

    IEnumerator WaitAndSwitch()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        targetPoint = (targetPoint == pointA) ? pointB : pointA;
        isWaiting = false;
    }

    // This overrides the damage logic in EnemyBase to add the bounce effect
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBouncing)
        {
            // Deal damage (inherits from EnemyBase logic)
            Debug.Log("Hit Player!");
            // other.GetComponent<PlayerScript>().TakeDamage(contactDamage);

            // Start Bounce Mechanic
            StartCoroutine(HandleBounce(other.transform.position));
        }
    }

    IEnumerator HandleBounce(Vector2 playerPos)
    {
        isBouncing = true;

        // Calculate direction: Away from player and slightly up
        Vector2 bounceDir = ((Vector2)transform.position - playerPos).normalized;
        bounceDir += Vector2.up * 0.5f;

        // Apply physics burst
        rb.linearVelocity = bounceDir.normalized * bounceForce;

        yield return new WaitForSeconds(bounceDuration);

        rb.linearVelocity = Vector2.zero;
        isBouncing = false;
    }

    void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    // Visual Gizmos for Editor
    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void DebugTakeDamage()
    {
        // Find every object in the scene that inherits from EnemyBase
        EnemyBase[] allEnemies = GameObject.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

        foreach (EnemyBase enemy in allEnemies)
        {
            enemy.TakeDamage(20); // Deals 20 damage to every enemy on screen
        }

        Debug.Log("Debug: Damaged all enemies!");
    }


}