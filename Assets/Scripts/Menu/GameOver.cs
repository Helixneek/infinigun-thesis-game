using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private string targetSceneName;

    public void ConcludeRun()
    {
        if(PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.isFirst = true;
            PlayerDataManager.Instance.SetTotalGems();
        }
        
        ChangeScene();
    }

    private void ChangeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
    }
}
