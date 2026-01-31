using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class FlyingChaserAI1 : EnemyBase
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;   // A, B, C, D
    public float patrolSpeed = 2f;
    public float waitTime = 1f;

    [Header("Chase Settings")]
    public float detectionRadius = 5f;
    public float chaseSpeed = 4f;

    [Header("Post-Chase Search")]
    public float searchTime = 2f;      // How long to wait when player is lost
    private bool isSearching = false;

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

        // Settings for flying movement
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    // ------------------------------------------------

    void Update()
    {
        // Priority 1: If bouncing from a hit, do nothing else
        if (isBouncing) return;

        CheckDetection();

        // Priority 2: If searching (hovering), do nothing else
        if (isSearching) return;

        // Priority 3: Movement Logic
        if (isChasing)
            ChasePlayer();
        else if (!isWaiting)
            Patrol();

        // Debug Key
        if (Input.GetKeyDown(KeyCode.K))
            DebugTakeDamage();
    }

    // ------------------------------------------------
    // DETECTION & SEARCH LOGIC
    // ------------------------------------------------

    void CheckDetection()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist < detectionRadius)
        {
            // If we were searching or waiting, stop that and chase immediately
            if (isSearching || isWaiting)
            {
                StopAllCoroutines();
                isSearching = false;
                isWaiting = false;
            }
            isChasing = true;
        }
        else if (isChasing && dist > detectionRadius)
        {
            // Player just went out of range
            StartCoroutine(SearchBeforeReturn());
        }
    }

    IEnumerator SearchBeforeReturn()
    {
        isChasing = false;
        isSearching = true;

        // Stop movement immediately
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(searchTime);

        isSearching = false;
        FindNearestPatrolPoint();
    }

    // ------------------------------------------------
    // PATROL LOGIC
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
    // MOVEMENT & COMBAT
    // ------------------------------------------------

    void ChasePlayer()
    {
        MoveTowards(player.position, chaseSpeed);
    }

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

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBouncing)
        {
            // If we hit the player, stop searching/chasing and bounce
            StopAllCoroutines();
            isSearching = false;
            isWaiting = false;

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

        // After bouncing, decide whether to re-detect or search
        CheckDetection();
    }

    // ------------------------------------------------
    // DEBUG & GIZMOS
    // ------------------------------------------------

    public void DebugTakeDamage()
    {
        EnemyBase[] allEnemies = GameObject.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        foreach (EnemyBase enemy in allEnemies)
            enemy.TakeDamage(20);

        Debug.Log("Debug: Damaged all enemies!");
    }

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

        Gizmos.color = isSearching ? Color.magenta : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}