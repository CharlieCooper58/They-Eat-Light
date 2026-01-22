using UnityEngine;

public class TrainThrottleLever : Lever
{
    [SerializeField] bool keyInIgnition;
    [SerializeField] AK.Wwise.Event leverStuckEvent;
    [SerializeField] AK.Wwise.Event defaultEvent;

    protected override void Awake()
    {
        base.Awake();
        base.onInteractEvent = leverStuckEvent;
    }
    protected override void FlipLever(bool leverIsPulled)
    {
        if (!keyInIgnition)
        {
            return;
        }
        base.FlipLever(leverIsPulled);

    }
}
