using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Boomerang : MonoBehaviour
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
    private Collider2D col;
    private SpriteRenderer sr;

    private Transform player;

    private bool isThrown = false;
    private bool returning = false;

    private float timer;
    private Vector3 localStartPos;

    // =========================================================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        player = transform.parent;
        localStartPos = transform.localPosition;

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        DisableBoomerang(); // start disabled
    }

    // =========================================================
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

    // =========================================================
    // THROW
    // =========================================================
    void Throw()
    {
        isThrown = true;
        returning = false;
        timer = 0f;

        EnableBoomerang();

        transform.SetParent(null);

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector2 dir = (mouseWorld - transform.position).normalized;

        rb.linearVelocity = dir * speed;
    }

    // =========================================================
    // RETURN
    // =========================================================
    void ReturnToPlayer()
    {
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = dir * returnSpeed;

        if (Vector2.Distance(rb.position, player.position) < 0.4f)
        {
            Catch();
        }
    }

    // =========================================================
    // CATCH
    // =========================================================
    void Catch()
    {
        isThrown = false;

        transform.SetParent(player, false);
        transform.localPosition = localStartPos;
        transform.localRotation = Quaternion.identity;

        DisableBoomerang();
    }

    // =========================================================
    // ENABLE / DISABLE
    // =========================================================
    void EnableBoomerang()
    {
        rb.simulated = true;
        col.enabled = true;
        if (sr) sr.enabled = true;
    }

    void DisableBoomerang()
    {
        rb.linearVelocity = Vector2.zero;

        rb.simulated = false;
        col.enabled = false;
        if (sr) sr.enabled = false;
    }

    // =========================================================
    // DAMAGE
    // =========================================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isThrown) return;

        if (other.TryGetComponent(out EnemyBase enemy))
        {
            enemy.TakeDamage(damage);
        }
    }
}
