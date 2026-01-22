using UnityEngine;
using AK.Wwise;

public class Interactable : MonoBehaviour
{
    [SerializeField] protected AK.Wwise.Event onInteractEvent;

    public virtual void Interact() {
        PostSound();
    }

    protected virtual void PostSound()
    {
        onInteractEvent.Post(gameObject);
    }
}
