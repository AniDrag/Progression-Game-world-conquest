using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "BuildingTemplate", menuName = "Tools/BuildingTemplate")]
public class BuildingPreset : ScriptableObject
{
    [Tooltip("Human readable name of the building.")]
    [field: SerializeField]public string BuildingName {  get; private set; }
    [Tooltip("Type/category of the building used for multipliers and logic.")]
    [field: SerializeField] public BuildingType type { get; private set; }
    [Tooltip("Icon sprite used in UI lists / buttons.")]
    [field: SerializeField] public Sprite Icon { get; private set; }
    [Tooltip("Maximum output when plot efficiency = 100%")]
    [field: SerializeField] public int maxEfficiencyOutput { get; private set; } = 10;
    [Tooltip("Cost to build this building.")]
    [field: SerializeField] public ResourcesCore buildPrice { get; private set; }
    [Tooltip("Resources returned when the building is destroyed.")]
    [field: SerializeField] public ResourcesCore refundAmount { get; private set; }
    private void OnValidate()
    {
#if UNITY_EDITOR
        BuildingName = this.name;
#endif
    }
}
