using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Usable : MonoBehaviour
{
    [Header("Action settings")]
    public string actionName;
    public ResourcesCore cost;
    public int cooldown;
    public Button activationBTN;
    [TextArea]
    public string actionDescription;

    [Header("References")]
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text price;
    [SerializeField] private TMP_Text description;

    protected bool CanBuy => GameManager.instance.playerResources.PlayerHasEnoughResources(cost);

    private int _currentCooldown;

    public virtual void OnValidate()
    {
        UpdateUsableUI();
    }
    protected virtual void Start()
    {
        activationBTN.interactable = ButtonEnableCondition();
        EventBus<OnTurnEnd>.OnEvent += ReduceCooldown;
    }
    protected virtual void OnEnable()
    {
        activationBTN.onClick.AddListener(ActivateAction);
        activationBTN.interactable = ButtonEnableCondition();
        UpdateUsableUI();
    }
    protected virtual void OnDisable()
    {
        activationBTN.onClick.RemoveListener(ActivateAction);
    }
    void ReduceCooldown(OnTurnEnd e)
    {
        if(_currentCooldown>0) _currentCooldown--;

        activationBTN.interactable = e.resource.PlayerHasEnoughResources(cost) && _currentCooldown <= 0;
    }

    void ActivateAction()
    {
        Debug.Log($"Action {actionName} used.");
        GameManager.instance.playerResources.Subtract(cost);
        activationBTN.interactable = false;
    }

    protected void UpdateUsableUI()
    {
        title.text = "Name: " + actionName;
        price.text = cost.DisplayData("Price");
        description.text = "Description:\n" + actionDescription;
    }

    protected virtual bool ButtonEnableCondition()
    {
        return CanBuy && _currentCooldown <= 0;
    }

    
}
