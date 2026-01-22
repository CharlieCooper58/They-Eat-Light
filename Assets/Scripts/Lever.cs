using UnityEngine;

public class Lever : Interactable
{
    Animator leverAnimator;
    bool pulled;

    protected virtual void Awake()
    {
        leverAnimator = GetComponent<Animator>();
    }
    public override void Interact()
    {
        base.Interact();
        FlipLever(!pulled);
    }
    protected virtual void FlipLever(bool leverIsPulled)
    {
        leverAnimator.SetBool("LeverIsPulled", leverIsPulled);
        pulled = leverIsPulled;
    }
}
