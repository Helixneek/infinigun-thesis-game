using UnityEngine;

public class EnemyRoomHandler : MonoBehaviour
{
    private PolygonCollider2D _collider;

    private RoomCreator _roomData;

    private void Start()
    {
         _collider = GetComponent<PolygonCollider2D>();
        _roomData = GetComponent<RoomCreator>();
    }

    private void Update()
    {
        if(_roomData.roomType != RoomType.Enemy || _roomData.roomType != RoomType.Boss) return;


    }
}
