using UnityEngine;

public class Door : Interactable
{
    [SerializeField] Transform pivot;
    [SerializeField] float speed;
    [SerializeField] float baseAngle = 0f;
    [SerializeField] float openAngle = 100f;
    float targetAngle;
    [SerializeField] bool closesAutomatically;
    float automaticCloseTimer;
    [SerializeField] float automaticCloseTime;
    bool isOpening;
    bool canInteract;
    [SerializeField] Animator doorAnimator;
    float yEuler;

    private void Start()
    {
        if(pivot == null)
        {
            pivot = transform;
        }
        yEuler = pivot.transform.localEulerAngles.y;
    }

    private void Update()
    {
        float angleDifference = targetAngle - transform.localEulerAngles.y;
        canInteract = (angleDifference == 0);
        if (!canInteract)
        {
            yEuler += angleDifference / Mathf.Abs(angleDifference) * Mathf.Min(Mathf.Abs(angleDifference), speed * Time.deltaTime);
            pivot.transform.localRotation = Quaternion.Euler(pivot.transform.localEulerAngles.x, yEuler, pivot.transform.localEulerAngles.z);
        }
    }
    public override void Interact()
    {
        if (canInteract)
        {
            base.Interact();
            SetIsOpening(!isOpening);
            if (isOpening && closesAutomatically)
            {
                TimerManager.SetTimer(automaticCloseTime).OnTimerEnd += AutomaticallyClose;
            }
        }

    }

    private void AutomaticallyClose()
    {
        Debug.Log("Timer off!");
        if(isOpening && canInteract) SetIsOpening(false);
        PostSound();
    }
    private void SetIsOpening(bool shouldBeOpen)
    {
        if (shouldBeOpen)
        {
            targetAngle = openAngle;
        }
        else
        {
            targetAngle = baseAngle;
        }
        isOpening = shouldBeOpen;
    }
    protected override void PostSound()
    {
        if(canInteract)
            base.PostSound();
    }
}
