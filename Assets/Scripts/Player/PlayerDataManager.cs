using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    // Singleton instance
    public static PlayerDataManager Instance { get; private set; }

    // General player info
    public GameObject PlayerClone;
    public int levelID;

    // Gem buffs
    public float permaDamageLevel = 0;
    public float permaFireRateLevel = 0;
    public int permaMaxAmmoLevel = 0;
    public float permaReloadSpeedLevel = 0;

    private void Awake()
    {
        // Setup singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void ReplacePlayerClone(GameObject newClone)
    {
        // Delete gameObject
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Add new gameObject and replace reference
        PlayerClone = Instantiate(newClone, transform);

        // Turn off gameObject
        PlayerClone.SetActive(false);

    }
}
