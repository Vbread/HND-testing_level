using UnityEngine;

public class GridCell
{
    public Vector2Int Position { get; private set; }
    public bool IsOccupied { get; set; }
    public GridCell(int x, int y)
    {
        Position = new Vector2Int(x, y);
        IsOccupied = false;
    }
}

public class Grid_2D : MonoBehaviour
{
    public BuildingRegisty buildingRegisty;

    [Header("Grid Settings")]
    public float CellSize = 1f;
    public int HorizontalCells = 10, VerticalCells = 10;
    public Color ColourUnoccupied = Color.gray;

    [Tooltip("Colour used for occupied cells when the game is running")]
    public Color ColourOccupied = Color.red;

    public GridCell[,] grid;

    public bool IsGridVisible = true;

    [HideInInspector]
    public bool IsIsomentric = false;

    void Start()
    {
        grid = CreateGrid();
    }

    public GridCell[,] CreateGrid()
    {
        GridCell[,] newGrid = new GridCell[HorizontalCells, VerticalCells];
        for (int x = 0; x < HorizontalCells; x++)
        {
            for (int y = 0; y < VerticalCells; y++)
            {
                newGrid[x, y] = new GridCell(x, y);
            }
        }
        return newGrid;
    }

    public void PlaceObject(BuildingRegisty.BuildableObjectsRegisty BuildingToBuild, Vector2Int position)
    {
        if (grid == null)
        {
            Debug.LogError("grid not initialized");
            return;
        }

        for (int x = 0; x < BuildingToBuild.Size.x; x++)
        {
            for (int y = 0; y < BuildingToBuild.Size.y; y++)
            {
                int gridX = position.x + x;
                int gridY = position.y + y;
                if (gridX >= HorizontalCells || gridY >= VerticalCells || grid[gridX, gridY].IsOccupied)
                {
                    Debug.Log("Cant place object here");
                    return;
                }
            }
        }

        for (int x = 0; x < BuildingToBuild.Size.x; x++)
        {
            for (int y = 0; y < BuildingToBuild.Size.y; y++)
            {
                int gridX = position.x + x;
                int gridY = position.y + y;
                grid[gridX, gridY].IsOccupied = true;
            }
        }

        Vector3 worldPosition = transform.position + new Vector3(
            (position.x + BuildingToBuild.Size.x / 2f) * CellSize,
            0,
            (position.y + BuildingToBuild.Size.y / 2f) * CellSize
        );
        Instantiate(BuildingToBuild.Prefab, worldPosition, Quaternion.identity);
    }
    // In Grid_2D.cs
    private void OnDrawGizmos()
    {
        if (!IsGridVisible) return;

        int collums, rows;
        if (Application.isPlaying && grid != null)
        {
            collums = grid.GetLength(0);
            rows = grid.GetLength(1);
        }
        else
        {
            collums = HorizontalCells;
            rows = VerticalCells;
        }

        for (int x = 0; x < collums; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // choose color based on occupancy (only shows in while playing)
                if (Application.isPlaying && grid != null)
                {
                    Gizmos.color = grid[x, y].IsOccupied ? ColourOccupied : ColourUnoccupied;
                }
                else
                {
                    Gizmos.color = ColourUnoccupied;
                }

                // offset by the grid's position
                Vector3 cellCenter = transform.position + new Vector3(
                    x * CellSize + CellSize * 0.5f,
                    0,
                    y * CellSize + CellSize * 0.5f
                );
                Gizmos.DrawWireCube(cellCenter, new Vector3(CellSize, 0.1f, CellSize));
            }
        }
    }
}