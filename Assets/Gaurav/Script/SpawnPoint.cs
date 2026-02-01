using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public static Transform Active;

    void Awake()
    {
        if (Active == null)
            Active = transform;
    }
}
