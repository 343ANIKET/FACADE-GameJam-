using UnityEngine;
using System.Collections;

public class TutScript : MonoBehaviour
{
    public GameObject Text1;
    public GameObject Text2;
    public GameObject Text3;

    void Start()
    {
        StartCoroutine(DeleteTextsAfterDelay(10f));
    }

    IEnumerator DeleteTextsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (Text1) Destroy(Text1);
        if (Text2) Destroy(Text2);
        if (Text3) Destroy(Text3);
    }
}
