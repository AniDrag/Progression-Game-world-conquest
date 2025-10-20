using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private Building currentBuilding;

    private void Start()
    {
        ClearUI();
    }
    private void OnEnable()
    {
        EventBus<OnBuildingSelected>.OnEvent += OnBuildingSelected;
        EventBus<OnBuildingUpgraded>.OnEvent += OnBuildingUpgraded;
        EventBus<OnBuildingDestroyed>.OnEvent += OnBuildingDestroyed;
        EventBus<OnBuildPlotSelected>.OnEvent += OnBuildBuildingSelected;

        upgradeButton.onClick.AddListener(OnUpgrade);
        destroyButton.onClick.AddListener(OnDestroyBuilding);
    }

    private void OnDisable()
    {
        EventBus<OnBuildingSelected>.OnEvent -= OnBuildingSelected;
        EventBus<OnBuildingUpgraded>.OnEvent -= OnBuildingUpgraded;
        EventBus<OnBuildingDestroyed>.OnEvent -= OnBuildingDestroyed;

        upgradeButton.onClick.RemoveListener(OnUpgrade);
        destroyButton.onClick.RemoveListener(OnDestroyBuilding);
    }

    private void OnBuildingSelected(OnBuildingSelected e)
    {
        SetButtonActivity(true);
        UpdateInfoTable(e.building);
    }
    private void OnBuildBuildingSelected(OnBuildPlotSelected e)
    {
        SetButtonActivity(false);
        currentBuilding = null;
        buildingName.text = "";
        buildingLevel.text = "";
        totalOutput.text = "";
        upgradeCost.text = "";
        refundText.text = "";

        buildingName.text = $"Efficiencies\n Mine: {e.MineEfficiency} \n Plantation: {e.PlantationEfficiency}\n Gold: {e.GoldEfficiency}";
    }

    private void OnBuildingUpgraded(OnBuildingUpgraded e)
    {
        if (e.building == currentBuilding)
            UpdateInfoTable(e.building);
    }

    private void OnBuildingDestroyed(OnBuildingDestroyed e)
    {
        if (e.building == currentBuilding)
            ClearUI();
    }

    protected void UpdateInfoTable(Building building)
    {
        currentBuilding = building;

        var resources = GameManager.instance.resourceStorage.resources;
        upgradeButton.interactable = resources.IsBiggerThan(building.Price) && building.BuildingLevel < 10;

        buildingName.text = building.Type.ToString();
        buildingLevel.text = $"Level: {building.BuildingLevel}";
        totalOutput.text = $"Output: {building.BuildingOutput}";
        upgradeCost.text = building.Price.DisplayData("Upgrade Price");
        refundText.text = building.Refund.DisplayData("Refund on Destroy");
    }

    private void OnUpgrade()
    {
        if (currentBuilding == null) return;
        currentBuilding.OnUpgrade(); // triggers event
    }

    private void OnDestroyBuilding()
    {
        if (currentBuilding == null) return;
        currentBuilding.OnDestory(); // triggers event
    }

    private void ClearUI()
    {
        currentBuilding = null;
        buildingName.text = "Type: -";
        buildingLevel.text = "Level: -";
        totalOutput.text = "Output: -";
        upgradeCost.text = "Upgrade Price: -";
        refundText.text = "Refund on Destroy: -";
    }

    void SetButtonActivity(bool active)
    {
        upgradeButton.gameObject.SetActive(active);
        destroyButton.gameObject.SetActive(active);
    }
}
