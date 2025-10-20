using TMPro;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    [SerializeField] ResourceStorage resources;
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
        UpdateUI();
        resources.Remove(e.building.Price);
    }

    private void OnBuildingDestroyed(OnBuildingDestroyed e)
    {
        UpdateUI();
        resources.Add(e.building.Refund);
    }
    private void OnTurnEnd(OnTurnEnd e)
    {
        UpdateUI();
    }
    public void UpdateUI()
    {
        MetalResource.text = $"Metal: {resources.resources.Metal}" ;
        OrganicsResource.text = $"Organics: {resources.resources.Organic}";
        GoldResource.text = $"Gold: {resources.resources.Gold}";
    }
}
