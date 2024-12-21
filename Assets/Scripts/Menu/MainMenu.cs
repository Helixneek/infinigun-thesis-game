using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gemsText;

    private void Start()
    {
        SetGemsText();
    }

    private void SetGemsText()
    {
        if(PlayerDataManager.Instance != null)
        {
            gemsText.text = PlayerDataManager.Instance.Gems.ToString();
            gemsText.fontSize = 60;
        }
        else
        {
            gemsText.text = "Not yet, play the game first!";
            gemsText.fontSize = 50;
        }
    }

    public void OpenLevel(int number)
    {
        SceneManager.LoadScene("Level " + number);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
