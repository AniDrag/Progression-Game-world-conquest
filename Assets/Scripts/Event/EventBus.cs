using System;

/// This is a simple implementation of the event bus pattern
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventBus<T> where T : Event
{
    public static event Action<T> OnEvent;

    public static void Publish(T pEvent)
    {
        OnEvent?.Invoke(pEvent);
    }
}

/// <summary>
/// Base class for events, for this simple demonstration it is empty, more fields
/// could be added in real projects, e.g. eventData
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
    public OnBuildingConstructed(Building building) => this.building = building;
}
public class OnBuildPlotSelected : Event
{
    public int MineEfficiency;
    public int PlantationEfficiency;
    public int GoldEfficiency;
    public OnBuildPlotSelected(PlotCore plotDetails){
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
public class OnTurnEnd:Event
{
    public readonly ResourcesCore resource;
    public OnTurnEnd(ResourcesCore resource) => this.resource.Add(resource); 
}


