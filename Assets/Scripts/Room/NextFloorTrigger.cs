using UnityEngine;
using UnityEngine.SceneManagement;

public class NextFloorTrigger : TriggerInteractionBase
{
    public int currentFloor;
    public int nextFloor;

    public bool isLocked = true;
    public bool finalFloor = false;

    private void Start()
    {
        currentFloor = SceneManager.GetActiveScene().buildIndex;
        nextFloor = currentFloor + 1;
    }

    public override void Interact()
    {
        if(isLocked) return;

        // Set player gems in the singleton
        PlayerDataManager.Instance.Gems = Player.GetComponent<PlayerWallet>().Gems;

        if (finalFloor)
        {
            // Delete player object
            PlayerDataManager.Instance.DeletePlayerClone();

            // This is supposed to be a "You Win!" scene
            SceneManager.LoadScene(7);
        }
        else
        {
            // Replace player object reference in singleton
            PlayerDataManager.Instance.ReplacePlayerClone(Player);

            // Increment level id
            PlayerDataManager.Instance.levelID++;

            // Load next floor
            SceneManager.LoadScene(nextFloor);
        }
        
    }
}
