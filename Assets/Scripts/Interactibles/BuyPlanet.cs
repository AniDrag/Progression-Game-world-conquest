using TMPro;
using UnityEngine;

/// <summary>
/// Example of a Usable that triggers an 'end game' screen after some delay.
/// </summary>
public class BuyPlanet : Usable
{
    [Header("End Game Settings")]
    [Tooltip("Ends game in seconds")]
    [SerializeField, Range(1, 10)] private float endIn = 5f;
    [SerializeField] private TMP_Text youWonText;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (youWonText != null) youWonText.gameObject.SetActive(false);
        if (activationBTN != null) activationBTN.onClick.AddListener(Trigger);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (activationBTN != null) activationBTN.onClick.RemoveListener(Trigger);
    }

    private void Trigger()
    {
        Invoke(nameof(EndGame), endIn);
    }

    private void EndGame()
    {
        if (youWonText != null) youWonText.gameObject.SetActive(true);
    }
}
