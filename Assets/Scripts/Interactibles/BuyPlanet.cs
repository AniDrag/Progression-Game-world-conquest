using UnityEngine;

public class BuyPlanet : Usable
{
    [Header(" End Game Settings")]
    [SerializeField,Range(1,10)] float endIn = 5;
    protected override void OnEnable()
    {
        base.OnEnable();
        activationBTN.onClick.AddListener(Trigger);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        activationBTN.onClick.RemoveListener(Trigger);
    }

    private void Trigger() => Invoke(nameof(EndGame), endIn);
    void EndGame()
    {
        Application.Quit();
    }
}
