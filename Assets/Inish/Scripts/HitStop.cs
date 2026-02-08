using UnityEngine;
using System.Collections;

public class HitStop : MonoBehaviour
{
    static bool isStopping;
    static HitStop instance;

    void Awake()
    {
        instance = this;
    }

    public static void Do(float duration)
    {
        if (isStopping || instance == null) return;
        instance.StartCoroutine(StopRoutine(duration));
    }

    static IEnumerator StopRoutine(float duration)
    {
        isStopping = true;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        isStopping = false;
    }
}
