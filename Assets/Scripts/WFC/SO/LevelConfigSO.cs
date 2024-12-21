using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Scriptable Objects/Level Config SO", order = 0)]
public class LevelConfigSO : ScriptableObject
{
    [Header("Basic")]
    public int levelId;
    public string levelName;

    [Header("Difficulty")]
    [Range(1, 10)]
    public int levelDifficulty;

    [Space]
    public bool easyRooms;
    public bool mediumRooms;
    public bool hardRooms;

    [Space]
    [Tooltip("Number where easy rooms will stop spawning and medium rooms will spawn instead")]
    public int easyRoomBreakoff;
    [Tooltip("Number where medium rooms will stop spawning and hard rooms will spawn instead")]
    public int mediumRoomBreakoff;

    [Header("Branch")]
    public int trueBranchLength;
    public int deadBranchLength;

    [Header("Rooms")]
    [Tooltip("Number of enemy rooms between safe rooms")]
    public int safeRoomGaps;

    [Space]
    public int maxRoomCount;
    public int maxEnemyCount;
    public int maxTreasureCount;
    public int maxPuzzleCount;
    public int maxTrapCount;
    public int maxShopCount;
}
