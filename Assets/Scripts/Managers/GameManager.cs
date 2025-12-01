using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Resources")]
    [SerializeField] int startingMetal = 100;
    [SerializeField] int startingOrganic = 100;
    [SerializeField] int startingGold = 100;
    public ResourcesCore playerResources { get; private set; }

    [Header("UI referneces")]
    [SerializeField] private Button EndTurnBtn;

    [Header("Buildings build Data")]
    [SerializeField] private List<Building> buildings = new();// buildings are added by event, only when cosntructed, removed when destroyed.

    [Header("Global Multipliers")]
    public float MineMultiplier = 1f;
    public float PlantationMultiplier = 1f;
    public float GoldMultiplier = 1f;


    //public ResourceStorage resourceStorage;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        playerResources = new ResourcesCore(startingOrganic, startingMetal, startingGold);// set starting resources

        //resourceStorage.InitializeStorage(10, 10, 10);
        EventBus<OnBuildingConstructed>.OnEvent += OnBuildingConstructed;
        EventBus<OnBuildingDestroyed>.OnEvent += OnBuildingDestroyed;
        EventBus<OnTypeUpgraded>.OnEvent -= OnTypeUpgraded;
        EndTurnBtn.onClick.AddListener(EndTurn);
    }
    public float GetMultiplier(BuildingType type)
    {
        return type switch
        {
            BuildingType.Mine => MineMultiplier,
            BuildingType.Plantation => PlantationMultiplier,
            BuildingType.Resort => GoldMultiplier,
            _ => 1f
        };
    }

    private void OnTypeUpgraded(OnTypeUpgraded e)
    {
        switch (e.type)
        {
            case BuildingType.Mine:
                MineMultiplier = e.newMultiplier;
                break;
            case BuildingType.Plantation:
                PlantationMultiplier = e.newMultiplier;
                break;
            case BuildingType.Resort:
                GoldMultiplier = e.newMultiplier;
                break;
        }
    }    
    private bool AddBuilding(Building building)
    {
        if (!buildings.Contains(building)){
            buildings.Add(building);
            return true;
        }
        return false;
    }
    private void RemoveBuilding(Building building)
    {
        if (buildings.Contains(building))
        {
            buildings.Remove(building);
            //playerResources.Add(building.Refund);
        }
    }
    private void EndTurn()
    {
        int mineOutput = 0;
        int plantationOutput = 0;
        int resortOutput = 0;
        foreach (var b in buildings)
        {
            switch (b.Type)
            {
                case BuildingType.Mine:
                    mineOutput +=b.BuildingOutput;
                    break;
                case BuildingType.Plantation:
                    plantationOutput += b.BuildingOutput;
                    break;
                case BuildingType.Resort:
                    resortOutput += b.BuildingOutput;
                    break;
            }
        }
        EventBus<OnTurnEnd>.Publish(new OnTurnEnd(new ResourcesCore(plantationOutput, mineOutput, resortOutput)));
    }
    private void OnBuildingConstructed(OnBuildingConstructed e)
    {
        if(AddBuilding(e.building))// only trigger if this building doesnt exist yet. no duplicates on tiles.
            EventBus<OnBuildingSelected>.Publish(new OnBuildingSelected(e.building));
    }
    private void OnBuildingDestroyed(OnBuildingDestroyed e)
    {
        RemoveBuilding(e.building);
    }


    /// End turn:
    /// Collect Money from owned buildings.
    /// Decrease Turn cooldown on abilities.
    /// 
    /// COMPLETE

    /// Update MoneyChanged:
    /// Called By End Turn to pudate the storage of resources.
    /// Called by: 
    ///     - Attack, 
    ///     - Defense, 
    ///     - Upgrades of buildings, 
    ///     - Upgrades of Building Types, 
    ///     - Purchesing Planet, 
    ///     - Destroying buildings, 
    ///     - Building buildings. 
    ///     !! this ither increases or decreases Storage !!
    ///     
    ///     solution is action UpdateResources !! 
    ///     
    ///     /// COMPLETE


    /// Select Building:
    /// Updates UI of Building Info.
    ///     - Title of building,
    ///     - Building type,
    ///     - Efficiency output,
    ///     - Bonus increse,
    ///     - Final output,
    ///     - Can upgrade button is interactibel?,
    ///     - Destroy building option.
    ///     
    /// Closes Build Plot info.
    /// Enable Building info.
    /// Calculates values and data.
    /// Other ui elements are managed.

    /// Select BuildPlot:
    /// Updates UI of BuildPlot Info:
    ///     - Plot efficiencies,
    ///     - build options.
    ///     
    /// Closes Building info.
    /// Enable BuildPlot info.
}
