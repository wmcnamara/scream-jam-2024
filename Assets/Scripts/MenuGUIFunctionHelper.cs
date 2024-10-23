using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGUIFunctionHelper : MonoBehaviour
{
    public void StartGame()
    {
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
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
