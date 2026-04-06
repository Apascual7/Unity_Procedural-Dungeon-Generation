using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomRenderer : MonoBehaviour
{
    [SerializeField] Tilemap floorTilemap;
    [SerializeField] Tilemap wallTilemap;

    public void LoadRoom(RoomData room)
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        for (int y = 0; y < room.size.y; y++)
            for (int x = 0; x < room.size.x; x++)
            {
                int idx = y * room.size.x + x;
                var pos = new Vector3Int(x, y, 0);

                floorTilemap.SetTile(pos, room.floorTilesArray[idx]);
                wallTilemap.SetTile(pos, room.wallTilesArray[idx]);
            }
    }
}