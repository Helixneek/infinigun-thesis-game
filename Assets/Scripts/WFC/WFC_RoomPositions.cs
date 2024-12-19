using UnityEngine;

public class WFC_RoomPositions : MonoBehaviour
{
    // Get the positions of safe rooms in a level
    public int[] GetSafeRoomPositions(int diff)
    {
        int[] pos = diff switch
        {
            1 => new int[6] { 2, 4, 6, 8, 10, 12 },
            2 => new int[5] { 2, 5, 7, 10, 14 },
            3 => new int[5] { 3, 6, 10, 13, 16 },
            4 => new int[4] { 4, 8, 12, 16 },
            5 => new int[5] { 4, 8, 13, 14, 19 },
            6 => new int[5] { 1, 5, 10, 15, 19 },
            _ => new int[6] { 2, 4, 6, 8, 10, 12 }, // default case
        };

        return pos;
    }
}
