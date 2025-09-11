using UnityEngine;

public class Door : Interactable
{
    [SerializeField] float cooldown = 1f;
    float cooldownTimer = 0f;
    [SerializeField] Animator doorAnimator;

    private void Update()
    {
        cooldownTimer = Mathf.Max(0, cooldownTimer - Time.deltaTime);
    }
    public override void Interact()
    {
        if(cooldownTimer > 0f)
        {
            return;
        }
        doorAnimator.SetTrigger("DoorTrigger");
        cooldownTimer = cooldown;
    }

}
