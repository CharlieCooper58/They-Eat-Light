using UnityEngine;

public class DoorsOpenLever : Lever
{
    DoorsController dc;
    protected override void Awake()
    {
        base.Awake();
        dc = transform.root.GetComponentInChildren<DoorsController>();
    }
    protected override void FlipLever(bool leverIsPulled)
    {
        base.FlipLever(leverIsPulled);
        dc.UpdateAllDoors(leverIsPulled);
    }
}
