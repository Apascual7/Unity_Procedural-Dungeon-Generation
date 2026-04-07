using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Room", menuName = "Scriptable Objects/Room Data")]
public class RoomData : ScriptableObject
{
    public Vector2Int size;

    public List<TileData> floorTiles = new List<TileData>();
    public List<TileData> wallTiles = new List<TileData>();
    public List<TileData> passableWallTiles = new List<TileData>();

    public bool doorUp, doorDown, doorLeft, doorRight;

    public TileBase[] floorTilesArray;     // array pla: y * width + x
    public TileBase[] wallTilesArray;
    public DoorData[] doors;          // posició + direcció de cada porta
}

[System.Serializable]
public struct TileData
{
    public Vector2Int position;
    public TileBase tile;

    public TileData(Vector2Int position, TileBase tile)
    {
        this.position = position;
        this.tile = tile;
    }
}

[System.Serializable]
public class DoorData
{
    public Vector2Int tilePosition;
    public Direction direction;
}

public enum Direction { North, South, East, West }
