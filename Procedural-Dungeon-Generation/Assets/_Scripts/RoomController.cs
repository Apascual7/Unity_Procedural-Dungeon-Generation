using System.Collections.Generic;
using UnityEngine;

namespace ProceduralDungeonGeneration
{
    public class RoomController : MonoBehaviour
    {
        [Header("Mida de la sala en tiles")]
        public Vector2Int roomSize = new Vector2Int(18, 18);

        [Header("Portes")]
        public Transform doorTop, doorBottom, doorLeft, doorRight;

        public HashSet<Vector2Int> GetFloorPositions(Vector2Int worldOffset)
        {
            var positions = new HashSet<Vector2Int>();

            int halfX = roomSize.x / 2;
            int halfY = roomSize.y / 2;

            for (int x = -halfX; x < halfX; x++)
            {
                for (int y = -halfY; y < halfY; y++)
                {
                    positions.Add(worldOffset + new Vector2Int(x, y));
                }
            }

            return positions;
        }

        public void SetActiveDoors(bool top, bool bottom, bool left, bool right)
        {
            if (doorTop) doorTop.gameObject.SetActive(top);
            if (doorBottom) doorBottom.gameObject.SetActive(bottom);
            if (doorLeft) doorLeft.gameObject.SetActive(left);
            if (doorRight) doorRight.gameObject.SetActive(right);
        }
    }
}