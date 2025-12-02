using System;

/// <summary>
/// Simple generic static EventBus. Subscribers should register/unregister on OnEnable/OnDisable.
/// </summary>
public static class EventBus<T> where T : Event
{
    public static event Action<T> OnEvent;

    public static void Publish(T pEvent)
    {
        // defensive: don't call if null
        OnEvent?.Invoke(pEvent);
    }
}

/// <summary>
/// Base event type. Keep extension lightweight.
/// </summary>
public abstract class Event { }

public class OnBuildingSelected : Event
{
    public readonly Building building;
    public OnBuildingSelected(Building building) => this.building = building;
}

public class OnBuildingUpgraded : Event
{
    public readonly Building building;
    public OnBuildingUpgraded(Building building) => this.building = building;
}

public class OnBuildingDestroyed : Event
{
    public readonly Building building;
    public OnBuildingDestroyed(Building building) => this.building = building;
}

public class OnBuildingConstructed : Event
{
    public readonly Building building;
    public readonly ResourcesCore constructionCost;
    public OnBuildingConstructed(Building building)
    {
         this.building = building;
    }
}

public class OnBuildPlotSelected : Event
{
    public readonly int MineEfficiency;
    public readonly int PlantationEfficiency;
    public readonly int GoldEfficiency;

    public OnBuildPlotSelected(PlotCore plotDetails)
    {
        MineEfficiency = plotDetails.PlotPercentConclusion(plotDetails.Mine);
        PlantationEfficiency = plotDetails.PlotPercentConclusion(plotDetails.Plantation);
        GoldEfficiency = plotDetails.PlotPercentConclusion(plotDetails.Gold);
    }
}

public class OnTypeUpgraded : Event
{
    public readonly BuildingType type;
    public readonly float newMultiplier;
    public readonly ResourcesCore cost;

    public OnTypeUpgraded(BuildingType type, float newMultiplier, ResourcesCore cost)
    {
        this.type = type;
        this.newMultiplier = newMultiplier;
        this.cost = cost;
    }
}

public class OnTurnEnd : Event
{
    // carry produced resources for the turn
    public readonly ResourcesCore Resource;
    public OnTurnEnd(ResourcesCore resource)
    {
        // defensive copy to avoid external mutation surprises
        Resource = resource != null ? new ResourcesCore(resource.Organics, resource.Metal, resource.Gold) : new ResourcesCore();
    }
}
