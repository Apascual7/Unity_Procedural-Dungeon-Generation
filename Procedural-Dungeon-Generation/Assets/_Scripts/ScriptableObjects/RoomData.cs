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
    public List<DoorData> doors = new List<DoorData>();

    public TileBase[] floorTilesArray;     // array pla: y * width + x
    public TileBase[] wallTilesArray;
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
public struct DoorData
{
    public Direction direction;
    public RoomData room;

    public DoorData(Direction direction, RoomData room)
    {
        this.direction = direction;
        this.room = room;
    }
}

public enum Direction { Top, Bottom, Left, Right }

public static class DirectionHelper
{
    public static Direction GetOposite(Direction direction)
    {
        switch (direction)
        {
            case Direction.Top:
                return Direction.Bottom;

            case Direction.Bottom:
                return Direction.Top;

            case Direction.Left:
                return Direction.Right;

            case Direction.Right:
                return Direction.Left;

            default:
                Debug.LogError("DIRECTION HELPER: Invalid direction provided.");
                return direction;
        }
    }

    public static Vector2Int GetDirectionVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Top:
                return Vector2Int.up;

            case Direction.Bottom:
                return Vector2Int.down;

            case Direction.Left:
                return Vector2Int.left;

            case Direction.Right:
                return Vector2Int.right;

            default:
                Debug.LogError("DIRECTION HELPER: Invalid direction provided.");
                return Vector2Int.zero;
        }
    }
}

public class RoomConnection
{
    public readonly DoorData[] connectedDoors;

    public RoomConnection(DoorData door1, DoorData door2)
    {
        connectedDoors = new[] { door1, door2 };
    }
}