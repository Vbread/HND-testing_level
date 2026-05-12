using UnityEngine;

[System.Serializable]
public struct GrowthStage
{
    [Header("Plant Name")]
    public string PlantName; // purely for debugging purposes and front end, not used in code

    [Header("Current Growth Stage")]
    public GameObject PlantPrefab;
    public float GrowthRate;
    public float GrowTime;

    [Header("next growth stage")]
    public GameObject NextGrowthStagePrefab;

}


[CreateAssetMenu(fileName = "Plant_Registory", menuName = "Scriptable Objects/Plant_Registory")]
public class Plant_Registory : ScriptableObject
{

    [SerializeField]
    public GrowthStage[] GrowthStages;

}
