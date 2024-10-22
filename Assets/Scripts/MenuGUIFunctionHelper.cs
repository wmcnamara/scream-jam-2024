using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGUIFunctionHelper : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
