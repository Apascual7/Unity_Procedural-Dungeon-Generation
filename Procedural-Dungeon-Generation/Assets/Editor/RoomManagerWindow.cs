using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManagerWindow : EditorWindow
{
    [SerializeField] private Tilemap _floorTilemap;
    [SerializeField] private Tilemap _wallTilemap;
    [SerializeField] private Tilemap _passableWallTilemap;

    [SerializeField] private string _newRoomName = "NewRoom";

    [SerializeField] private RoomData _roomToShow;

    [MenuItem("Dungeon/Room Manager")]
    public static void ShowWindow()
    {
        GetWindow<RoomManagerWindow>("Room Manager");
    }

    void OnGUI()
    {
        GUILayout.Label("Room Exporter", EditorStyles.boldLabel);

        _floorTilemap = (Tilemap)EditorGUILayout.ObjectField("Floor Tilemap", _floorTilemap, typeof(Tilemap), true);
        _wallTilemap = (Tilemap)EditorGUILayout.ObjectField("Wall Tilemap", _wallTilemap, typeof(Tilemap), true);
        _passableWallTilemap = (Tilemap)EditorGUILayout.ObjectField("Passable Wall Tilemap", _passableWallTilemap, typeof(Tilemap), true);

        _newRoomName = EditorGUILayout.TextField("Room Name", _newRoomName);

        if (GUILayout.Button("Export Room"))
        {
            ExportRoom();
        }

        EditorGUILayout.Space(20);

        GUILayout.Label("Room Visualizer", EditorStyles.boldLabel);

        _roomToShow = (RoomData)EditorGUILayout.ObjectField("Room To Show", _roomToShow, typeof(RoomData), true);

        if (GUILayout.Button("Show Room"))
        {
            ShowRoom();
        }
    }

    void ExportRoom()
    {
        if (_floorTilemap == null || _wallTilemap == null || _passableWallTilemap == null)
        {
            Debug.LogError("ROOM EXPORTER: One or more of the Tilemaps are Null");
            return;
        }

        _floorTilemap.CompressBounds();
        _wallTilemap.CompressBounds();
        _passableWallTilemap.CompressBounds();

        BoundsInt floorBounds = _floorTilemap.cellBounds;
        BoundsInt wallBounds = _wallTilemap.cellBounds;
        BoundsInt passableWallBounds = _passableWallTilemap.cellBounds;

        int width = wallBounds.size.x;
        int height = wallBounds.size.y;

        var room = CreateInstance<RoomData>();
        room.size = new Vector2Int(width, height);
        room.floorTiles = FillTileData(floorBounds, _floorTilemap);
        room.passableWallTiles = FillTileData(passableWallBounds, _passableWallTilemap);

        List<Direction> doorsDirections = new List<Direction>();
        foreach (var position in wallBounds.allPositionsWithin)
        {
            room.wallTiles.Add(new TileData(new Vector2Int(position.x, position.y), _wallTilemap.GetTile(position)));

            if (_wallTilemap.GetTile(position) == null)
            {
                if (position.y == wallBounds.yMax - 1 && !doorsDirections.Contains(Direction.Top))
                {
                    doorsDirections.Add(Direction.Top);
                }

                if (position.y == wallBounds.yMin && !doorsDirections.Contains(Direction.Bottom))
                {
                    doorsDirections.Add(Direction.Bottom);
                }

                if (position.x == wallBounds.xMin && !doorsDirections.Contains(Direction.Left))
                {
                    doorsDirections.Add(Direction.Left);
                }

                if (position.x == wallBounds.xMax - 1 && !doorsDirections.Contains(Direction.Right))
                {
                    doorsDirections.Add(Direction.Right);
                }
            }
        }
        foreach (var direction in doorsDirections)
        {
            room.doors.Add(new DoorData(direction, room));
        }

        string path = $"Assets/_Scripts/ScriptableObjects/{_newRoomName}.asset";
        AssetDatabase.CreateAsset(room, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"ROOM EXPORTER: Room Exported to: {path}");
    }

    private List<TileData> FillTileData(BoundsInt bounds, Tilemap tilemap)
    {
        List<TileData> tiles = new List<TileData>();
        foreach (var position in bounds.allPositionsWithin)
        {
            tiles.Add(new TileData(new Vector2Int(position.x, position.y), tilemap.GetTile(position)));
        }
        return tiles;
    }

    void ShowRoom()
    {
        _floorTilemap.ClearAllTiles();
        _wallTilemap.ClearAllTiles();
        _passableWallTilemap.ClearAllTiles();

        foreach (var tile in _roomToShow.floorTiles)
        {
            var tilePosition = _floorTilemap.WorldToCell((Vector3Int)tile.position);
            _floorTilemap.SetTile(tilePosition, tile.tile);
        }
        foreach (var tile in _roomToShow.wallTiles)
        {
            var tilePosition = _wallTilemap.WorldToCell((Vector3Int)tile.position);
            _wallTilemap.SetTile(tilePosition, tile.tile);
        }
        foreach (var tile in _roomToShow.passableWallTiles)
        {
            var tilePosition = _wallTilemap.WorldToCell((Vector3Int)tile.position);
            _passableWallTilemap.SetTile(tilePosition, tile.tile);
        }

        Debug.Log($"ROOM EXPORTER: Room '{_roomToShow.name}' displayed. Doors {_roomToShow.doors.Count}");
    }
}
