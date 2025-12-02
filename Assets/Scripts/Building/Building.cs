using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Building : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Button plotButton;
    [SerializeField] private Button buildingButton;

    [Header("Building Data")]
    [SerializeField] private BuildingType type;
    [SerializeField] private int level = 1;
    [SerializeField] private int baseOutput = 10;
    [SerializeField] private float efficiency = 1f; // 0–1, based on plot
    [SerializeField] private float typeMultiplier = 1f;

    [Header("Runtime")]
    [SerializeField] private int output;
    [SerializeField] private Sprite buildingImage;

    // internal resource caches (private fields)
    private ResourcesCore _price;
    private ResourcesCore _refund;
    private ResourcesCore _spent;      // Total resources spent on building & upgrades

    #region UI Refrences / Getters
    public string BuildingName => type.ToString();
    public BuildingType Type => type;
    public int BuildingLevel => level;
    public int BuildingOutput => output;
    public ResourcesCore Price => _price;
    public ResourcesCore Refund => _refund;
    #endregion

    private void Awake()
    {
        if (buildingButton == null)
            buildingButton = GetComponentInChildren<Button>();

        RecalculateOutput();
    }

    private void OnEnable()
    {
        if (buildingButton != null)
            buildingButton.onClick.AddListener(OnSelect);

        EventBus<OnTypeUpgraded>.OnEvent += OnTypeUpgraded;
    }

    private void OnDisable()
    {
        if (buildingButton != null)
            buildingButton.onClick.RemoveListener(OnSelect);

        EventBus<OnTypeUpgraded>.OnEvent -= OnTypeUpgraded;
    }

    /// <summary>
    /// Initialize this building using a preset and plot efficiency.
    /// Note: Setup expects GameManager.Instance to be present to read multipliers.
    /// </summary>
    public void Setup(BuildingPreset preset, float efficiencyPercent)
    {
        if (preset == null)
        {
            Debug.LogError("Building.Setup called with null preset.");
            return;
        }

        type = preset.type;
        level = 1;
        efficiency = Mathf.Clamp01(efficiencyPercent / 100f);
        baseOutput = Mathf.RoundToInt(preset.maxEfficiencyOutput * efficiency);

        // assign global multiplier from GameManager if available
        typeMultiplier = GameManager.Instance != null ? GameManager.Instance.GetMultiplier(preset.type) : 1f;
        _price = preset.buildPrice;
        _spent = new ResourcesCore();
        _spent.Add(_price);
        UpdateResources();
    }

    public void OnSelect() => EventBus<OnBuildingSelected>.Publish(new OnBuildingSelected(this));

    public void OnUpgrade()
    {
        if (level > 9)
        {
            Debug.Log("Building level is max");
            return;
        }

        // check resources before upgrading
        if (!GameManager.Instance.TrySpendResources(_price))
        {
            Debug.Log("Not enough resources to upgrade building.");
            return;
        }

        level++;
        UpdateResources();
        EventBus<OnBuildingUpgraded>.Publish(new OnBuildingUpgraded(this));
    }

    public void OnDestory()
    {
        // give refund via GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.AddResources(_refund);

        if (plotButton != null) plotButton.gameObject.SetActive(true);

        EventBus<OnBuildingDestroyed>.Publish(new OnBuildingDestroyed(this));
        this.gameObject.SetActive(false);
    }

    private void OnTypeUpgraded(OnTypeUpgraded e)
    {
        if (e == null) return;
        if (e.type == this.type)
        {
            typeMultiplier = e.newMultiplier;
            UpdateResources();
            EventBus<OnBuildingUpgraded>.Publish(new OnBuildingUpgraded(this));
        }
    }

    #region Math Functions

    private void RecalculateOutput() => output = Mathf.RoundToInt(baseOutput * Mathf.Pow(2, level - 1) * typeMultiplier);

    /// <summary>
    /// Recalculates price & refund based on output.
    /// Keeps internal _price/_refund as ResourcesCore objects used by other systems.
    /// </summary>
    private void UpdateResources()
    {
        RecalculateOutput();

        int priceA = Mathf.Max(1, Mathf.RoundToInt(output * 1.8f));
        int priceB = Mathf.Max(1, Mathf.RoundToInt(output * 1.7f));
        int priceC = Mathf.Max(1, Mathf.RoundToInt(output * 1.9f));

        int refundA = Mathf.Max(1, Mathf.RoundToInt(output * 0.25f));
        int refundB = Mathf.Max(1, Mathf.RoundToInt(output * 0.25f));
        int refundC = Mathf.Max(1, Mathf.RoundToInt(output * 0.25f));

        // update ResourcesCore caches

        _spent.Add(_price);
        _price = new ResourcesCore(priceA, priceB, priceC);
        _refund = new ResourcesCore(priceA, priceB, priceC);
    }

    #endregion
}
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

public enum BuildingType
{
    Mine,
    Plantation,
    Resort
}