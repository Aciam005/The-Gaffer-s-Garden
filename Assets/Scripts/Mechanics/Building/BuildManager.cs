using System.Collections.Generic;
using UnityEngine;

namespace Game.Mechanics.Building
{
    // Singleton manager for handling build mode toggling
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager instance { get; private set; }

        public bool IsBuildMode { get; private set; } = false;
        private static Grid<int> grid;

        [Header("Grid Settings")]
        public int gridWidth = 10;
        public int gridHeight = 10;
        public float cellSize = 1f;
        public Vector3 gridOrigin = Vector3.zero;

        [Header("BUILDING REFERENCES")]

        //TODO:Implement a proper way to switch between buildings
        public BuildingSO RabbitHutchSO;
        public BuildingSO CropTileSO;

        private GameObject currentBuildingPrefab;
        private BuildingSO currentBuildingSO;
        private BuildingRotation currentRotation = BuildingRotation.Down;

        [Header("SCRIPT REFERENCES")]
        [SerializeField] private CurrencyManager currencyManager;

        private void Awake()
        {
            // Standard singleton pattern
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                gridOrigin = transform.position;

                // Instantiate grid of ints
                grid = new Grid<int>(
                    gridWidth,
                    gridHeight,
                    cellSize,
                    gridOrigin,
                    (g, x, y) => 0 // initialize all cells to 0
                );


            }
        }

        void Start()
        {

        }
        private void Update()
        {
            HandleBuildModeToggle();
            if (IsBuildMode && currentBuildingSO != null)
            {
                HandleBuildModeLogic();
            }
        }

        private void HandleBuildModeToggle()
        {
            // Toggle build mode when 'B' key is pressed
            if (Input.GetKeyDown(KeyCode.B))
            {
                IsBuildMode = !IsBuildMode;
            }
            else if(currentBuildingPrefab != null)
            {
                currentBuildingPrefab.SetActive(false);
                //TODO:Disable grid view.
            }
        }

        private void HandleBuildModeLogic()
        {
            // Handle building logic here
            currentBuildingPrefab.SetActive(true);

            // Get mouse position in world space
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 mouseWorldPosition;
            //TODO:Update Raycast to only hit the ground
            if (Physics.Raycast(ray, out RaycastHit hit, 999f))
            {
                mouseWorldPosition = hit.point;
            }
            else
            {
                // If raycast doesn't hit anything, use the camera's forward direction
                mouseWorldPosition = Camera.main.transform.position + Camera.main.transform.forward * 10f;
            }

            // Convert mouse position to grid coordinates
            int x, y;
            grid.GetXY(mouseWorldPosition, out x, out y);

            //Cheaty way to avoid out of grid placement.
            if (x < 0 )
            {
                x = 0;
            }

            if (y < 0)
            {
                y = 0;
            }


            // Rotate the building when 'R' key is pressed
            if (Input.GetKeyDown(KeyCode.R))
            {
                // Rotate the building when 'R' key is pressed
                currentRotation = currentBuildingSO.GetNextRotation(currentRotation);
                currentBuildingPrefab.transform.rotation = currentBuildingSO.GetBuildingRotation(currentRotation);
                Debug.Log($"Current Rotation: {currentRotation}");
            }

            // Move the current building prefab to the middle of the grid cell
            Vector3 worldPosition = grid.GetWorldPosition(x, y);
            Vector2Int positionOffset = currentBuildingSO.GetRotationOffset(currentRotation);
            worldPosition += new Vector3(positionOffset.x, 0, positionOffset.y) * cellSize;

            currentBuildingPrefab.transform.position = worldPosition;

            HandleBuildingPlacement(x, y, worldPosition);
        }

        private void HandleBuildingPlacement(int x, int y, Vector3 worldPosition)
        {
            // Handle building placement when left mouse button is clicked
            if (Input.GetMouseButtonDown(0) && currencyManager.CanAfford(currentBuildingSO.cost))
            {
                // Check if the cell is already occupied
                if (grid.GetGridObject(x, y) == 0)
                {
                    // Place the building

                    Vector2Int startCell = new Vector2Int(x, y);
                    List<Vector2Int> occupiedCells = currentBuildingSO.GetOccupiedCells(startCell, currentRotation);

                    foreach (Vector2Int cell in occupiedCells)
                    {
                        if (grid.GetGridObject(cell.x, cell.y) != 0)
                        {
                            Debug.LogWarning($"Cell {x}, {y} is already occupied!");
                            return; // Exit if any cell is occupied
                        }

                    }

                    foreach (Vector2Int cell in occupiedCells)
                    {
                        grid.SetGridObject(cell.x, cell.y, 1); // Set the cell to occupied (1)
                    }

                    Instantiate(currentBuildingPrefab, worldPosition, currentBuildingSO.GetBuildingRotation(currentRotation));
                    currencyManager.RemoveGlimmers(currentBuildingSO.cost);
                }
                else
                {
                    Debug.LogWarning($"Cell {x}, {y} is already occupied!");
                }
            }
        }

        public void SwitchToRabbitHutch()
        {
            if (currentBuildingPrefab != null)
            {
                Destroy(currentBuildingPrefab);
            }
            currentBuildingSO = RabbitHutchSO;
            currentBuildingPrefab = Instantiate(currentBuildingSO.buildingPrefab);
            currentBuildingPrefab.SetActive(false);
            currentRotation = BuildingRotation.Down;
        }

        public void SwitchToCropTile()
        {
            if (currentBuildingPrefab != null)
            {
                Destroy(currentBuildingPrefab);
            }
            currentBuildingSO = CropTileSO;
            currentBuildingPrefab = Instantiate(currentBuildingSO.buildingPrefab);
            currentBuildingPrefab.SetActive(false);
            currentRotation = BuildingRotation.Down;
        }
    }
}
