using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class WFC_Tile
{
    public RoomCreator room;
    public List<string> edges;

    public List<int> up;
    public List<int> right;
    public List<int> down;
    public List<int> left;

    // Constructor
    public WFC_Tile(RoomCreator room, string up, string right, string down, string left)
    {
        this.room = room;
        this.edges = new List<string>();
        edges.Add(up);
        edges.Add(left);
        edges.Add(right);
        edges.Add(down);

        this.up = new List<int>();
        this.right = new List<int>();
        this.down = new List<int>();
        this.left = new List<int>();
    }

    // Reverse a string
    public string ReverseString(string s)
    {
        return new string(s.Reverse().ToArray());
    }

    // Compare edges
    public bool CompareEdge(string s1, string s2)
    {
        return s1 == ReverseString(s2);
    }

    // Analyze edges of tiles and store possible options on this tile
    public void Analyze(WFC_Tile[] tiles)
    {
        for(int i = 0; i < tiles.Length; i++)
        {
            WFC_Tile tile = tiles[i];

            // Check up direction
            if (CompareEdge(tile.edges[2], tile.edges[0]))
            {
                this.up.Add(i);
            }

            // Check right direction
            if (CompareEdge(tile.edges[3], tile.edges[1]))
            {
                this.up.Add(i);
            }

            // Check bottom direction
            if (CompareEdge(tile.edges[0], tile.edges[2]))
            {
                this.up.Add(i);
            }

            // Check up direction
            if (CompareEdge(tile.edges[1], tile.edges[3]))
            {
                this.up.Add(i);
            }
        }
    }
}
