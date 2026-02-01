using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(AudioSource))]
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

    [Header("Audio")]
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip catchSound;

    [Header("Input")]
    public KeyCode throwKey = KeyCode.Mouse0;

    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer sr;
    AudioSource audioSource;

    Transform player;

    bool isThrown = false;
    bool returning = false;

    float timer;
    Vector3 localStartPos;

    // =========================================================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        player = transform.parent;
        localStartPos = transform.localPosition;

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        audioSource.playOnAwake = false;

        DisableBoomerang();
    }

    // =========================================================
    void Update()
    {
        if (!isThrown)
        {
            // 🔒 BLOCK THROW IF MASK NOT OWNED
            if (Input.GetKeyDown(throwKey))
            {
                if (MaskController.Instance != null &&
                    MaskController.Instance.HasMask)
                {
                    Throw();
                }
                else
                {
                    // Optional debug (remove later)
                    Debug.Log("Cannot throw boomerang — mask not acquired");
                }
            }

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
    void Throw()
    {
        isThrown = true;
        returning = false;
        timer = 0f;

        PlaySfx(throwSound);

        EnableBoomerang();

        transform.SetParent(null);

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector2 dir = (mouseWorld - transform.position).normalized;
        rb.linearVelocity = dir * speed;
    }

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
    void Catch()
    {
        isThrown = false;

        PlaySfx(catchSound);

        transform.SetParent(player, false);
        transform.localPosition = localStartPos;
        transform.localRotation = Quaternion.identity;

        DisableBoomerang();
    }

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isThrown) return;

        if (other.TryGetComponent(out EnemyBase enemy))
        {
            enemy.TakeDamage(damage);
            PlaySfx(hitSound);
        }
    }

    private void PlaySfx(AudioClip clip)
    {
        if (audioSource && clip)
            audioSource.PlayOneShot(clip);
    }
}
