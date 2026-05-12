using UnityEngine;
using UnityEngine.InputSystem;

public class MultiTool : MonoBehaviour
{

    public enum CurrentTool
    {
        Clean,
        Dig,
        Cutting,
        Salvage
    }

    public Input_system_Manager inputActions;


    private TilePollution pollutionManager;
    public LayerMask groundLayer;
    public Grid environmentGrid;

    [Header("debug, no touch")]
    public int currentActiveCleans = 0;
    public int maxConcurrentCleans = 3;

    [Header("Tool Settings")]
    public CurrentTool currentTool;
    public int EffectRange = 1;
    public int Efffectiveness = 1;

    [Header("Cleaning Tool Settings")]
    public float cleaningSpeed = 1f;


    public float passivePlantGrowthMultiplier = 1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputActions = new Input_system_Manager();

        currentTool = CurrentTool.Clean;

        inputActions.IsometricView.Tool_Switch.performed += ctx => SwitchTool();
        inputActions.IsometricView.Tool_Use.performed += ctx => UseTool();
    }

    // Update is called once per frame
    void Update()
    {
         

    }

    /*
    private void OnEnable()
    {
        inputActions.Enable();

    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
    */

    public void SwitchTool()
    {
        currentTool = (CurrentTool)(((int)currentTool + 1) % System.Enum.GetNames(typeof(CurrentTool)).Length);
    }

    public void UseTool()
    {
        switch (currentTool)
        {
            case CurrentTool.Clean:

                if (pollutionManager == null || environmentGrid == null) return;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
                {
                    Vector3 worldHitPosition = hit.point;
                    Vector3Int cellPosition = environmentGrid.WorldToCell(worldHitPosition);


                    bool successfullyStarted = pollutionManager.StartCleaningTile(cellPosition);
                    if (successfullyStarted)
                    {
                        currentActiveCleans++;
                        Debug.Log($"Started cleaning cell {cellPosition}. Slots used: {currentActiveCleans}/{maxConcurrentCleans}");
                    }
                }

                break;
            case CurrentTool.Dig:



                break;
            case CurrentTool.Cutting:



                break;
            case CurrentTool.Salvage:



                break;
        }
    }

    public void OnTileFinishedCleaning()
    {
        currentActiveCleans--;
        if (currentActiveCleans < 0) currentActiveCleans = 0;
    }

}
