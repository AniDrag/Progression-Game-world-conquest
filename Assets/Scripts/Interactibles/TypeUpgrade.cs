using UnityEngine;

/// <summary>
/// Upgrade that affects a building type globally (changes multiplier and cost).
/// Uses Usable as base (UI + cost/cooldown handling).
/// </summary>
public class TypeUpgrade : Usable
{
    [Header("Type settings")]
    [Tooltip("Which building type this upgrade affects.")]
    public BuildingType type;

    [Tooltip("Current upgrade level (1..9).")]
    public int level = 1;

    protected float multi;

    protected override void OnValidate()
    {
        cost = UpgradeMultiplier(type, level);
        base.OnValidate();
    }

    protected override void Start()
    {
        activationBTN.interactable = ButtonEnableCondition();
        cost = UpgradeMultiplier(type, level);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (activationBTN != null)
            activationBTN.onClick.AddListener(Upgrade);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (activationBTN != null)
            activationBTN.onClick.RemoveListener(Upgrade);
    }

    private void Upgrade()
    {
        if (level < 9) level++;
        cost = UpgradeMultiplier(type, level);

        // Publish type upgrade. GameManager listens to update global multiplier.
        EventBus<OnTypeUpgraded>.Publish(new OnTypeUpgraded(type, multi, cost));
        activationBTN.interactable = ButtonEnableCondition();
    }

    /// <summary>
    /// Compute cost and multiplier for the selected type & level.
    /// Returns a ResourcesCore representing the cost.
    /// </summary>
    private ResourcesCore UpgradeMultiplier(BuildingType type, int level)
    {
        // Target ~2k → 100k over 10 levels
        float baseCost = 2000f;
        float growthRate = 1.35f + (level * 0.05f);
        float cost = baseCost * Mathf.Pow(growthRate, level - 1);

        multi = 1f + (level - 1) * 0.15f; // +15% per level

        switch (type)
        {
            default:
                Debug.LogError("TypeUpgrade: Wrong Type input or type missing");
                return new ResourcesCore();

            case BuildingType.Mine:
                return new ResourcesCore(0, Mathf.RoundToInt(cost), Mathf.RoundToInt(cost * 0.8f));

            case BuildingType.Plantation:
                return new ResourcesCore(Mathf.RoundToInt(cost), 0, Mathf.RoundToInt(cost * 0.8f));

            case BuildingType.Resort:
                return new ResourcesCore(Mathf.RoundToInt(cost * 0.7f), Mathf.RoundToInt(cost * 0.7f), Mathf.RoundToInt(cost));
        }
    }

    protected override bool ButtonEnableCondition()
    {
        return CanBuy && level < 9;
    }
}
