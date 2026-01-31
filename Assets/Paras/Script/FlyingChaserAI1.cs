using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class FlyingChaserAI1 : EnemyBase
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;   // A B C D
    public float patrolSpeed = 2f;
    public float waitTime = 1f;

    [Header("Chase Settings")]
    public float detectionRadius = 5f;
    public float chaseSpeed = 4f;

    [Header("Attack & Bounce")]
    public float bounceForce = 7f;
    public float bounceDuration = 0.4f;

    private Transform player;

    private int currentIndex = 0;

    private bool isWaiting = false;
    private bool isChasing = false;
    private bool isBouncing = false;

    // ------------------------------------------------

    protected override void Awake()
    {
        base.Awake();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    // ------------------------------------------------

    void Update()
    {
        if (isBouncing) return;

        CheckDetection();

        if (isChasing)
            ChasePlayer();
        else if (!isWaiting)
            Patrol();

        if (Input.GetKeyDown(KeyCode.K))
            DebugTakeDamage();
    }

    // ------------------------------------------------
    // DETECTION
    // ------------------------------------------------

    void CheckDetection()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist < detectionRadius)
        {
            isChasing = true;
        }
        else if (isChasing && dist > detectionRadius)
        {
            isChasing = false;
            rb.linearVelocity = Vector2.zero;
            FindNearestPatrolPoint();
        }
    }

    // ------------------------------------------------
    // PATROL A → B → C → D → LOOP
    // ------------------------------------------------

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentIndex];

        MoveTowards(target.position, patrolSpeed);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitAndNext());
        }
    }

    IEnumerator WaitAndNext()
    {
        isWaiting = true;

        yield return new WaitForSeconds(waitTime);

        currentIndex++;

        if (currentIndex >= patrolPoints.Length)
            currentIndex = 0;

        isWaiting = false;
    }

    void FindNearestPatrolPoint()
    {
        float minDist = Mathf.Infinity;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float d = Vector2.Distance(transform.position, patrolPoints[i].position);

            if (d < minDist)
            {
                minDist = d;
                currentIndex = i;
            }
        }
    }

    // ------------------------------------------------
    // CHASE
    // ------------------------------------------------

    void ChasePlayer()
    {
        MoveTowards(player.position, chaseSpeed);
    }

    // ------------------------------------------------
    // MOVEMENT
    // ------------------------------------------------

    void MoveTowards(Vector2 destination, float speed)
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            destination,
            speed * Time.deltaTime
        );

        if (destination.x > transform.position.x && transform.localScale.x < 0) Flip();
        else if (destination.x < transform.position.x && transform.localScale.x > 0) Flip();
    }

    void Flip()
    {
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    // ------------------------------------------------
    // BOUNCE
    // ------------------------------------------------

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBouncing)
        {
            Debug.Log("Hit Player!");
            StartCoroutine(HandleBounce(other.transform.position));
        }
    }

    IEnumerator HandleBounce(Vector2 playerPos)
    {
        isBouncing = true;

        Vector2 dir = ((Vector2)transform.position - playerPos).normalized;
        dir += Vector2.up * 0.5f;

        rb.linearVelocity = dir.normalized * bounceForce;

        yield return new WaitForSeconds(bounceDuration);

        rb.linearVelocity = Vector2.zero;
        isBouncing = false;
    }

    // ------------------------------------------------
    // DEBUG
    // ------------------------------------------------

    public void DebugTakeDamage()
    {
        EnemyBase[] allEnemies = GameObject.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

        foreach (EnemyBase enemy in allEnemies)
            enemy.TakeDamage(20);

        Debug.Log("Debug: Damaged all enemies!");
    }

    // ------------------------------------------------
    // GIZMOS
    // ------------------------------------------------

    private void OnDrawGizmos()
    {
        if (patrolPoints != null && patrolPoints.Length > 1)
        {
            Gizmos.color = Color.yellow;

            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Transform a = patrolPoints[i];
                Transform b = patrolPoints[(i + 1) % patrolPoints.Length];

                if (a != null && b != null)
                    Gizmos.DrawLine(a.position, b.position);
            }
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
