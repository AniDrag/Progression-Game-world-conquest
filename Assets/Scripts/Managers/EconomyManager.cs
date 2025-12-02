using TMPro;
using UnityEngine;

/// <summary>
/// Keeps resource UI in sync with the GameManager's core ResourcesCore.
/// Listens to events and updates displays accordingly.
/// </summary>
public class EconomyManager : MonoBehaviour
{
    [SerializeField] private ResourcesCore resources;
    [SerializeField] private TMP_Text metalResource;
    [SerializeField] private TMP_Text organicsResource;
    [SerializeField] private TMP_Text goldResource;

    private void Start()
    {
        // get authoritative resource reference from GameManager
        resources = GameManager.Instance?.PlayerResources;
        EventBus<OnBuildingUpgraded>.OnEvent += OnBuildingUpgraded;
        EventBus<OnBuildingDestroyed>.OnEvent += OnBuildingDestroyed;
        EventBus<OnTurnEnd>.OnEvent += OnTurnEnd;

        UpdateUI();
    }

    private void OnDestroy()
    {
        EventBus<OnBuildingUpgraded>.OnEvent -= OnBuildingUpgraded;
        EventBus<OnBuildingDestroyed>.OnEvent -= OnBuildingDestroyed;
        EventBus<OnTurnEnd>.OnEvent -= OnTurnEnd;
    }

    private void OnBuildingUpgraded(OnBuildingUpgraded e)
    {
        if (e?.building == null) return;
        // When building upgraded, cost was already subtracted by building logic.
        UpdateUI();
    }

    private void OnBuildingDestroyed(OnBuildingDestroyed e)
    {
        if (e?.building == null) return;
        UpdateUI();
    }

    private void OnTurnEnd(OnTurnEnd e)
    {
        // resources updated in GameManager; just refresh UI
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (resources == null) return;
        if (metalResource != null) metalResource.text = $"Metal: {resources.Metal}";
        if (organicsResource != null) organicsResource.text = $"Organics: {resources.Organics}";
        if (goldResource != null) goldResource.text = $"Gold: {resources.Gold}";
    }
}
