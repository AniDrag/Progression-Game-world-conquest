using UnityEngine;

public class TypeUpgrade : Usable
{
    [Header("Type settigs")]
    public BuildingType type;
    public int level = 1;
    protected float multi;

    public override void OnValidate()
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
        activationBTN.onClick.AddListener(Upgrade);
    }
    protected override void OnDisable()
    {
        base.OnEnable();
        activationBTN.onClick.RemoveListener(Upgrade);
    }

    void Upgrade()
    {
        if(level < 9)
        level++;
        cost = UpgradeMultiplier(type, level);
        EventBus<OnTypeUpgraded>.Publish(new OnTypeUpgraded(type, multi, cost));

    }
    private ResourcesCore UpgradeMultiplier(BuildingType type, int level)
    {
        // Target ~2k → 100k over 10 levels
        float baseCost = 2000f;
        float growthRate = 1.35f + (level * 0.05f); // slightly accelerating curve
        float cost = baseCost * Mathf.Pow(growthRate, level - 1);

        multi = 1f + (level - 1) * 0.15f; // gradual +15% bonus per level

        switch (type)
        {
            default: Debug.LogError("Wrong Type input or type missing"); return null;

            case BuildingType.Mine:
                return new ResourcesCore(
                    Mathf.RoundToInt(cost),
                    0,
                    Mathf.RoundToInt(cost * 0.8f)
                );

            case BuildingType.Plantation:
                return new ResourcesCore(
                    0,
                    Mathf.RoundToInt(cost),
                    Mathf.RoundToInt(cost * 0.8f)
                );

            case BuildingType.Resort:
                return new ResourcesCore(
                    Mathf.RoundToInt(cost * 0.7f),
                    Mathf.RoundToInt(cost * 0.7f),
                    Mathf.RoundToInt(cost)
                );
        }
    }

    protected override bool ButtonEnableCondition()
    {
        return CanBuy && level < 9;
    }
}
