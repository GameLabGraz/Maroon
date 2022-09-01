using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class CoinCollector : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

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
            _particleSystem.Play();
        }
    }
}
