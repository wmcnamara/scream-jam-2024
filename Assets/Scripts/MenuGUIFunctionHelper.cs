using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGUIFunctionHelper : MonoBehaviour
{
    public void StartGame()
    {
        UnpauseGame();  // Ensure game is unpaused
        SceneManager.LoadScene("Level1");
    }

    public void TogglePause()
    {
        PauseManager.Instance.TogglePause();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenMainMenu()
    {
        UnpauseGame();  // Ensure game is unpaused
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenCredits()
    {
        UnpauseGame();  // Ensure game is unpaused
        SceneManager.LoadScene("Credits");
    }

    // Helper function to unpause the game before loading a new scene
    private void UnpauseGame()
    {
        Time.timeScale = 1.0f;
    }
}
