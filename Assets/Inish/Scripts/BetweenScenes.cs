using UnityEngine;
using UnityEngine.SceneManagement;

public class BetweenScenes : MonoBehaviour
{
    public string sceneToLoad = "Level1";

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Mouse Button 1 (Left Click)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
