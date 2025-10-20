using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// To do Get cell Efficiency as an input and asign it to said building
/// </summary>
public class BuildBuilding : MonoBehaviour
{
    [Header("Plot Data")]
    [SerializeField] public PlotPercent Mine;
    [SerializeField] public PlotPercent Plantation;
    [SerializeField] public PlotPercent Gold;

    [Header("UI References")]
    [SerializeField] private TMP_Dropdown buildingDropdown;
    [SerializeField] private Button buildButton;

    [Header("Building Data")]
    [SerializeField] private List<BuildingPreset> buildingTypes;

    [Header("Building Prefab")]
    [SerializeField] private GameObject buildingBTN; // The prefab that has the Building script

    public void OnSelected() => EventBus<OnBuildPlotSelected>.Publish(new OnBuildPlotSelected(this));
    private void Awake()
    {
        if (!buildButton) buildButton = GetComponent<Button>();
        //buildingDropdown.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        buildButton?.onClick.AddListener(OnBuildButtonClicked);
        buildingDropdown?.onValueChanged.AddListener(BuildingSelected);
    }

    private void OnDisable()
    {
        buildButton?.onClick.RemoveListener(OnBuildButtonClicked);
        buildingDropdown?.onValueChanged.RemoveListener(BuildingSelected);
    }

    private void OnBuildButtonClicked()
    {
        OnSelected();
        buildingDropdown.gameObject.SetActive(true);
    }

    public void BuildingSelected(int index)
    {
        string selectedName = buildingDropdown.options[index].text;
        BuildBuildingByName(selectedName);
        buildingDropdown.gameObject.SetActive(false);
    }

    private void BuildBuildingByName(string name)
    {
        BuildingPreset selectedData = buildingTypes.Find(b => b.buildingName == name);
        if (selectedData == null)
        {
            Debug.LogWarning($"No BuildingData found for {name}");
            return;
        }

        // Apply UI appearance
        buildingBTN.GetComponent<Image>().sprite = selectedData.icon;

        // Calculate plot effectiveness
        int effectiveness = selectedData.type switch
        {
            BuildingType.Mine => PlotPercentConclusion(Mine),
            BuildingType.Plantation => PlotPercentConclusion(Plantation),
            BuildingType.Resort => PlotPercentConclusion(Gold),
            _ => 0
        };

        // Setup the building
        Building buildingData = buildingBTN.GetComponent<Building>();
        buildingData.Setup(selectedData, effectiveness);

        // Enable the building button (the actual plot)
        buildingBTN.SetActive(true);
        this.gameObject.SetActive(false);

        // Fire event instead of direct GameManager call
        EventBus<OnBuildingConstructed>.Publish(new OnBuildingConstructed(buildingData));
    }

    public int PlotPercentConclusion(PlotPercent setup) => setup switch
    {
        PlotPercent.Quarter => 25,
        PlotPercent.Half => 50,
        PlotPercent.ThreeQuarters => 75,
        PlotPercent.Full => 100,
        _ => 0,
    };

    public enum PlotPercent
    {
        Zero,
        Quarter,
        Half,
        ThreeQuarters,
        Full
    }
}
