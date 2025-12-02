using TMPro;
using UnityEngine;

/// <summary>
/// Keeps resource UI in sync with the GameManager's core ResourcesCore.
/// Listens to events and updates displays accordingly.
/// </summary>
public class EconomyManager : MonoBehaviour
{
    [SerializeField] private ResourcesCore resources;
    [SerializeField] private TMP_Text trurnCounter;
    [SerializeField] private TMP_Text metalResource;
    [SerializeField] private TMP_Text organicsResource;
    [SerializeField] private TMP_Text goldResource;

    private void Start()
    {
        // get authoritative resource reference from GameManager
        resources = GameManager.Instance?.PlayerResources;
        GameManager.Instance.UpdateResources += UpdateUI;

        UpdateUI();
    }

    private void OnDestroy()
    {
        GameManager.Instance.UpdateResources -= UpdateUI;
    }


    public void UpdateUI()
    {
        trurnCounter.text = "Turn: "+GameManager.Instance.TurnCounter.ToString();
        if (resources == null) return;
        if (metalResource != null) metalResource.text = $"Metal: {resources.Metal}";
        if (organicsResource != null) organicsResource.text = $"Organics: {resources.Organics}";
        if (goldResource != null) goldResource.text = $"Gold: {resources.Gold}";
    }
}
