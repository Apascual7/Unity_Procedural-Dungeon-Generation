#if UNITY_EDITOR
using Codice.Client.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomExporterWindow : EditorWindow
{
    Tilemap floorTilemap;
    Tilemap wallTilemap;
    string roomName = "NewRoom";
    RoomData room;

    [MenuItem("Dungeon/Room Exporter")]
    public static void ShowWindow()
    {
        GetWindow<RoomExporterWindow>("Room Exporter");
    }

    void OnGUI()
    {
        GUILayout.Label("Room Exporter", EditorStyles.boldLabel);

        floorTilemap = (Tilemap)EditorGUILayout.ObjectField(
            "Floor Tilemap", floorTilemap, typeof(Tilemap), true);

        wallTilemap = (Tilemap)EditorGUILayout.ObjectField(
            "Wall Tilemap", wallTilemap, typeof(Tilemap), true);

        roomName = EditorGUILayout.TextField("Room Name", roomName);

        if (GUILayout.Button("Export Room"))
        {
            ExportRoom();
        }

        room = (RoomData)EditorGUILayout.ObjectField("Room To Show", room, typeof(RoomData), true);

        if (GUILayout.Button("Show Room"))
        {
            ShowRoom();
        }
    }

    void ExportRoom()
    {
        if (floorTilemap == null || wallTilemap == null)
        {
            Debug.LogError("Assigna els dos Tilemaps!");
            return;
        }

        // Comprimeix els bounds a les tiles realment ocupades
        floorTilemap.CompressBounds();
        wallTilemap.CompressBounds();

        BoundsInt bounds = floorTilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;

        var room = ScriptableObject.CreateInstance<RoomData>();
        room.size = new Vector2Int(width, height);
        room.floorTilesArray = new TileBase[width * height];
        room.wallTilesArray = new TileBase[width * height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                int idx = y * width + x;
                var pos = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);

                room.floorTilesArray[idx] = floorTilemap.GetTile(pos);
                room.wallTilesArray[idx] = wallTilemap.GetTile(pos);
            }


        foreach (var position in bounds.allPositionsWithin)
        {
            room.floorTiles.Add(new TileData(new Vector2Int(position.x, position.y), floorTilemap.GetTile(position)));
            Debug.Log(position);
        }

        foreach (var position in wallTilemap.cellBounds.allPositionsWithin)
        {
            room.wallTiles.Add(new TileData(new Vector2Int(position.x, position.y), wallTilemap.GetTile(position)));
            Debug.Log(position);
        }



        string path = $"Assets/_Scripts/aaa/{roomName}.asset";
        AssetDatabase.CreateAsset(room, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Sala exportada a {path}");
    }

    void ShowRoom()
    {
        foreach (var tile in room.floorTiles)
        {
            var tilePosition = floorTilemap.WorldToCell((Vector3Int)tile.position);
            floorTilemap.SetTile(tilePosition, tile.tile);
        }
        foreach (var tile in room.wallTiles)
        {
            var tilePosition = wallTilemap.WorldToCell((Vector3Int)tile.position);
            wallTilemap.SetTile(tilePosition, tile.tile);
        }
    }
}
#endif