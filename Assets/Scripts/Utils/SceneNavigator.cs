using UnityEngine;

public class SceneNavigator : MonoBehaviour
{
    [SerializeField] private string targetSceneName;

    public void ChangeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
    }
}
