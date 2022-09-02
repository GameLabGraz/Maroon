using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class CoinCollector : MonoBehaviour
{
    [SerializeField] private UnityEvent onCoinCollect;

    private Interactable _interactableCoin;

    private void Start()
    {
        _interactableCoin = GetComponent<Interactable>();
    }

    protected virtual void HandHoverUpdate(Hand hand)
    {
        var startingGrabType = hand.GetGrabStarting();

        if (_interactableCoin.attachedToHand == null 
            && startingGrabType != GrabTypes.None)
        {
            gameObject.SetActive(false);
            onCoinCollect?.Invoke();
        }
    }
}
