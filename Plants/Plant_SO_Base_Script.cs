using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Plant_SO_Base_Script", menuName = "Scriptable Objects/Plant_SO_Base_Script")]
public class Plant_SO_Base_Script : ScriptableObject
{

    [Header("Display Info")] // These fields are for display purposes only, and do not affect the growth logic. For designers to easily identify and organize stages in the editor.
    public string StageName;
    [TextArea(3, 10)] 
    public string Description;
    public Image SpriteForUI;

    [Header("Growth Logic")]
    public GameObject ModelPrefab;
    public float GrowthRate = 10f;
    public float TimeTillNextStage = 10f;
    public bool IsFinalStage = false;

    [Header("Growth Requirements")]
    [Description("0 = nothing, 4 = max need.")]
    public int WaterRequirement = 1; 
    public int SoilQualityRequirement = 1; 
    public int SunlightRequirement = 1;

    [Tooltip("Drag Next Plant stage here to link them.")]
    public Plant_SO_Base_Script NextStage;

    [Header("Fully grown effects")]
    public float PositivePollutingEffect = 0;
    public int AmountOfItemsDropped = 0;
    public float DroptRate = 0f; 
    public int AmountCanBeHarvested = 0;
    [Tooltip("If the plant drops or produces an item, assign the ItemID here. This ID should correspond to the item in used in the inventory system.")]
    public Item_SO Item;
}
