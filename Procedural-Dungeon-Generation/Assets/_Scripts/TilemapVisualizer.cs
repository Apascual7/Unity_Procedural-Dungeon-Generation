using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ProceduralDungeonGeneration
{
    public class TilemapVisualizer : MonoBehaviour
    {
        [SerializeField] private Tilemap _floorTilemap, _wallTilemap;
        [SerializeField] private TileBase _floorTile, _wallTop;

        public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
        {
            PaintTiles(floorPositions, _floorTilemap, _floorTile);
        }

        public void Clear()
        {
            _floorTilemap.ClearAllTiles();
            _wallTilemap.ClearAllTiles();
        }

        private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
        {
            foreach (var position in positions)
            {
                PaintSingleTile(position, tilemap, tile);
            }
        }

        private void PaintSingleTile(Vector2Int position, Tilemap tilemap, TileBase tile)
        {
            var tilePosition = tilemap.WorldToCell((Vector3Int)position);
            tilemap.SetTile(tilePosition, tile);
        }

        internal void PaintSingleBasicWall(Vector2Int position)
        {
            PaintSingleTile(position, _wallTilemap, _wallTop);
        }
    }
}