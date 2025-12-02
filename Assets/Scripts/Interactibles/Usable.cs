using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Base class for simple actions/skills that consume resources and have cooldowns.
/// Handles UI wiring and basic checks. Specific logic should be put in derived classes (e.g. BuyPlanet).
/// </summary>
public class Usable : MonoBehaviour
{
    [Header("Action settings")]
    [Tooltip("Display name of the action.")]
    [SerializeField] protected string actionName;
    [Tooltip("Cost of using the action.")]
    [SerializeField] protected ResourcesCore cost;
    [Tooltip("Cooldown in turns.")]
    [SerializeField] protected int cooldown = 0;
    [Tooltip("Button that activates this usable.")]
    [SerializeField] protected Button activationBTN;
    [TextArea]
    [SerializeField] private string actionDescription;

    [Header("UI References")]
    [SerializeField] protected TMP_Text title;
    [SerializeField] protected TMP_Text price;
    [SerializeField] protected TMP_Text description;

    protected int _currentCooldown;

    protected bool CanBuy =>
        GameManager.Instance != null &&
        GameManager.Instance.PlayerResources != null &&
        cost != null &&
        GameManager.Instance.PlayerResources.PlayerHasEnoughResources(cost);

    protected virtual void OnValidate()
    {
        UpdateUsableUI();
    }

    protected virtual void Start()
    {
        if (activationBTN == null)
        {
            Debug.LogError($"Usable '{name}' has NO activation button assigned.");
            return;
        }

        activationBTN.interactable = ButtonEnableCondition();

        // subscribe to global events to keep interactable state updated
        EventBus<OnTurnEnd>.OnEvent += ReduceCooldown;
        EventBus<OnBuildingUpgraded>.OnEvent += OnBuildingUpgraded;
        EventBus<OnBuildingDestroyed>.OnEvent += OnBuildingDestroyed;
    }

    protected virtual void OnEnable()
    {
        if (activationBTN != null)
        {
            activationBTN.onClick.AddListener(ActivateAction);
            activationBTN.interactable = ButtonEnableCondition();
        }

        UpdateUsableUI();

        // subscribe (defensive duplicates won't break since we always unsubscribe on disable/destroy)
        EventBus<OnTurnEnd>.OnEvent += ReduceCooldown;
        EventBus<OnBuildingUpgraded>.OnEvent += OnBuildingUpgraded;
        EventBus<OnBuildingDestroyed>.OnEvent += OnBuildingDestroyed;
    }

    protected virtual void OnDisable()
    {
        if (activationBTN != null)
        {
            activationBTN.onClick.RemoveListener(ActivateAction);
        }

        EventBus<OnTurnEnd>.OnEvent -= ReduceCooldown;
        EventBus<OnBuildingUpgraded>.OnEvent -= OnBuildingUpgraded;
        EventBus<OnBuildingDestroyed>.OnEvent -= OnBuildingDestroyed;
    }

    private void ReduceCooldown(OnTurnEnd e)
    {
        if (_currentCooldown > 0) _currentCooldown--;

        if (activationBTN != null)
            activationBTN.interactable = e != null && e.Resource != null && e.Resource.PlayerHasEnoughResources(cost) && _currentCooldown <= 0;
    }

    private void OnBuildingUpgraded(OnBuildingUpgraded e)
    {
        if (activationBTN != null)
            activationBTN.interactable = ButtonEnableCondition();
    }

    private void OnBuildingDestroyed(OnBuildingDestroyed e)
    {
        if (activationBTN != null)
            activationBTN.interactable = ButtonEnableCondition();
    }

    protected void ActivateAction()
    {
        if (!CanBuy)
        {
            Debug.LogWarning($"Cannot use action '{actionName}', not enough resources.");
            return;
        }

        // Spend resources via GameManager's central authority
        if (!GameManager.Instance.TrySpendResources(cost))
        {
            Debug.LogWarning($"Usable '{actionName}': TrySpendResources failed unexpectedly.");
            return;
        }

        Debug.Log($"Action '{actionName}' activated.");

        _currentCooldown = cooldown;
        if (activationBTN != null) activationBTN.interactable = false;
    }

    protected void UpdateUsableUI()
    {
        if (title != null) title.text = $"Name: {actionName}";
        if (price != null) price.text = cost != null ? cost.DisplayData("Price") : "Price: N/A";
        if (description != null) description.text = $"Description:\n{actionDescription}";
    }

    protected virtual bool ButtonEnableCondition()
    {
        return CanBuy && _currentCooldown <= 0;
    }
}
