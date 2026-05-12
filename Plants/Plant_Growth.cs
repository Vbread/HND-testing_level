using UnityEngine;

public class Plant_Growth : MonoBehaviour
{
    public Plant_SO_Base_Script CurrentPlant;

    [Header("Possition Requirement effects")]
    public int CurrentWaterLevel;
    public int CurrentSunlightLevel;
    public int CurrentSoilQuality;

    [Header("Runtime Growth Tracking, dont touch it.")]
    public float GrowthTimer;

    

    private void Awake()
    {
        
        Instantiate(CurrentPlant.ModelPrefab, transform.position,transform.rotation, transform);
        GrowthTimer = CurrentPlant.TimeTillNextStage;

    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentPlant.IsFinalStage)
        {
            case true:
                print("Plant is fully grown, you can add harvesting logic here");

             break;

            case false:
                GrowthTimer -= CurrentPlant.GrowthRate * Time.deltaTime;

             break; 
        }

        CheckRequirements();

        if (GrowthTimer <= 0)
        {
            AdvanceStage();
        }
        else
        {
            // visual feedback, like it growing or just adding a timer above the plant, can be implemented here. 
        }

    }

    private void AdvanceStage()
    {
        if (CurrentPlant.NextStage != null)
        {
            Destroy(transform.GetChild(0).gameObject); 
            CurrentPlant = CurrentPlant.NextStage;
            GrowthTimer = CurrentPlant.TimeTillNextStage;
            Instantiate(CurrentPlant.ModelPrefab, transform.position, transform.rotation, transform);

        }

    }

    private void CheckRequirements()
    {
        if (CurrentPlant.WaterRequirement > CurrentWaterLevel | CurrentPlant.SunlightRequirement > CurrentSunlightLevel | CurrentPlant.SoilQualityRequirement > CurrentSoilQuality)
        {
            print("Requirements not met for " + CurrentPlant.StageName + ", resetting growth timer.");
            GrowthTimer = CurrentPlant.TimeTillNextStage; // Reset growth timer if requirements are not met

        }
    }

    public void AddWater(int amount)
    {
        CurrentWaterLevel = Mathf.Min(4, CurrentWaterLevel + amount);
    }

    public void AddSunlight(int amount)
    {
        CurrentSunlightLevel = Mathf.Min(4, CurrentSunlightLevel + amount);
    }

    public void AddSoilQuality(int amount)
    {
        CurrentSoilQuality = Mathf.Min(4, CurrentSoilQuality + amount);
    }

    public void RemoveWater(int amount)
    {
        CurrentWaterLevel = Mathf.Max(0,CurrentWaterLevel - amount);
    }

    public void RemoveSunlight(int amount)
    {
        CurrentSunlightLevel = Mathf.Max(0, CurrentSunlightLevel - amount);
    }

    public void DegradeSoilQuality(int amount)
    {
        CurrentSoilQuality = Mathf.Max(0, CurrentSoilQuality - amount);
    }

    public void Harvest()
    {
        // call this from player controller (Declan)
        int harvestedAmount = CurrentPlant.AmountCanBeHarvested;

        if (harvestedAmount > 0)
        {
            // add invertory logic here
        }

        Destroy(gameObject);
    }

    public void ApplyPositivePollutionEffect()
    {
        // Implement logic to apply positive pollution effect to the environment
    }

}
