using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    public ResourceStorage resourceStorage;

    [Header("UI referneces")]
    [SerializeField] private Button EndTurnBtn;

    [Header("Buildings build Data")]
    [SerializeField] private List<Building> buildings = new();

    [Header("Global Multipliers")]
    public float MineMultiplier = 1f;
    public float PlantationMultiplier = 1f;
    public float GoldMultiplier = 1f;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        resourceStorage.InitializeStorage(10, 10, 10);
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
    private void AddBuilding(Building building)
    {
        if (!buildings.Contains(building))
            buildings.Add(building);
    }
    private void RemoveBuilding(Building building)
    {
        if (buildings.Contains(building))
            buildings.Remove(building);
    }
    private void EndTurn()
    {
        foreach (var b in buildings)
        {
            switch (b.Type)
            {
                case BuildingType.Mine:
                    resourceStorage.resources.AddSingle(b.Type, b.BuildingOutput);
                    break;
                case BuildingType.Plantation:
                    resourceStorage.resources.AddSingle(b.Type, b.BuildingOutput);
                    break;
                case BuildingType.Resort:
                    resourceStorage.resources.AddSingle(b.Type, b.BuildingOutput);
                    break;
            }
        }
        EventBus<OnTurnEnd>.Publish(new OnTurnEnd(resourceStorage.resources));
    }
    private void OnBuildingConstructed(OnBuildingConstructed e)
    {
        AddBuilding(e.building);
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
