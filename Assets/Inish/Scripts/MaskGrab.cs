using UnityEngine;

public class MaskGrab : MonoBehaviour
{
    [Header("Bobbing")]
    public float bobHeight = 0.25f;
    public float bobSpeed = 2f;
    public GameObject SceneChanger;
    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        SceneChanger.SetActive(false);
    }

    void Update()
    {
        // Smooth vertical bobbing
        float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPosition + Vector3.up * bobOffset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (MaskController.Instance != null)
        {
            MaskController.Instance.EnableMask();
           

        }

        Debug.Log("Mask Grabbed");
        Destroy(gameObject);
        SceneChanger.SetActive(true);
    }
}
