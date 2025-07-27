using UnityEngine;

public class ProceduralLegAnimator : MonoBehaviour
{
    [SerializeField] Transform raycastOrigin;
    [SerializeField] Vector3 raycastResult;


    [SerializeField] Transform footIKTarget;

    Vector3 footIKTargetPosition;

    Vector3 footOffset;
    float footDistance;

    // Used to calculate body y position so that legs are able to stay on the ground
    [SerializeField] float legLength;
    // This is how far we're ok with having our leg be from where it's supposed to be
    [SerializeField] float legBackDistance;
    [SerializeField] float stepDistance;
    [SerializeField] float legIKLerpSpeed;

    Vector3 moveDirection;

    private void Start()
    {
        footIKTargetPosition = footIKTarget.position;
    }
    public void Animate(Vector3 direction)
    {
        moveDirection = direction;
        float moveMagnitude = moveDirection.magnitude;
        moveDirection /= moveMagnitude;
        Physics.Raycast(raycastOrigin.position, Vector3.down * 10f, out RaycastHit footRaycastHit);
        raycastResult = footRaycastHit.point;

        footOffset = raycastResult - footIKTargetPosition;
        footDistance = Vector3.SqrMagnitude(footOffset);
        if ((footDistance > legBackDistance * legBackDistance && Vector3.Dot(footOffset, moveDirection) > 0))
        {
            footIKTargetPosition = raycastResult + stepDistance * moveDirection;
        }
        footIKTarget.position = Vector3.Lerp(footIKTarget.position, footIKTargetPosition, legIKLerpSpeed * moveMagnitude);
    }

    public float GetNeededBodyOffset()
    {
        float actualFootDistance = Vector3.SqrMagnitude(footIKTarget.position - raycastResult);
        float heightNeeded = legLength - Mathf.Sqrt(legLength * legLength - actualFootDistance);
        return heightNeeded;
    }
}
