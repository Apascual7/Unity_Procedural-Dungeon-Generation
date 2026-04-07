using System.Collections.Generic;
using ProceduralDungeonGeneration;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SemiProceduralGeneration : AbstractDungeonGenerator
{
    [SerializeField] private Vector2Int _startingPosition;
    [SerializeField] private RoomData _startingRoom;
    [SerializeField] private RoomData[] _allRooms;
    [SerializeField] private int _maxRooms = 10;

    [SerializeField] private Tilemap _floorTilemap;
    [SerializeField] private Tilemap _wallTilemap;
    [SerializeField] private Tilemap _passableWallTilemap;

    private List<RoomData> _roomsWithTopDoor = new List<RoomData>();
    private List<RoomData> _roomsWithBottomDoor = new List<RoomData>();
    private List<RoomData> _roomsWithLeftDoor = new List<RoomData>();
    private List<RoomData> _roomsWithRightDoor = new List<RoomData>();

    private HashSet<RoomNode> _rooms = new HashSet<RoomNode>();

    private void Start()
    {
        if (_allRooms == null || _allRooms.Length == 0)
        {
            Debug.LogError("SEMI-PROCEDURAL GENERATION: No rooms assigned to the generator.");
            return;
        }
    }

    protected override void RunProceduralGeneration()
    {
        _roomsWithTopDoor = new List<RoomData>();
        _roomsWithBottomDoor = new List<RoomData>();
        _roomsWithLeftDoor = new List<RoomData>();
        _roomsWithRightDoor = new List<RoomData>();

        _rooms = new HashSet<RoomNode>();

        SetRoomsDoors();
        RunGeneration();
        DrawRooms();

        Debug.Log("SEMI-PROCEDURAL GENERATION: Dungeon generated with " + _rooms.Count + " rooms.");
    }

    private void SetRoomsDoors()
    {
        foreach (var room in _allRooms)
        {
            foreach (var door in room.doors)
            {
                switch (door.direction)
                {
                    case Direction.Top:
                        _roomsWithTopDoor.Add(room);
                        break;

                    case Direction.Bottom:
                        _roomsWithBottomDoor.Add(room);
                        break;

                    case Direction.Left:
                        _roomsWithLeftDoor.Add(room);
                        break;

                    case Direction.Right:
                        _roomsWithRightDoor.Add(room);
                        break;

                    default:
                        continue;
                }
            }
        }
    }

    private void RunGeneration()
    {
        _rooms.Add(new RoomNode(_startingRoom, _startingPosition));

        int roomCount = 1;
        while (roomCount < _maxRooms)
        {
            HashSet<RoomNode> newRooms = CreateNeighbourRooms(_startingRoom);
            roomCount += newRooms.Count;

            _rooms.UnionWith(newRooms);
        }
    }

    private HashSet<RoomNode> CreateNeighbourRooms(RoomData room)
    {
        HashSet<RoomNode> neighbours = new HashSet<RoomNode>();

        foreach (var door in room.doors)
        {
            Direction opositeDirection = DirectionHelper.GetOposite(door.direction);
            List<RoomData> possibleRooms = GetRoomsWithDoor(opositeDirection);

            RoomData newRoom = possibleRooms[Random.Range(0, possibleRooms.Count)];
            Vector2Int newPosition = DirectionHelper.GetDirectionVector(door.direction) * room.size + _startingPosition;
            neighbours.Add(new RoomNode(newRoom, newPosition));
        }

        return neighbours;
    }

    private void DrawRooms()
    {
        ClearTilemaps();

        foreach (var roomNode in _rooms)
        {
            PaintRoom(roomNode.room, roomNode.position);
        }
    }

    private void ClearTilemaps()
    {
        _floorTilemap.ClearAllTiles();
        _wallTilemap.ClearAllTiles();
        _passableWallTilemap.ClearAllTiles();
    }

    private void PaintRoom(RoomData room, Vector2Int roomPosition)
    {
        foreach (var tile in room.floorTiles)
        {
            PaintTile(tile, _floorTilemap, roomPosition);
        }

        foreach (var tile in room.wallTiles)
        {
            PaintTile(tile, _wallTilemap, roomPosition);
        }

        foreach (var tile in room.passableWallTiles)
        {
            PaintTile(tile, _passableWallTilemap, roomPosition);
        }
    }

    private void PaintTile(TileData tile, Tilemap tilemap, Vector2Int roomPosition)
    {
        var tilePosition = _floorTilemap.WorldToCell((Vector3Int)(tile.position + roomPosition));
        tilemap.SetTile(tilePosition, tile.tile);
    }

    private List<RoomData> GetRoomsWithDoor(Direction direction)
    {
        switch (direction)
        {
            case Direction.Top:
                return _roomsWithTopDoor;

            case Direction.Bottom:
                return _roomsWithBottomDoor;

            case Direction.Left:
                return _roomsWithLeftDoor;

            case Direction.Right:
                return _roomsWithRightDoor;

            default:
                Debug.LogError("SEMI-PROCEDURAL GENERATION: Invalid direction provided.");
                return new List<RoomData>();
        }
    }
}

public struct RoomNode
{
    public RoomData room;
    public Vector2Int position;

    public RoomNode(RoomData room, Vector2Int position)
    {
        this.room = room;
        this.position = position;
    }
}