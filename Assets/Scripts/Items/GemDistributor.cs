using System;
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
        try
        {
            // Get list of rooms
            rooms = new List<RoomCreator>(drawer.roomObjects);

            // Get the highest difficulty
            if (drawer.levelConfig.mediumRooms && !drawer.levelConfig.hardRooms)
            {
                // Medium is highest
                _highestDiff = EnemyDifficulty.Medium;
            }
            else if (drawer.levelConfig.hardRooms && drawer.levelConfig.mediumRooms)
            {
                // Hard is highest
                _highestDiff = EnemyDifficulty.Hard;
            }

            // Filter list to only enemy rooms of the hardest difficulty
            Debug.Log("Rooms length: " + rooms.Count);

            List<RoomCreator> filteredRooms = rooms.Where(n => n != null && n.roomType == RoomType.Enemy && n.EnemyDifficulty == _highestDiff).ToList();

            Debug.Log("Filtered rooms length: " + filteredRooms.Count);
            // Choose random room from that filtered list
            RoomCreator randomRoom = filteredRooms[UnityEngine.Random.Range(0, filteredRooms.Count - 1)];

            // Choose a random enemy
            Enemy randomEnemy = randomRoom.enemies[UnityEngine.Random.Range(0, randomRoom.enemies.Length - 1)];

            // Have it drop gems instead of coins/hearts
            randomEnemy.canDropGems = true;
            randomEnemy.gameObject.name = "THIS ONE HAS GEMS";
            randomEnemy.ChangeColor();
        }
        catch (Exception e)
        {
            // give the fucking gem to a random enemy idk
            List<Enemy> enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList();

            Enemy target = enemies[UnityEngine.Random.Range(0, enemies.Count - 1)];

            target.canDropGems = true;
            target.gameObject.name = "THIS ONE HAS GEMS";
            target.ChangeColor();
        }
    }
}
