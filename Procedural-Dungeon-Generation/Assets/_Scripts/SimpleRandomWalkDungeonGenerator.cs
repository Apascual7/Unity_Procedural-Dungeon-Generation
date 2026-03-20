using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProceduralDungeonGeneration
{
    public class SimpleRandomWalkDungeonGenerator : MonoBehaviour
    {
        [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
        [SerializeField] private int _iterations = 10;
        [SerializeField] private TilemapVisualizer _tilemapVisualizer;

        public int walkLength = 10;
        public bool startRandomlyEachIteration = true;

        public void RunProceduralGeneration()
        {
            HashSet<Vector2Int> floorPositions = RunRandomWalk();
            _tilemapVisualizer.Clear();
            _tilemapVisualizer.PaintFloorTiles(floorPositions);
        }

        protected HashSet<Vector2Int> RunRandomWalk()
        {
            var currentPosition = startPosition;
            HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

            for (int i = 0; i < _iterations; i++)
            {
                var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, walkLength);
                floorPositions.UnionWith(path);

                if (startRandomlyEachIteration)
                {
                    currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
                }
            }

            return floorPositions;
        }
    }
}