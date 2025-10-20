using UnityEngine;

[CreateAssetMenu(fileName = "ResourceStorage", menuName = "Tools/Resource Storage")]
public class ResourceStorage : ScriptableObject
{
    [SerializeField] private ResourceData _resources;
    public ResourceData resources => _resources;

    public void InitializeStorage(int metal, int organic, int gold) => _resources = new ResourceData(metal, organic, gold);
    public void Add(ResourceData value) => _resources.Add(value);
    public void AddSingle(BuildingType type, int amount) => _resources.AddSingle(type, amount);
    public void Remove(ResourceData cost) => _resources.Remove(cost);
    public void RemoveSingle(BuildingType type, int amount) => _resources.RemoveSingle(type, amount);
    public bool IsBiggerThan(ResourceData price) => _resources.IsBiggerThan(price);
    public string DisplayData(string title) => _resources.DisplayData(title);
}