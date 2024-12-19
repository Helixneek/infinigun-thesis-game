using UnityEngine;
using UnityEngine.SceneManagement;

public class NextFloorTrigger : TriggerInteractionBase
{
    public int currentFloor;
    public int nextFloor;

    public bool isLocked = true;

    private void Start()
    {
        currentFloor = SceneManager.GetActiveScene().buildIndex;
        nextFloor = currentFloor + 1;
    }

    public override void Interact()
    {
        if(isLocked) return;

        // Replace player object reference in singleton
        PlayerDataManager.Instance.ReplacePlayerClone(Player);

        // Increment level id
        PlayerDataManager.Instance.levelID++;

        SceneManager.LoadScene(nextFloor);
    }
}
