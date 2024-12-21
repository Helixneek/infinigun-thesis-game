using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GemDistributor : MonoBehaviour
{
    private WFC_Drawer drawer;
    private List<RoomCreator> rooms;

    private EnemyDifficulty _highestDiff = EnemyDifficulty.Medium;

    private void Start()
    {
        drawer = GetComponent<WFC_Drawer>();
    }

    public void AssignGemToEnemy()
    {
        // Get list of rooms
        rooms = new List<RoomCreator>(drawer.roomObjects);

        // Get the highest difficulty
        if(drawer.levelConfig.mediumRooms && !drawer.levelConfig.hardRooms)
        {
            // Medium is highest
            _highestDiff = EnemyDifficulty.Medium;
        }
        else if(drawer.levelConfig.hardRooms && drawer.levelConfig.mediumRooms)
        {
            // Hard is highest
            _highestDiff = EnemyDifficulty.Hard;
        }

        // Filter list to only enemy rooms of the hardest difficulty
        //List<RoomCreator> filteredRooms = rooms.Where(n => n.EnemyDifficulty == _highestDiff).ToList();
        List<RoomCreator> filteredRooms = new List<RoomCreator>();

        for(int i = 0; i < rooms.Count; i++)
        {
            Debug.Log(rooms[i]);
            if (rooms[i] != null && rooms[i].roomType == RoomType.Enemy && rooms[i].EnemyDifficulty == _highestDiff)
            {
                filteredRooms.Add(rooms[i]);
            }
        }

        // Choose random room from that filtered list
        RoomCreator randomRoom = filteredRooms[Random.Range(0, filteredRooms.Count)];

        // Choose a random enemy
        Enemy randomEnemy = randomRoom.enemies[Random.Range(0, randomRoom.enemies.Length)];

        // Have it drop gems instead of coins/hearts
        randomEnemy.canDropGems = true;
        randomEnemy.gameObject.name = "THIS ONE HAS GEMS";
    }
}
