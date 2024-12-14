using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int gridWidth, gridHeight;
    [SerializeField] private Transform gridParent;

    [Space]
    [SerializeField] private float tileWidth, tileHeight;
    [SerializeField] private Tile tilePrefab;

    private Dictionary<Vector2, Tile> _tiles = new Dictionary<Vector2, Tile>();

    //private float _width, _height;

    private void Start()
    {
        //if(_width == 0 || _height == 0)
        //{
        //    _width = tilePrefab.GetWidth();
        //    _height = tilePrefab.GetHeight();
        //}

        GenerateBasicGrid();
    }

    void GenerateBasicGrid()
    {
        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x * tileWidth, y * tileHeight, 0), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.transform.parent = gridParent;

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }
    }
}
