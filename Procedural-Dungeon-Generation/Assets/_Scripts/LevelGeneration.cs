using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProceduralDungeonGeneration
{
    public class LevelGeneration : SimpleRandomWalkDungeonGenerator
    {
        [Header("World Settings")]
        [SerializeField] private Vector2Int worldSize = new Vector2Int(4, 4);
        [SerializeField] private int numberOfRooms = 20;

        [Header("Room Settings")]
        [SerializeField] private Vector2Int roomCellSize = new Vector2Int(20, 20);
        [SerializeField] private Vector2Int roomSize = new Vector2Int(16, 16);
        [SerializeField] private GameObject normalRoomPrefab;
        [SerializeField] private GameObject startRoomPrefab;

        private Room[,] rooms;
        private List<Vector2Int> takenPositions = new List<Vector2Int>();
        private List<GameObject> spawnedRooms = new List<GameObject>();
        private int gridSizeX, gridSizeY;

        protected override void RunProceduralGeneration()
        {
            foreach (var go in spawnedRooms)
                if (go != null) DestroyImmediate(go);
            spawnedRooms.Clear();
            takenPositions.Clear();

            numberOfRooms = Mathf.Min(numberOfRooms,
                (worldSize.x * 2) * (worldSize.y * 2));

            gridSizeX = worldSize.x;
            gridSizeY = worldSize.y;

            CreateRooms();
            SetRoomDoors();
            DrawMap();
        }

        private void CreateRooms()
        {
            rooms = new Room[gridSizeX * 2, gridSizeY * 2];
            rooms[gridSizeX, gridSizeY] = new Room(Vector2Int.zero, 1);
            takenPositions.Add(Vector2Int.zero);

            float randomCompareStart = 0.2f, randomCompareEnd = 0.01f;

            for (int i = 0; i < numberOfRooms - 1; i++)
            {
                float randomPerc = i / ((float)numberOfRooms - 1);
                float randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);

                Vector2Int checkPos = NewPosition();

                if (NumberOfNeighbors(checkPos, takenPositions) > 1
                    && Random.value > randomCompare)
                {
                    int iterations = 0;
                    do
                    {
                        checkPos = SelectiveNewPosition();
                        iterations++;
                    }
                    while (NumberOfNeighbors(checkPos, takenPositions) > 1
                           && iterations < 100);
                }

                rooms[checkPos.x + gridSizeX, checkPos.y + gridSizeY] =
                    new Room(checkPos, 0);
                takenPositions.Add(checkPos);
            }
        }

        private Vector2Int NewPosition()
        {
            Vector2Int checkingPos;
            do
            {
                int index = Random.Range(0, takenPositions.Count);
                int x = takenPositions[index].x;
                int y = takenPositions[index].y;

                if (Random.value < 0.5f)
                    y += Random.value < 0.5f ? 1 : -1;
                else
                    x += Random.value < 0.5f ? 1 : -1;

                checkingPos = new Vector2Int(x, y);
            }
            while (takenPositions.Contains(checkingPos)
                   || checkingPos.x >= gridSizeX
                   || checkingPos.x < -gridSizeX
                   || checkingPos.y >= gridSizeY
                   || checkingPos.y < -gridSizeY);

            return checkingPos;
        }

        private Vector2Int SelectiveNewPosition()
        {
            Vector2Int checkPos;
            do
            {
                int inc = 0, index;
                do
                {
                    index = Random.Range(0, takenPositions.Count);
                    inc++;
                }
                while (NumberOfNeighbors(takenPositions[index], takenPositions) > 1
                       && inc < 100);

                int x = takenPositions[index].x;
                int y = takenPositions[index].y;

                if (Random.value < 0.5f)
                    y += Random.value < 0.5f ? 1 : -1;
                else
                    x += Random.value < 0.5f ? 1 : -1;

                checkPos = new Vector2Int(x, y);
            }
            while (takenPositions.Contains(checkPos)
                   || checkPos.x >= gridSizeX
                   || checkPos.x < -gridSizeX
                   || checkPos.y >= gridSizeY
                   || checkPos.y < -gridSizeY);

            return checkPos;
        }

        private int NumberOfNeighbors(Vector2Int pos, List<Vector2Int> taken)
        {
            int count = 0;
            if (taken.Contains(pos + Vector2Int.right)) count++;
            if (taken.Contains(pos + Vector2Int.left)) count++;
            if (taken.Contains(pos + Vector2Int.up)) count++;
            if (taken.Contains(pos + Vector2Int.down)) count++;
            return count;
        }

        private void SetRoomDoors()
        {
            for (int x = 0; x < gridSizeX * 2; x++)
            {
                for (int y = 0; y < gridSizeY * 2; y++)
                {
                    if (rooms[x, y] == null) continue;

                    rooms[x, y].doorBottom = y - 1 >= 0 && rooms[x, y - 1] != null;
                    rooms[x, y].doorTop = y + 1 < gridSizeY * 2 && rooms[x, y + 1] != null;
                    rooms[x, y].doorLeft = x - 1 >= 0 && rooms[x - 1, y] != null;
                    rooms[x, y].doorRight = x + 1 < gridSizeX * 2 && rooms[x + 1, y] != null;
                }
            }
        }

        private void DrawMap()
        {
            HashSet<Vector2Int> allFloorTiles = new HashSet<Vector2Int>();
            HashSet<Vector2Int> corridorTiles = new HashSet<Vector2Int>();

            for (int x = 0; x < gridSizeX * 2; x++)
            {
                for (int y = 0; y < gridSizeY * 2; y++)
                {
                    if (rooms[x, y] == null) continue;

                    Vector2Int gridPos = rooms[x, y].position;
                    Vector3 worldPos = new Vector3(
                        gridPos.x * roomCellSize.x,
                        gridPos.y * roomCellSize.y, 0f);

                    GameObject prefab = rooms[x, y].type == 1
                        ? startRoomPrefab : normalRoomPrefab;

                    if (prefab == null) continue;

                    GameObject instance = Instantiate(prefab, worldPos,
                        Quaternion.identity, transform);
                    spawnedRooms.Add(instance);

                    var controller = instance.GetComponent<RoomController>();
                    if (controller != null)
                    {
                        var floorTiles = controller.GetFloorPositions(
                            new Vector2Int(
                                gridPos.x * roomCellSize.x,
                                gridPos.y * roomCellSize.y));
                        allFloorTiles.UnionWith(floorTiles);

                        controller.SetActiveDoors(
                            rooms[x, y].doorTop,
                            rooms[x, y].doorBottom,
                            rooms[x, y].doorLeft,
                            rooms[x, y].doorRight);
                    }

                    if (rooms[x, y].doorRight)
                        corridorTiles.UnionWith(
                            BuildCorridor(gridPos, gridPos + Vector2Int.right));
                    if (rooms[x, y].doorTop)
                        corridorTiles.UnionWith(
                            BuildCorridor(gridPos, gridPos + Vector2Int.up));
                }
            }

            allFloorTiles.UnionWith(corridorTiles);
            tilemapVisualizer.PaintFloorTiles(allFloorTiles);
            WallGenerator.CreateWalls(allFloorTiles, tilemapVisualizer);
        }

        private HashSet<Vector2Int> BuildCorridor(Vector2Int from, Vector2Int to)
        {
            HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();

            Vector2Int direction = to - from;
            Vector2Int start = GetDoorTilePosition(from, direction);
            Vector2Int end = GetDoorTilePosition(to, -direction);

            var pos = start;

            while (pos != end)
            {
                corridor.Add(pos);
                if (direction.x != 0)
                {
                    corridor.Add(pos + Vector2Int.up);
                    corridor.Add(pos + Vector2Int.down);
                }
                else
                {
                    corridor.Add(pos + Vector2Int.right);
                    corridor.Add(pos + Vector2Int.left);
                }

                pos.x += (pos.x != end.x) ? (int)Mathf.Sign(end.x - pos.x) : 0;
                pos.y += (pos.y != end.y) ? (int)Mathf.Sign(end.y - pos.y) : 0;
            }

            corridor.Add(end);
            if (direction.x != 0)
            {
                corridor.Add(end + Vector2Int.up);
                corridor.Add(end + Vector2Int.down);
            }
            else
            {
                corridor.Add(end + Vector2Int.right);
                corridor.Add(end + Vector2Int.left);
            }

            return corridor;
        }

        private Vector2Int GetDoorTilePosition(Vector2Int gridPos, Vector2Int direction)
        {
            int cx = gridPos.x * roomCellSize.x + roomCellSize.x / 2;
            int cy = gridPos.y * roomCellSize.y + roomCellSize.y / 2;

            int halfX = roomSize.x / 2;
            int halfY = roomSize.y / 2;

            if (direction == Vector2Int.right) return new Vector2Int(cx + halfX, cy);
            if (direction == Vector2Int.left) return new Vector2Int(cx - halfX, cy);
            if (direction == Vector2Int.up) return new Vector2Int(cx, cy + halfY);
            if (direction == Vector2Int.down) return new Vector2Int(cx, cy - halfY);

            return new Vector2Int(cx, cy);
        }
    }
}