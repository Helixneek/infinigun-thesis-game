using UnityEngine;

[System.Serializable]
public class WFC_TileTemplate
{
    public string name;
    public RoomCreator roomData;

    [Space]
    public string up;
    public string right;
    public string down;
    public string left;
}