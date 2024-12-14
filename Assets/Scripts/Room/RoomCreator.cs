using UnityEngine;

public class RoomCreator : MonoBehaviour
{
    public int roomID = -1;
    public RoomType roomType;
    public bool isFirst = false;

    public DoorTriggerInteraction[] doorObjects;

    private LayoutManager _layoutManager;
    private RoomLayoutList _layoutList;
    private Transform _gridTransform;

    public void SetupRoom(RoomType roomType)
    {
        this.roomType = roomType;

        doorObjects = new DoorTriggerInteraction[4] { new(), new(), new(), new() };

        GenerateRoom();
    }

    public void GenerateRoom()
    {
        Debug.Log("[GenerateRoom] Generating room contents");

        // Get the layout manager and grab the layout list
        _layoutManager = FindObjectsByType<LayoutManager>(FindObjectsSortMode.None)[0];
        _layoutList = _layoutManager.layoutList;

        // Get the grid transform so you can put in the room tilemap
        // Its the second object
        _gridTransform = transform.GetChild(1);

        switch (roomType)
        {
            case RoomType.Empty:
                int index = Random.Range(0, _layoutList.emptyRoomLayouts.Count);
                GameObject content = Instantiate(_layoutList.emptyRoomLayouts[index], _gridTransform);
                break;

            case RoomType.Enemy:
                index = Random.Range(0, _layoutList.enemyRoomLayouts.Count);
                content = Instantiate(_layoutList.enemyRoomLayouts[index], _gridTransform);
                break;

            case RoomType.Treasure:
                index = Random.Range(0, _layoutList.treasureRoomLayouts.Count);
                content = Instantiate(_layoutList.treasureRoomLayouts[index], _gridTransform);
                break;

            case RoomType.Puzzle:
                index = Random.Range(0, _layoutList.puzzleRoomLayouts.Count);
                content = Instantiate(_layoutList.puzzleRoomLayouts[index], _gridTransform);
                break;

            case RoomType.Trap:
                index = Random.Range(0, _layoutList.trapRoomLayouts.Count);
                content = Instantiate(_layoutList.trapRoomLayouts[index], _gridTransform);
                break;

            case RoomType.Shop:
                index = Random.Range(0, _layoutList.shopRoomLayouts.Count);
                content = Instantiate(_layoutList.shopRoomLayouts[index], _gridTransform);
                break;

            case RoomType.Boss:
                index = Random.Range(0, _layoutList.bossRoomLayouts.Count);
                content = Instantiate(_layoutList.bossRoomLayouts[index], _gridTransform);
                break;

            default:
                break;
        }
    }
    
    public void SetDoorIDs(int currentRoomID, int nextRoomID, int doorDirection)
    {
        doorObjects[doorDirection].currentRoomID = currentRoomID;
        doorObjects[doorDirection].nextRoomID = nextRoomID;
    }
}
