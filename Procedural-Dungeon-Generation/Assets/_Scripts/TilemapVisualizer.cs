using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ProceduralDungeonGeneration
{
    public class TilemapVisualizer : MonoBehaviour
    {
        [SerializeField] private Tilemap _floorTilemap, _wallTilemap;
        [SerializeField] private TileBase _floorTile, _wallTop, _wallSideRight, _wallSideLeft, _wallBottom, _wallFull,
            _wallInnerCornerDownRight, _wallInnerCornerDownLeft,
            _wallDiagonalCornerDownRight, _wallDiagonalCornerDownLeft, _wallDiagonalCornerUpRight, _wallDiagonalCornerUpLeft;

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

        internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
        {
            int typeAsInt = Convert.ToInt32(binaryType, 2);
            TileBase tile = null;

            if (WallTypesHelper.wallTop.Contains(typeAsInt))
            {
                tile = _wallTop;
            }
            else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
            {
                tile = _wallSideRight;
            }
            else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
            {
                tile = _wallSideLeft;
            }
            else if (WallTypesHelper.wallBottm.Contains(typeAsInt))
            {
                tile = _wallBottom;
            }
            else if (WallTypesHelper.wallFull.Contains(typeAsInt))
            {
                tile = _wallFull;
            }

            if (tile != null)
            {
                PaintSingleTile(position, _wallTilemap, tile);
            }
        }

        internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
        {
            int typeAsInt = Convert.ToInt32(binaryType, 2);
            TileBase tile = null;

            if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
            {
                tile = _wallInnerCornerDownRight;
            }
            else if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
            {
                tile = _wallInnerCornerDownLeft;
            }
            else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
            {
                tile = _wallDiagonalCornerDownRight;
            }
            else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
            {
                tile = _wallDiagonalCornerDownLeft;
            }
            else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
            {
                tile = _wallDiagonalCornerUpRight;
            }
            else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
            {
                tile = _wallDiagonalCornerUpLeft;
            }
            else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
            {
                tile = _wallFull;
            }
            else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
            {
                tile = _wallBottom;
            }

            if (tile != null)
            {
                PaintSingleTile(position, _wallTilemap, tile);
            }
        }
    }
}