using UnityEngine;

namespace ProceduralDungeonGeneration
{
    public class Room
    {
        public Vector2Int position;
        public int type;
        public bool doorTop, doorBottom, doorLeft, doorRight;

        public Room(Vector2Int position, int type)
        {
            this.position = position;
            this.type = type;
        }
    }
}