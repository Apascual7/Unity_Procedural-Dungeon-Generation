using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomTest : MonoBehaviour
{
    [SerializeField] RoomData room;
    [SerializeField] Tilemap floorTilemap;
    [SerializeField] Tilemap wallTilemap;

    void Start()
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