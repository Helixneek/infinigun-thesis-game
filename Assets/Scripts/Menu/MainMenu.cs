using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OpenLevel(int number)
    {
        SceneManager.LoadScene("Level " + number);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
