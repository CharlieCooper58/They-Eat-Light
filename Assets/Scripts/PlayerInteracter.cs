using UnityEngine;

public class PlayerInteracter : MonoBehaviour
{
    [SerializeField] float interactRange;
    [SerializeField] LayerMask interactMask;
    public void TryInteract()
    {
        Vector3 interactPosition = Camera.main.transform.position;
        Vector3 forwardDirection = Camera.main.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(interactPosition, forwardDirection, out hit, interactRange, interactMask))
        {
            if(hit.collider.TryGetComponent(out Interactable interactable))
            {
                interactable.Interact();
            }
            

        }
    }
}
