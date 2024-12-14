using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D polygonCollider;

    public float GetWidth()
    {
        return polygonCollider.bounds.size.x;
    }

    public float GetHeight()
    {
        return polygonCollider.bounds.size.y;
    }
}
