using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Level Select")]
    [SerializeField] private Slider levelSlider;
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("Gems")]
    [SerializeField] private TextMeshProUGUI gemsText;

    private void Start()
    {
        SetGemsText();
    }

    public void OnSliderChange()
    {
        buttonText.text = "START FROM LEVEL " + levelSlider.value;
    }

    public void ChooseLevel()
    {
        SceneManager.LoadScene("Level " + levelSlider.value);
    }

    private void SetGemsText()
    {
        if(PlayerDataManager.Instance != null)
        {
            gemsText.text = PlayerDataManager.Instance.TotalGems.ToString();
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
