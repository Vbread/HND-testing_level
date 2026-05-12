using Unity.VisualScripting;
using UnityEngine;

public class Prespawned : MonoBehaviour
{
    public bool isPrespawned = false;

    public Grid_2D grid;

    public BuildingRegisty buildingRegisty;

    [Tooltip("Index of the building to place from the BuildingRegisty. Ensure this index is valid and corresponds to a building in the registry.")]
    public int buildingIndex = 0;

    private void Awake()
    {
        if (isPrespawned)
        {
            Invoke(nameof(DelaySnap), 0.1f); 
        }   


    }

     private void DelaySnap()
    {
        grid = FindFirstObjectByType<Grid_2D>();

        Ray ray = new Ray(transform.position, Vector3.down * 2);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 worldPos = hit.point;
            int gridX = Mathf.FloorToInt(worldPos.x / grid.CellSize);
            int gridY = Mathf.FloorToInt(worldPos.z / grid.CellSize);
            var building = buildingRegisty.BuildableObjectsRegistry[buildingIndex];
            grid.PlaceObject(building, new Vector2Int(gridX, gridY));
        }

        Destroy(this.gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * 2);

    }
}
