using TMPro;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    [SerializeField] ResourcesCore resources;
    [SerializeField] private TMP_Text MetalResource;
    [SerializeField] private TMP_Text OrganicsResource;
    [SerializeField] private TMP_Text GoldResource;

    private void Start()
    {
        EventBus<OnBuildingUpgraded>.OnEvent += OnBuildingUpgraded;
        EventBus<OnBuildingDestroyed>.OnEvent += OnBuildingDestroyed;
        EventBus<OnTurnEnd>.OnEvent += OnTurnEnd;
        UpdateUI();
    }
    private void OnBuildingUpgraded(OnBuildingUpgraded e)
    {
        resources.Subtract(e.building.Price);
        UpdateUI();
    }

    private void OnBuildingDestroyed(OnBuildingDestroyed e)
    {
        resources.Add(e.building.Refund);
        UpdateUI();
    }
    private void OnTurnEnd(OnTurnEnd e)
    {
        UpdateUI();
    }
    public void UpdateUI()
    {
        MetalResource.text = $"Metal: {resources.Metal}" ;
        OrganicsResource.text = $"Organics: {resources.Organics}";
        GoldResource.text = $"Gold: {resources.Gold}";
    }
}
