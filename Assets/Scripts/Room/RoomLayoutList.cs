using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomLayoutList", menuName = "Scriptable Objects/Room Layout List")]
public class RoomLayoutList : ScriptableObject
{

    public List<GameObject> emptyRoomLayouts = new List<GameObject>();

    [Space]
    public List<GameObject> easyEnemyRoomLayouts = new List<GameObject>();
    public List<GameObject> mediumEnemyRoomLayouts = new List<GameObject>();
    public List<GameObject> hardEnemyRoomLayouts = new List<GameObject>();

    [Space]
    public List<GameObject> treasureRoomLayouts = new List<GameObject>();

    [Space]
    public List<GameObject> puzzleRoomLayouts = new List<GameObject>();

    [Space]
    public List<GameObject> trapRoomLayouts = new List<GameObject>();

    [Space]
    public List<GameObject> shopRoomLayouts = new List<GameObject>();

    [Space]
    public List<GameObject> bossRoomLayouts = new List<GameObject>();
}
