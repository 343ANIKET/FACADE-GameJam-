using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject mainMenuPanel;

    private bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !mainMenuPanel.activeSelf)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    // CONTINUE
    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // PAUSE
    void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    // QUIT → go back to main menu panel
    public void QuitGame()
    {
        Time.timeScale = 1f;

        pausePanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        isPaused = false;
    }
}
