using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Building : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Button buildBuildingBtn;
    [SerializeField] private Button buildingButton;

    [Header("Building Data")]
    [SerializeField] private BuildingType type;
    [SerializeField] private int level = 1;
    [SerializeField] private int baseOutput = 10;
    [SerializeField] private float efficiency = 1f; // 0–1, based on paper map
    [SerializeField] private float typeMultiplier = 1f;

    [Header("Runtime")]
    [SerializeField] private int output;
    [SerializeField] private Sprite buildingImage;
    [SerializeField] private ResourceData price = new ResourceData();
    [SerializeField] private ResourceData refund = new ResourceData();

    #region UI Refrences / Getters
    public string BuildingName => type.ToString();
    public BuildingType Type => type;
    public int BuildingLevel => level;
    public int BuildingOutput => output;
    public ResourceData Price => price;
    public ResourceData Refund => refund;
    #endregion

    
    private void Awake()
    {
        if (buildingButton == null)
            buildingButton = GetComponentInChildren<Button>();

        RecalculateOutput();
    }
    private void OnEnable()
    {
        buildingButton.onClick.AddListener(OnSelect);
        EventBus<OnTypeUpgraded>.OnEvent += OnTypeUpgraded;
    }

    private void OnDisable()
    {
        buildingButton.onClick.RemoveListener(OnSelect);
        EventBus<OnTypeUpgraded>.OnEvent -= OnTypeUpgraded;
    }

    public void Setup(BuildingPreset preset, float efficiencyPercent)
    {
        type = preset.type;
        level = 1;
        efficiency = Mathf.Clamp01(efficiencyPercent / 100f);
        baseOutput = Mathf.RoundToInt(preset.maxEfficiencyOutput * efficiency);

        // Assign global multipliers
        typeMultiplier = GameManager.instance.GetMultiplier(preset.type);
        UpdateResources();
    }
    public void OnSelect() => EventBus<OnBuildingSelected>.Publish(new OnBuildingSelected(this));
    public void OnUpgrade()
    {
        /// Upgrade
        /// 
        /// Upgrades building, dubbeling its stats.
        /// Capped at level 10.
        /// Is effected by the TypeBonus.
        /// 
        if(level > 9 && !GameManager.instance.resourceStorage.IsBiggerThan(Price)) { Debug.Log("Building level is max"); return; }

        level++;
        GameManager.instance.resourceStorage.Remove(price); // if i change this how can i make an event that will cal Ui update?
        UpdateResources();

        // Fire UI event
        EventBus<OnBuildingUpgraded>.Publish(new OnBuildingUpgraded(this));
    }
    public void OnDestory()
    {
        this.gameObject.SetActive(false);
        buildBuildingBtn.gameObject.SetActive(true);

        // Fire UI event
        EventBus<OnBuildingDestroyed>.Publish(new OnBuildingDestroyed(this));
    }
    private void OnTypeUpgraded(OnTypeUpgraded e)
    {
        if (e.type == this.type)
        {
            // update local multiplier and recalc
            typeMultiplier = e.newMultiplier;
            UpdateResources(); // this recalcs output & price & refund
                               // notify UI if needed
            EventBus<OnBuildingUpgraded>.Publish(new OnBuildingUpgraded(this));
        }
    }

    #region Math Functions
    private void RecalculateOutput() => output = Mathf.RoundToInt(baseOutput * Mathf.Pow(2, level - 1) * typeMultiplier);
    private void UpdateResources()
    {
        RecalculateOutput();

        // Base price derived from efficiency
        price.Set(Mathf.RoundToInt(output * 1.8f), Mathf.RoundToInt(output * 1.7f), Mathf.RoundToInt(output * 1.9f));
        refund.Set(Mathf.RoundToInt(output * 0.25f), Mathf.RoundToInt(output * 0.25f), Mathf.RoundToInt(output * 0.25f));
    }

    #endregion
    /// Upgrade
    /// 
    /// Upgrades building, dubbeling its stats.
    /// Capped at level 10.
    /// Is effected by the TypeBonus.

    /// Updating UI:
    /// needs public functions for
    ///     - Name,
    ///     - Level,
    ///     - Final output,
    ///     - Upgrade price,
    ///     - On destroy resources we get back,

    /// OnDestroy:
    /// when destroyes we give back resources,
    /// Remove buildig from buildings manager.
    /// Disable this button for Building and we enable BuildPlot button.
    /// We also cleare data from this so no resedue is there.
}

public enum BuildingType
{
    Mine,
    Plantation,
    Resort
}
[System.Serializable]
public class ResourceData
{
    public int Metal;
    public int Organic;
    public int Gold;
    public ResourceData(int metal = 10, int organic = 10, int gold = 10)
    {
        Set(metal, organic, gold);
    }

    public void Set(int metal, int organic, int gold)
    {
        Metal = metal;
        Organic = organic;
        Gold = gold;
    }
    public void Add(ResourceData add) => ModifyResourceGroup(add, 1);
    public void Remove(ResourceData cost) => ModifyResourceGroup(cost, -1);
    public bool IsBiggerThan(ResourceData price)
    {
        return Metal >= price.Metal &&
               Organic >= price.Organic &&
               Gold >= price.Gold;
    }
    public void AddSingle(BuildingType type, int amount) => ModifyResource(type, amount);
    public void RemoveSingle(BuildingType type, int amount) => ModifyResource(type, -amount);
    public string DisplayData(string title) => $" {title}\n Metal: {Metal:N0},\n Organics: {Organic:N0},\n Gold: {Gold:N0}.";

    #region Math Functions
    void ModifyResource(BuildingType type, int amount)
    {
        switch (type)
        {
            case BuildingType.Mine: Metal += amount; break;
            case BuildingType.Plantation: Organic += amount; break;
            case BuildingType.Resort: Gold += amount; break;
        }
    }
    private void ModifyResourceGroup(ResourceData data, int multiplier)
    {
        Metal += data.Metal * multiplier;
        Organic += data.Organic * multiplier;
        Gold += data.Gold * multiplier;
    }

    #endregion

    /// AddSingle(type, amount)
    ///     switch(type) -> type += amoutn.
    ///     
    /// RemoveSingle(type, amoutn)
    ///     switch(type) -> type -= amoutn.
    ///     
    /// IsBiggerThan(ResourceData data)
    ///     data <= resources.
    ///     
    /// Display() => metal amount \n organics amount \n, Gold amount.
}