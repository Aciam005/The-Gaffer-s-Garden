using System.Collections.Generic;
using UnityEngine;

namespace Game.Mechanics.Building
{
    // ScriptableObject for building properties
    [CreateAssetMenu(fileName = "BuildingSO", menuName = "Buildings/BuildingSO")]
    public class BuildingSO : ScriptableObject
    {
        [Header("Building Properties")]
        public string buildingName;
        public GameObject buildingPrefab;
        public int width; // Width of the building in grid cells
        public int length; // Length of the building in grid cells
        public int cost; // Cost in Glimmers to build this building







        /// <summary>
        /// Returns a list of occupied cells based on the building's width, length and rotation.
        /// </summary>
        public List<Vector2Int> GetOccupiedCells(Vector2Int startCell, BuildingRotation rotation)
        {
            // Returns a list of occupied cells based on the building's width, length and rotation
            List<Vector2Int> occupiedCells = new List<Vector2Int>();
            if (rotation == BuildingRotation.Up || rotation == BuildingRotation.Down)
            {
                for (int x = 0; x < length; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        occupiedCells.Add(new Vector2Int(startCell.x + x, startCell.y + y));
                    }
                }
            }
            else
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < length; y++)
                    {
                        occupiedCells.Add(new Vector2Int(startCell.x + x, startCell.y + y));
                    }
                }
            }
            return occupiedCells;
        }

        public BuildingRotation GetNextRotation(BuildingRotation currentRotation)
        {
            // Returns the next rotation enum based on the current rotation
            switch (currentRotation)
            {
                case BuildingRotation.Down:
                    return BuildingRotation.Left;
                case BuildingRotation.Left:
                    return BuildingRotation.Up;
                case BuildingRotation.Up:
                    return BuildingRotation.Right;
                case BuildingRotation.Right:
                    return BuildingRotation.Down;
                default:
                    return BuildingRotation.Down; // Default to Down if none match
            }
        }

        public Quaternion GetBuildingRotation(BuildingRotation rotation)
        {
            // Returns the rotation of the building based on the specified rotation enum
            switch (rotation)
            {
                case BuildingRotation.Up:
                    return Quaternion.Euler(0, 180, 0);
                case BuildingRotation.Down:
                    return Quaternion.Euler(0, 0, 0);
                case BuildingRotation.Left:
                    return Quaternion.Euler(0, -90, 0);
                case BuildingRotation.Right:
                    return Quaternion.Euler(0, 90, 0);
                default:
                    return Quaternion.Euler(0, 0, 0);
            }
        }
        
        public Vector2Int GetRotationOffset(BuildingRotation rotation)
        {
            // Returns the offset based on the rotation
            switch (rotation)
            {
                case BuildingRotation.Down:
                    return new Vector2Int(0, 0);
                case BuildingRotation.Up:
                    return new Vector2Int(length, width);
                case BuildingRotation.Left:
                    return new Vector2Int(width, 0);
                case BuildingRotation.Right:
                    return new Vector2Int(0, length);
                default:
                    return new Vector2Int(0, 0);
            }
        }

    }

    public enum BuildingRotation
    {
        Up,
        Down,
        Left,
        Right
    }
}
