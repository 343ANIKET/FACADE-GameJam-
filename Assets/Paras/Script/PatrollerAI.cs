using UnityEngine;

public class PatrollerAI: EnemyBase
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float waitTime = 1f;

    private Transform targetPoint;
    private bool isWaiting = false;

    protected override void Awake()
    {
        base.Awake(); // Grabs rb and sprite from EnemyBase
        targetPoint = pointB;
    }

    void Update()
    {
        if (isWaiting) return;

        MoveTowardsTarget();

        // Check if we reached the target (using a small threshold)
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            StartCoroutine(SwitchTarget());
        }
    }

    void MoveTowardsTarget()
    {
        // Move towards target on X axis
        float step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, step);

        // Flip sprite based on direction
        if (targetPoint.position.x > transform.position.x && transform.localScale.x < 0)
            Flip();
        else if (targetPoint.position.x < transform.position.x && transform.localScale.x > 0)
            Flip();
    }

    System.Collections.IEnumerator SwitchTarget()
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

    // Visual Gizmos for Editor
    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
    }