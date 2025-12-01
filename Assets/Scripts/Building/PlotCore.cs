using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// To do Get cell Efficiency as an input and asign it to said building
/// </summary>
public class PlotCore : MonoBehaviour
{
    [Header("================CONTROLL=================")]
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
    [SerializeField] private GameObject buildingButton    ; // The prefab that has the Building script

    public void OnSelected() => EventBus<OnBuildPlotSelected>.Publish(new OnBuildPlotSelected(this));
    private void Awake()
    {
        if (!plotButton) plotButton = GetComponent<Button>();
        if (!buildingDropdown) buildingDropdown = GetComponent<TMP_Dropdown>();
        buildingDropdown.gameObject.SetActive(false);
    }
    private void Start()
    {
        RandomizeMyStuff();
    }
    private void OnEnable()
    {
        plotButton?.onClick.AddListener(PlotSelected);
        buildingDropdown?.onValueChanged.AddListener(BuildingSelected);

        buildingDropdown.value = 0;
        buildingDropdown.RefreshShownValue();
    }

    private void OnDisable()
    {
        plotButton?.onClick.RemoveListener(PlotSelected);
        buildingDropdown?.onValueChanged.RemoveListener(BuildingSelected);
    }

    private void PlotSelected()
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
            //Debug.LogWarning($"No BuildingData found for {name}");
            return;
        }

        // Apply UI appearance
        buildingButton.GetComponent<Image>().sprite = selectedData.icon;

        // Calculate plot effectiveness
        int effectiveness = selectedData.type switch
        {
            BuildingType.Mine => PlotPercentConclusion(Mine),
            BuildingType.Plantation => PlotPercentConclusion(Plantation),
            BuildingType.Resort => PlotPercentConclusion(Gold),
            _ => 0
        };

        // Setup the building
        Building buildingData = buildingButton.GetComponent<Building>();
        buildingData.Setup(selectedData, effectiveness);

        // Enable the building button (the actual plot)
        buildingButton.SetActive(true);
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

    void RandomizeMyStuff()
    {
        Mine = GetWeightedRandomPlotPercent();
        Plantation = GetWeightedRandomPlotPercent();
        Gold = GetWeightedRandomPlotPercent();

        //Debug.Log($"Randomized -> Mine:{Mine}, Plantation:{Plantation}, Gold:{Gold}");
    }

    PlotPercent GetWeightedRandomPlotPercent()
    {
        // Weights (you can tweak these)
        // Lower number = rarer
        Dictionary<PlotPercent, int> weights = new Dictionary<PlotPercent, int>()
    {
        { PlotPercent.Zero, 20 },
        { PlotPercent.Quarter, 50 },
        { PlotPercent.Half, 30 },
        { PlotPercent.ThreeQuarters, 20 },
        { PlotPercent.Full, 10 } // ★ VERY RARE
    };

        int totalWeight = 0;
        foreach (var w in weights.Values)
            totalWeight += w;

        int randomValue = Random.Range(0, totalWeight);
        int current = 0;

        foreach (var pair in weights)
        {
            current += pair.Value;
            if (randomValue < current)
                return pair.Key;
        }

        return PlotPercent.Half; // fallback (should never hit)
    }

}
