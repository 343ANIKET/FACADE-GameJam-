using UnityEngine;

public class DashAfterimage : MonoBehaviour
{
    public float fadeSpeed = 6f;

    SpriteRenderer sr;
    Color color;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // index: 0 = strongest, max-1 = weakest
    public void Init(Sprite sprite, bool flipX, Vector3 scale, int index, int max)
    {
        sr.sprite = sprite;
        sr.flipX = flipX;
        transform.localScale = scale;

        // Yellow color
        color = Color.yellow;

        // Alpha decreases per image
        float t = (float)index / (max - 1); // 0 → 1
        color.a = Mathf.Lerp(0.8f, 0.2f, t);

        sr.color = color;
    }

    void Update()
    {
        color.a -= fadeSpeed * Time.deltaTime;
        sr.color = color;

        if (color.a <= 0f)
            Destroy(gameObject);
    }
}
