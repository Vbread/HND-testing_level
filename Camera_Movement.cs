using UnityEngine;
using UnityEngine.InputSystem;

public class Camera_Movement : MonoBehaviour
{
    public float speed = 5.0f;
    public float mouseSensitivity = 2.0f;

    private Vector2 movementInput, mousePositionInput;

    public UI_In_Game UIInGame;

    private Grid_2D grid;

    public BuildingRegisty buildingRegisty;
    public int selectedBuildingIndex = 0;  // index of the building to place

    public int currentBuildingIndex = 0;

    private Input_system_Manager inputActions;

    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Awake()
    {

        grid = FindFirstObjectByType<Grid_2D>();

        LoadInputs();
    }

    private void LoadInputs()
    {

        inputActions = new Input_system_Manager();

        inputActions.IsometricView.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.IsometricView.Movement.canceled += ctx => movementInput = Vector2.zero;

        inputActions.IsometricView.Mouse_Position.performed += ctx => mousePositionInput = ctx.ReadValue<Vector2>();

        inputActions.IsometricView.Click_left.performed += ctx => LeftClick();
        inputActions.IsometricView.Hold_right.performed += ctx => RightHold();

        inputActions.IsometricView.Cycle_Up.performed += ctx => CycleBuildingUp();
        inputActions.IsometricView.Cycle_Down.performed += ctx => CycleBuildingDown();

        inputActions.IsometricView.Turn_Grid_Off.performed += ctx => GridToggle();


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grid.IsIsomentric = true;
    }

    private void GridToggle()
    {
        
        if (grid.IsGridVisible)
        {
            grid.IsGridVisible = false;
        }
        else
        {
            grid.IsGridVisible = true;
        }

    }

    private void CycleBuildingUp()
    {
        
        if (currentBuildingIndex >= buildingRegisty.BuildableObjectsRegistry.Length - 1)
        {
            currentBuildingIndex = 0;
            selectedBuildingIndex = currentBuildingIndex;
            UIInGame.CycleUpHotbar();
        }
        else
        {
            currentBuildingIndex++;
            selectedBuildingIndex = currentBuildingIndex;
            UIInGame.CycleUpHotbar();
        }
    }

    private void CycleBuildingDown()
    {
        if (currentBuildingIndex <= 0)
        {

            currentBuildingIndex = buildingRegisty.BuildableObjectsRegistry.Length - 1;
            selectedBuildingIndex = currentBuildingIndex;
            UIInGame.CycleDownHotbar();
        }
        else
        {          
            currentBuildingIndex--;
            selectedBuildingIndex = currentBuildingIndex;
            UIInGame.CycleDownHotbar();
        }
    }

    private void LeftClick()
    {

        if (buildingRegisty == null)
        {
            Debug.LogWarning("BuildingRegistry not assigned");
            return;
        }

        if (selectedBuildingIndex < 0 || selectedBuildingIndex >= buildingRegisty.BuildableObjectsRegistry.Length)
        {
            Debug.LogWarning("Invalid building index selected");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(mousePositionInput);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // convert hit point to grids local space
            Vector3 localPoint = grid.transform.InverseTransformPoint(hit.point);

            int gridX = Mathf.FloorToInt(localPoint.x / grid.CellSize);
            int gridY = Mathf.FloorToInt(localPoint.z / grid.CellSize);

            if (gridX < 0 || gridX >= grid.HorizontalCells || gridY < 0 || gridY >= grid.VerticalCells)
            {
                Debug.Log("Clicked outside grid");
                return;
            }

            var building = buildingRegisty.BuildableObjectsRegistry[selectedBuildingIndex];
            grid.PlaceObject(building, new Vector2Int(gridX, gridY));
        }
    }

    private void RightHold()
    {
        // implement right-click hold functionality here
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

    }

    private void Movement()
    {

        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y) * speed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }
}
