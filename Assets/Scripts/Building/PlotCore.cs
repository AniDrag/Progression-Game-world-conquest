using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a build plot; handles UI dropdown and spawning a Building from a preset.
/// Publishes OnBuildPlotSelected when the plot is selected.
/// </summary>
public class PlotCore : MonoBehaviour
{
    [Header("Control")]
    [Tooltip("When true, the plot will randomize its efficencies at Start.")]
    public bool USE_RANDOMIZE = true;

    [Header("Plot Data")]
    [SerializeField] public PlotPercent Mine;
    [SerializeField] public PlotPercent Plantation;
    [SerializeField] public PlotPercent Gold;

    [Header("UI References")]
    [SerializeField] private TMP_Dropdown buildingDropdown;
    [SerializeField] private Button plotButton;

    [Header("Building Data")]
    [SerializeField] private List<BuildingPreset> buildingTypes;

    [Header("Building Prefab")]
    [SerializeField] private GameObject buildingButton; // prefab which contains Building script

    public void OnSelected() => EventBus<OnBuildPlotSelected>.Publish(new OnBuildPlotSelected(this));

    private void Awake()
    {
        if (!plotButton) plotButton = GetComponent<Button>();
        if (!buildingDropdown) buildingDropdown = GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        if (USE_RANDOMIZE) RandomizeMyStuff();
    }

    private void OnEnable()
    {
        if (plotButton != null) plotButton.onClick.AddListener(PlotSelected);
        if (buildingDropdown != null) buildingDropdown.onValueChanged.AddListener(BuildingSelected);

        if (buildingDropdown != null)
        {
            buildingDropdown.value = 0;
            buildingDropdown.RefreshShownValue();
        }
    }

    private void OnDisable()
    {
        if (plotButton != null) plotButton.onClick.RemoveListener(PlotSelected);
        if (buildingDropdown != null) buildingDropdown.onValueChanged.RemoveListener(BuildingSelected);
    }

    private void PlotSelected()
    {
        OnSelected();
        if (buildingDropdown != null)
            buildingDropdown.gameObject.SetActive(true);
    }

    public void BuildingSelected(int index)
    {
        if (buildingDropdown == null) return;
        string selectedName = buildingDropdown.options[index].text;
        BuildBuildingByName(selectedName);
        buildingDropdown.gameObject.SetActive(false);
    }

    private void BuildBuildingByName(string name)
    {
        if (buildingTypes == null || buildingButton == null) return;

        BuildingPreset selectedData = buildingTypes.Find(b => b != null && b.BuildingName == name);
        if (selectedData == null)
        {
            Debug.LogWarning($"No BuildingData found for {name}");
            return;
        }

        var image = buildingButton.GetComponent<Image>();
        if (image != null) image.sprite = selectedData.Icon;

        int effectiveness = selectedData.type switch
        {
            BuildingType.Mine => PlotPercentConclusion(Mine),
            BuildingType.Plantation => PlotPercentConclusion(Plantation),
            BuildingType.Resort => PlotPercentConclusion(Gold),
            _ => 0
        };

        Building buildingData = buildingButton.GetComponent<Building>();
        if (buildingData == null)
        {
            Debug.LogError("Building prefab does not contain Building component.");
            return;
        }

        buildingData.Setup(selectedData, effectiveness);

        buildingButton.SetActive(true);
        this.gameObject.SetActive(false);

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

    void RandomizeMyStuff()
    {
        Mine = GetWeightedRandomPlotPercent();
        Plantation = GetWeightedRandomPlotPercent();
        Gold = GetWeightedRandomPlotPercent();
    }

    PlotPercent GetWeightedRandomPlotPercent()
    {
        Dictionary<PlotPercent, int> weights = new Dictionary<PlotPercent, int>()
        {
            { PlotPercent.Zero, 20 },
            { PlotPercent.Quarter, 50 },
            { PlotPercent.Half, 30 },
            { PlotPercent.ThreeQuarters, 20 },
            { PlotPercent.Full, 10 }
        };

        int totalWeight = 0;
        foreach (var w in weights.Values) totalWeight += w;

        int randomValue = Random.Range(0, totalWeight);
        int current = 0;

        foreach (var pair in weights)
        {
            current += pair.Value;
            if (randomValue < current) return pair.Key;
        }

        return PlotPercent.Half;
    }
}
