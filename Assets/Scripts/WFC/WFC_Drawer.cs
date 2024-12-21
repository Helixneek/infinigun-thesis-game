using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WFC_Drawer : MonoBehaviour
{
    [Header("Objects")]
    public LevelConfigSO levelConfig;
    [SerializeField] private RoomCreator roomPrefab;
    [SerializeField] private DoorTriggerInteraction doorPrefab;
    [SerializeField] private Vector2[] doorSpawnPositions;

    [Header("Draw Settings")]
    [SerializeField] private int tileWidth;
    [SerializeField] private int tileHeight;

    private GemDistributor gemDistributor;

    private int _dimension;
    private List<WFC_Cell> _grid;
    private WFC_Cell _firstCell;
    private Transform _gridParent;

    public List<RoomCreator> roomObjects;

    private int _totalRoomIndex = 0;

    // Difficulty parameters
    private int _maxEasyRooms = 0;
    private int _maxMediumRooms = 0;

    private int _easyRoomCount = 0;
    private int _mediumRoomCount = 0;

    private void Awake()
    {
        WaveFunctionCollapse wfc = GetComponent<WaveFunctionCollapse>();
        levelConfig = wfc.levelConfigFile;

        gemDistributor = GetComponent<GemDistributor>();
    }

    private void Start()
    {
        if(levelConfig.easyRooms)
        {
            _maxEasyRooms = levelConfig.easyRoomBreakoff;
        }

        if(levelConfig.mediumRooms)
        {
            _maxMediumRooms = levelConfig.mediumRoomBreakoff;
        }
    }

    public void StartDrawer(int dimension, List<WFC_Cell> grid, WFC_Cell fcell, Transform gridparent)
    {

        if (dimension <= 0 || grid == null || grid.Count < dimension * dimension)
        {
            Debug.LogError("Invalid StartDrawer parameters. Check dimension and grid!");
            return;
        }

        // Get the data from that function
        _dimension = dimension;
        _grid = grid;
        _firstCell = fcell;
        _gridParent = gridparent;

        // Draw the cell tiles
        DrawCellTiles();

        // Put the doors in
        CreateDoors();

        // Add the gem
        gemDistributor.AssignGemToEnemy();
    }

    private void DrawCellTiles()
    {
        roomObjects = new List<RoomCreator>(_dimension * _dimension);

        for (int j = 0; j < _dimension; j++)
        {
            for (int i = 0; i < _dimension; i++)
            {
                WFC_Cell cell = _grid[i + j * _dimension];

                if (cell != null && cell.collapsed)
                {
                    // Get index
                    int index = cell.options[0];

                    // If the cell is a wall
                    // Add an empty room and dont give an ID (it'll be -1)
                    if (cell.options[0] == (int)RoomType.Wall)
                    {
                        RoomCreator empty = null;
                        roomObjects.Add(empty);

                        continue;
                    }

                    // Create the room object
                    RoomCreator obj = Instantiate(roomPrefab, new Vector3(i * tileWidth, j * tileHeight, 0), Quaternion.identity);

                    // Set the ID of the room
                    // and the increment it
                    obj.roomID = _totalRoomIndex++;

                    // Set the difficulty if the room is an enemy room
                    if (cell.options[0] == (int)RoomType.Enemy)
                    {
                        if(levelConfig.easyRooms && _easyRoomCount <= _maxEasyRooms)
                        {
                            obj.EnemyDifficulty = EnemyDifficulty.Easy;
                            _easyRoomCount++;
                        }
                        else if(levelConfig.mediumRooms && _mediumRoomCount <= _maxMediumRooms)
                        {
                            obj.EnemyDifficulty = EnemyDifficulty.Medium;
                            _mediumRoomCount++;
                        }
                        else if(levelConfig.hardRooms)
                        {
                            obj.EnemyDifficulty = EnemyDifficulty.Hard;
                        }
                        else
                        {
                            obj.EnemyDifficulty = EnemyDifficulty.Easy;
                            _easyRoomCount++;
                        }
                    }

                    // Add room object to the list
                    roomObjects.Add(obj);

                    // Prepare room data and generate the contents
                    obj.SetupRoom(GetRoomTypeFromInt(cell.options[0]));

                    // Rename and organize in the hierarchy
                    if (cell.first)
                    {
                        obj.isFirst = true;
                        obj.gameObject.name = $"Start Room {i} {j}";
                        obj.tag = "Start Room";

                        _firstCell = cell;
                    }
                    else if(obj.roomType == RoomType.Enemy)
                    {
                        obj.gameObject.name = $"{obj.EnemyDifficulty} {obj.roomType} Room {i} {j}";
                    }
                    else
                    {
                        obj.gameObject.name = $"{obj.roomType} Room {i} {j}";
                    }

                    obj.gameObject.transform.parent = _gridParent;


                }

            }
        }

        //Debug.Log("[DrawCellTiles] Room Object count:" + _roomObjects.Count);
    }

    private RoomType GetRoomTypeFromInt(int val)
    {
        RoomType roomType;

        switch (val)
        {
            case 0:
                roomType = RoomType.Wall;
                break;
            case 1:
                roomType = RoomType.Empty;
                break;
            case 2:
                roomType = RoomType.Enemy;
                break;
            case 3:
                roomType = RoomType.Treasure;
                break;
            case 4:
                roomType = RoomType.Puzzle;
                break;
            case 5:
                roomType = RoomType.Trap;
                break;
            case 6:
                roomType = RoomType.Shop;
                break;
            case 7:
                roomType = RoomType.Boss;
                break;
            default:
                roomType = RoomType.Empty;
                break;
        }

        return roomType;
    }

    private void CreateDoors()
    {
        int _roomIndex = _totalRoomIndex; 

        for(int j = 0; j < _dimension; j++)
        {
            for(int i = 0; i < _dimension; i++)
            {
                WFC_Cell cell = _grid[i + j * _dimension];

                int index = i + j * _dimension;

                if (index >= _grid.Count || index < 0)
                {
                    Debug.LogError($"Index out of range: {index}, Grid Size: {_grid.Count}, i: {i}, j: {j}");
                }

                // Make sure that wall rooms are skipped
                if (cell != null && cell.collapsed && cell.options[0] != (int)RoomType.Wall)
                {
                    // Check each direction
                    // UP
                    if ((j + 1) < _dimension 
                        && _grid[i + (j + 1) * _dimension] != null 
                        && _grid[i + (j + 1) * _dimension].collapsed
                        && _grid[i + (j + 1) * _dimension].options[0] != (int)RoomType.Wall
                        && _grid[i + (j + 1) * _dimension].options[0] != (int)RoomType.Boss)
                    {
                        DoorTriggerInteraction door = Instantiate(doorPrefab, new Vector2(doorSpawnPositions[0].x + (i * tileWidth), doorSpawnPositions[0].y + (j * tileHeight)), Quaternion.identity);
                        door.gameObject.name = $"Up Door {i} {j}";

                        door.transform.SetParent(roomObjects[i + j * _dimension].transform);

                        //Debug.Log("[CreateDoors] DoorObjects length: " + _roomObjects[i + j * _dimension].doorObjects.Length);
                        roomObjects[i + j * _dimension].doorObjects[0] = door;
                    }

                    // RIGHT
                    if ((i + 1) < _dimension 
                        && _grid[(i + 1) + j * _dimension] != null 
                        && _grid[(i + 1) + j * _dimension].collapsed
                        && _grid[(i + 1) + j * _dimension].options[0] != (int)RoomType.Wall
                        && _grid[(i + 1) + j * _dimension].options[0] != (int)RoomType.Boss)
                    {
                        DoorTriggerInteraction door = Instantiate(doorPrefab, new Vector2(doorSpawnPositions[1].x + (i * tileWidth), doorSpawnPositions[1].y + (j * tileHeight)), Quaternion.Euler(0, 0, 270));
                        door.gameObject.name = $"Right Door {i} {j}";

                        door.transform.SetParent(roomObjects[i + j * _dimension].transform);

                        roomObjects[i + j * _dimension].doorObjects[1] = door;
                    }

                    // DOWN
                    if ((j - 1) >= 0
                        && _grid[i + (j - 1) * _dimension] != null 
                        && _grid[i + (j - 1) * _dimension].collapsed
                        && _grid[i + (j - 1) * _dimension].options[0] != (int)RoomType.Wall
                        && _grid[i + (j - 1) * _dimension].options[0] != (int)RoomType.Boss)
                    {
                        DoorTriggerInteraction door = Instantiate(doorPrefab, new Vector2(doorSpawnPositions[2].x + (i * tileWidth), doorSpawnPositions[2].y + (j * tileHeight)), Quaternion.Euler(0, 0, 180));
                        door.gameObject.name = $"Down Door {i} {j}";

                        door.transform.SetParent(roomObjects[i + j * _dimension].transform);

                        roomObjects[i + j * _dimension].doorObjects[2] = door;
                    }

                    // LEFT
                    if ((i - 1) >= 0 
                        && _grid[(i - 1) + j * _dimension] != null 
                        && _grid[(i - 1) + j * _dimension].collapsed 
                        && _grid[(i - 1) + j * _dimension].options[0] != (int)RoomType.Wall
                        && _grid[(i - 1) + j * _dimension].options[0] != (int)RoomType.Boss)
                    {
                        DoorTriggerInteraction door = Instantiate(doorPrefab, new Vector2(doorSpawnPositions[3].x + (i * tileWidth), doorSpawnPositions[3].y + (j * tileHeight)), Quaternion.Euler(0, 0, 90));
                        door.gameObject.name = $"Left Door {i} {j}";

                        door.transform.SetParent(roomObjects[i + j * _dimension].transform);

                        roomObjects[i + j * _dimension].doorObjects[3] = door;
                    }

                    // Give the door values
                    if(i >= 0 && i < _dimension && j >= 0 && j < _dimension)
                    {
                        AssignDoorValues(i, j);
                    }
                    
                }
                
            }
        }
    }

    private void AssignDoorValues(int x, int y)
    {
        // Check for each direction
        // UP
        if((y + 1) < _dimension
            && _grid[x + (y + 1) * _dimension] != null
            && _grid[x + (y + 1) * _dimension].collapsed
            && _grid[x + (y + 1) * _dimension].options[0] != (int)RoomType.Wall
            && roomObjects[x + (y + 1) * _dimension].roomID >= 0)
        {
            // Add the current room ID and next room ID to the doors
            roomObjects[x + y * _dimension].SetDoorIDs(
                roomObjects[x + y * _dimension].roomID, 
                roomObjects[x + (y + 1) * _dimension].roomID, 
                0);
        }

        // RIGHT
        if((x + 1) < _dimension
            && _grid[(x + 1) + y * _dimension] != null
            && _grid[(x + 1) + y * _dimension].collapsed
            && _grid[(x + 1) + y * _dimension].options[0] != (int)RoomType.Wall
            && roomObjects[(x + 1) + y * _dimension].roomID >= 0)
        {
            // Add the current room ID and next room ID to the doors
            roomObjects[x + y * _dimension].SetDoorIDs(
                roomObjects[x + y * _dimension].roomID,
                roomObjects[(x + 1) + y * _dimension].roomID,
                1);
        }

        // DOWN
        if ((y - 1) >= 0
            && (y - 1) < _dimension
            && _grid[x + (y - 1) * _dimension] != null
            && _grid[x + (y - 1) * _dimension].collapsed
            && _grid[x + (y - 1) * _dimension].options[0] != (int)RoomType.Wall
            && roomObjects[x + (y - 1) * _dimension].roomID >= 0)
        {
            // Add the current room ID and next room ID to the doors
            roomObjects[x + y * _dimension].SetDoorIDs(
                roomObjects[x + y * _dimension].roomID,
                roomObjects[x + (y - 1) * _dimension].roomID,
                2);
        }

        // LEFT
        if ((x - 1) >= 0
            && (x - 1) < _dimension
            && _grid[(x - 1) + y * _dimension] != null
            && _grid[(x - 1) + y * _dimension].collapsed
            && _grid[(x - 1) + y * _dimension].options[0] != (int)RoomType.Wall
            && roomObjects[(x - 1) + y * _dimension].roomID >= 0)
        {
            // Add the current room ID and next room ID to the doors
            roomObjects[x + y * _dimension].SetDoorIDs(
                roomObjects[x + y * _dimension].roomID,
                roomObjects[(x - 1) + y * _dimension].roomID,
                3);
        }
    }
}
