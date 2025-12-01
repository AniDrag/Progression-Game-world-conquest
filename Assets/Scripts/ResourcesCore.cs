using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[System.Serializable]
public class ResourcesCore 
{
    private int organics;
    private int metal;
    private int gold;

    public int Organics => organics;
    public int Metal => metal;
    public int Gold => gold;

    public ResourcesCore(int pOrganics = 0, int pIron = 0, int pGold = 0)
    {
        organics = pOrganics;
        metal = pIron;
        gold = pGold;
    }
    public void Add(ResourcesCore other)
    {
        organics += other.organics;
        metal += other.metal;
        gold += other.gold;
        organics = Mathf.Min(organics,int.MaxValue);
        metal = Mathf.Min(metal, int.MaxValue);
        gold = Mathf.Min(gold, int.MaxValue);
    }
    public void Subtract(ResourcesCore other)
    {
        organics -= other.organics;
        metal -= other.metal;
        gold -= other.gold;

        organics = Mathf.Max(0, organics);
        metal = Mathf.Max(0, metal);
        gold = Mathf.Max(0, gold);
    }
    public bool PlayerHasEnoughResources(ResourcesCore cost)
    {
        return (organics >= cost.organics &&
         metal >= cost.metal &&
         gold >= cost.gold);
    }
    public string DisplayData(string title) => $" {title}\n Metal: {metal:N0},\n Organics: {organics:N0},\n Gold: {gold:N0}.";

}
