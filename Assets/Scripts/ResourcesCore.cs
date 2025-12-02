using UnityEngine;

[System.Serializable]
public class ResourcesCore
{
    // internal fields (not serialized by default; use wrappers)
    [SerializeField] private int organics;
    [SerializeField] private int metal;
    [SerializeField] private int gold;

    public int Organics => organics;
    public int Metal => metal;
    public int Gold => gold;

    public ResourcesCore(int pOrganics = 0, int pMetal = 0, int pGold = 0)
    {
        organics = Mathf.Max(0, pOrganics);
        metal = Mathf.Max(0, pMetal);
        gold = Mathf.Max(0, pGold);
    }

    /// <summary>
    /// Safely add other resources into this one (caps at int.MaxValue).
    /// </summary>
    public void Add(ResourcesCore other)
    {
        if (other == null) return;
        organics = SafeAdd(organics, other.organics);
        metal = SafeAdd(metal, other.metal);
        gold = SafeAdd(gold, other.gold);
    }

    /// <summary>
    /// Subtract other resources; clamped at zero (no negative).
    /// </summary>
    public void Subtract(ResourcesCore other)
    {
        if (other == null) return;
        organics = Mathf.Max(0, organics - other.organics);
        metal = Mathf.Max(0, metal - other.metal);
        gold = Mathf.Max(0, gold - other.gold);
    }

    public bool PlayerHasEnoughResources(ResourcesCore cost)
    {
        if (cost == null) return true; // no cost
        return organics >= cost.organics &&
               metal >= cost.metal &&
               gold >= cost.gold;
    }

    public string DisplayData(string title)
        => $"{title}\n Metal: {metal:N0},\n Organics: {organics:N0},\n Gold: {gold:N0}.";

    private int SafeAdd(int a, int b)
    {
        long result = (long)a + b;
        return (int)Mathf.Min(result, int.MaxValue);
    }
}
