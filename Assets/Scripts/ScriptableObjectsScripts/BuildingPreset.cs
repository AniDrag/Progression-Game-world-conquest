using UnityEngine;
[CreateAssetMenu(fileName = "BuildingTemplate", menuName = "Tools/BuildingTemplate")]
public class BuildingPreset : ScriptableObject
{
    public string buildingName;
    public BuildingType type;
    public Sprite icon;
    public int maxEfficiencyOutput = 10;

    public ResourcesCore buildPrice;
    public ResourcesCore refundAmount;
}
