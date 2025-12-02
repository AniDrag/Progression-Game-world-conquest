using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Centralized core data holder: resources, building registry and global multipliers.
/// GameManager intentionally keeps responsibilities narrow — it's a data authority.
/// Other systems listen to events or call GameManager's public methods.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton (public to allow everyone to read)
    public static GameManager Instance { get; private set; }

    [Header("Resources")]
    [Tooltip("Starting metal for player.")]
    [SerializeField] private int startingMetal = 100;
    [Tooltip("Starting organic resources for player.")]
    [SerializeField] private int startingOrganic = 100;
    [Tooltip("Starting gold for player.")]
    [SerializeField] private int startingGold = 100;

    // Public read-only property for other systems to query/update via GameManager methods
    public ResourcesCore PlayerResources { get; private set; }

    [Header("UI references")]
    [Tooltip("End turn button (optional).")]
    [SerializeField] private Button endTurnBtn;

    [Header("Buildings build Data")]
    [Tooltip("Runtime list of active buildings (managed by GameManager).")]
    [SerializeField] private List<Building> buildings = new();

    [Header("Global Multipliers")]
    [Tooltip("Global multiplier for mines")]
    public float MineMultiplier { get; private set; } = 1f;
    [Tooltip("Global multiplier for plantations")]
    public float PlantationMultiplier { get; private set; } = 1f;
    [Tooltip("Global multiplier for resorts/gold")]
    public float GoldMultiplier { get; private set; } = 1f;
    public int TurnCounter { get; private set; } = 1;
    public Action UpdateResources;
    private void Awake()
    {
        // Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        PlayerResources = new ResourcesCore(startingOrganic, startingMetal, startingGold);

        // Register only to events that affect GameManager core data: construction & destruction
        EventBus<OnBuildingConstructed>.OnEvent += OnBuildingConstructed;
        EventBus<OnBuildingDestroyed>.OnEvent += OnBuildingDestroyed;
        EventBus<OnTypeUpgraded>.OnEvent += OnTypeUpgraded;

        if (endTurnBtn != null)
            endTurnBtn.onClick.AddListener(EndTurn);
    }

    private void OnDestroy()
    {
        // clean up subscriptions
        EventBus<OnBuildingConstructed>.OnEvent -= OnBuildingConstructed;
        EventBus<OnBuildingDestroyed>.OnEvent -= OnBuildingDestroyed;
        EventBus<OnTypeUpgraded>.OnEvent -= OnTypeUpgraded;

        if (endTurnBtn != null)
            endTurnBtn.onClick.RemoveListener(EndTurn);
    }

    /// <summary>
    /// Return the current multiplier for a building type (read-only helper).
    /// </summary>
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

    /// <summary>Apply a type upgrade to the global multipliers.</summary>
    private void OnTypeUpgraded(OnTypeUpgraded e)
    {
        if (e == null) return;
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
        UpdateResources?.Invoke();
    }

    /// <summary>
    /// Register a building in the manager list. Duplicate-safe.
    /// </summary>
    private void OnBuildingConstructed(OnBuildingConstructed e)
    {
        if (e?.building == null) return;
        if (!buildings.Contains(e.building))
        {
            buildings.Add(e.building);
            // let other subscribers know a building is selected by publishing the event
            EventBus<OnBuildingSelected>.Publish(new OnBuildingSelected(e.building));
        }
        UpdateResources?.Invoke();
    }

    private void OnBuildingDestroyed(OnBuildingDestroyed e)
    {
        if (e?.building == null) return;
        if (buildings.Contains(e.building))
        {
            PlayerResources.Add(e.building.Refund);   
            buildings.Remove(e.building);
            UpdateResources?.Invoke();
        }
    }

    /// <summary>
    /// End turn aggregator — calculates production and publishes OnTurnEnd
    /// GameManager only calculates based on core data and publishes; UI/other systems react.
    /// </summary>
    private void EndTurn()
    {
        int mineOutput = 0;
        int plantationOutput = 0;
        int resortOutput = 0;

        foreach (var b in buildings)
        {
            if (b == null) continue;
            switch (b.Type)
            {
                case BuildingType.Mine:
                    mineOutput += b.BuildingOutput;
                    break;
                case BuildingType.Plantation:
                    plantationOutput += b.BuildingOutput;
                    break;
                case BuildingType.Resort:
                    resortOutput += b.BuildingOutput;
                    break;
            }
        }

        var produced = new ResourcesCore(plantationOutput, mineOutput, resortOutput);
        // Update internal resources (GameManager owns core data)
        PlayerResources.Add(produced);

        // publish so other systems know to update UI or cooldowns
        EventBus<OnTurnEnd>.Publish(new OnTurnEnd(produced));
        TurnCounter++;
        UpdateResources?.Invoke();
    }

    // Exposed helpers for other systems wanting to change resources via GameManager (central authority)

    /// <summary>
    /// Try to spend resources; returns true if successful.
    /// </summary>
    public bool TrySpendResources(ResourcesCore cost)
    {
        if (cost == null) return false;
        if (PlayerResources.PlayerHasEnoughResources(cost))
        {
            PlayerResources.Subtract(cost);
            // publish a change if desired
            UpdateResources?.Invoke();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Add resources to the player's pool.
    /// </summary>
    public void AddResources(ResourcesCore add)
    {
        if (add == null) return;
        PlayerResources.Add(add);
        UpdateResources?.Invoke();
    }
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
