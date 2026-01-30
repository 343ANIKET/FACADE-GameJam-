using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BoomerangChild : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 14f;
    public float returnSpeed = 18f;
    public float travelTime = 0.5f;

    [Header("Spin")]
    public float rotateSpeed = 900f;

    [Header("Combat")]
    public int damage = 20;

    [Header("Input")]
    public KeyCode throwKey = KeyCode.Mouse0;

    private Rigidbody2D rb;
    private Transform player;

    private bool isThrown = false;
    private bool returning = false;

    private float timer;

    private Vector3 localStartPos;

    // ------------------------------------------------

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        player = transform.parent;
        localStartPos = transform.localPosition;

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        Attach(); // 🔥 start attached properly
    }

    // ------------------------------------------------

    void Update()
    {
        if (!isThrown)
        {
            if (Input.GetKeyDown(throwKey))
                Throw();

            return;
        }

        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

        timer += Time.deltaTime;

        if (!returning && timer >= travelTime)
            returning = true;

        if (returning)
            ReturnToPlayer();
    }

    // ------------------------------------------------
    // ATTACH (FOLLOW PLAYER)
    // ------------------------------------------------

    void Attach()
    {
        rb.simulated = false;        // ⭐ MOST IMPORTANT LINE

        transform.SetParent(player, false);
        transform.localPosition = localStartPos;
        transform.localRotation = Quaternion.identity;

        isThrown = false;
    }


    // ------------------------------------------------
    // THROW
    // ------------------------------------------------

    void Throw()
    {
        isThrown = true;

        transform.SetParent(null);

        rb.simulated = true;   // ⭐ enable physics again

        Vector2 dir = player.localScale.x > 0 ? Vector2.right : Vector2.left;
        rb.linearVelocity = dir * speed;
    }


    // ------------------------------------------------
    // RETURN
    // ------------------------------------------------

    void ReturnToPlayer()
    {
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = dir * returnSpeed;

        if (Vector2.Distance(rb.position, player.position) < 0.4f)
        {
            Catch();
        }
    }

    void Catch()
    {
        rb.linearVelocity = Vector2.zero;

        Attach(); // 🔥 use attach method
    }

    // ------------------------------------------------
    // DAMAGE
    // ------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isThrown) return;

        if (other.TryGetComponent(out EnemyBase enemy))
        {
            enemy.TakeDamage(damage);
        }
    }
}
