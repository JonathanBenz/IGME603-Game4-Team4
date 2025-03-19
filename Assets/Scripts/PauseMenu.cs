using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanvas;
    private bool paused;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        // Pause the game
        Time.timeScale = 0;
        pauseMenuCanvas.SetActive(true);
        paused = true;

    }

    public void ResumeGame()
    {
        // Resume the game
        Time.timeScale = 1;
        pauseMenuCanvas.SetActive(false);
        paused = false;
    }

    public void ToMainMenu()
    {
        // Load the main menu scene
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
