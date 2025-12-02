using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI panel that displays building details and provides upgrade/destroy interactions.
/// Subscribes to events to keep UI in sync.
/// </summary>
public class BuildingDetails : MonoBehaviour
{
    [Header("Building References")]
    [SerializeField] private TMP_Text buildingName;
    [SerializeField] private TMP_Text buildingLevel;
    [SerializeField] private TMP_Text totalOutput;

    [Header("Upgrade btn References")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TMP_Text upgradeCost;

    [Header("Destroy btn References")]
    [SerializeField] private Button destroyButton;
    [SerializeField] private TMP_Text refundText;

    private Building _currentBuilding;

    private void Start()
    {
        ClearUI();
    }

    private void OnEnable()
    {
        EventBus<OnBuildingSelected>.OnEvent += OnBuildingSelected;
        EventBus<OnBuildingUpgraded>.OnEvent += OnBuildingUpgraded;
        EventBus<OnBuildingDestroyed>.OnEvent += OnBuildingDestroyed;
        EventBus<OnBuildPlotSelected>.OnEvent += OnBuildPlotSelected;

        if (upgradeButton != null) upgradeButton.onClick.AddListener(OnUpgrade);
        if (destroyButton != null) destroyButton.onClick.AddListener(OnDestroyBuilding);
    }

    private void OnDisable()
    {
        EventBus<OnBuildingSelected>.OnEvent -= OnBuildingSelected;
        EventBus<OnBuildingUpgraded>.OnEvent -= OnBuildingUpgraded;
        EventBus<OnBuildingDestroyed>.OnEvent -= OnBuildingDestroyed;
        EventBus<OnBuildPlotSelected>.OnEvent -= OnBuildPlotSelected;

        if (upgradeButton != null) upgradeButton.onClick.RemoveListener(OnUpgrade);
        if (destroyButton != null) destroyButton.onClick.RemoveListener(OnDestroyBuilding);
    }

    private void OnBuildingSelected(OnBuildingSelected e)
    {
        if (e?.building == null) return;
        SetButtonActivity(true);
        UpdateInfoTable(e.building);
    }

    private void OnBuildPlotSelected(OnBuildPlotSelected e)
    {
        SetButtonActivity(false);
        _currentBuilding = null;
        buildingName.text = "";
        buildingLevel.text = "";
        totalOutput.text = "";
        upgradeCost.text = "";
        refundText.text = "";

        buildingName.text = $"Efficiencies\n Mine: {e.MineEfficiency} \n Plantation: {e.PlantationEfficiency}\n Gold: {e.GoldEfficiency}";
    }

    private void OnBuildingUpgraded(OnBuildingUpgraded e)
    {
        if (e?.building == null) return;
        if (e.building == _currentBuilding)
            UpdateInfoTable(e.building);
    }

    private void OnBuildingDestroyed(OnBuildingDestroyed e)
    {
        if (e?.building == null) return;
        if (e.building == _currentBuilding)
            ClearUI();
    }

    protected void UpdateInfoTable(Building building)
    {
        if (building == null) return;
        _currentBuilding = building;

        var resources = GameManager.Instance?.PlayerResources;
        if (resources == null) return;

        if (upgradeButton != null)
            upgradeButton.interactable = resources.PlayerHasEnoughResources(building.Price) && building.BuildingLevel < 10;

        buildingName.text = "Name:" +building.BuildingName+ " | Type:" +building.Type.ToString();
        buildingLevel.text = $"Level: {building.BuildingLevel}";
        totalOutput.text = $"Output: {building.BuildingOutput}";
        upgradeCost.text = building.Price != null ? building.Price.DisplayData("Upgrade Price") : "Upgrade Price: N/A";
        refundText.text = building.Refund != null ? building.Refund.DisplayData("Refund on Destroy") : "Refund: N/A";
    }

    private void OnUpgrade()
    {
        if (_currentBuilding == null) return;
        _currentBuilding.OnUpgrade(); // building triggers event that UI listens to
    }

    private void OnDestroyBuilding()
    {
        if (_currentBuilding == null) return;
        _currentBuilding.OnDestory(); // building triggers event
    }

    private void ClearUI()
    {
        _currentBuilding = null;
        if (buildingName != null) buildingName.text = "Name: - | Type: -";
        if (buildingLevel != null) buildingLevel.text = "Level: -";
        if (totalOutput != null) totalOutput.text = "Output: -";
        if (upgradeCost != null) upgradeCost.text = "Upgrade Price: -";
        if (refundText != null) refundText.text = "Refund on Destroy: -";
    }

    void SetButtonActivity(bool active)
    {
        if (upgradeButton != null) upgradeButton.gameObject.SetActive(active);
        if (destroyButton != null) destroyButton.gameObject.SetActive(active);
    }
}
