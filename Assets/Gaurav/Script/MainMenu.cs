using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "Game"; // your gameplay scene name

    // PLAY BUTTON
    public void PlayGame()
    {
        Debug.Log("Play");
        SceneManager.LoadScene("Prolouge");
    }

    // QUIT BUTTON
    public void QuitGame()
    {
        Debug.Log("Quit Game");

        Application.Quit();

        // Stop play mode inside Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
