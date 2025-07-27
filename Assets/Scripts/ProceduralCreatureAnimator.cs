using UnityEngine;

public class ProceduralCreatureAnimator : MonoBehaviour
{
    [SerializeField] Transform root;
    ProceduralLegAnimator[] legs;
    Vector3 lastPosition;
    Vector3 moveDirection;

    [SerializeField] float footDepthOffset;
    float bodyOffset;
    private void Awake()
    {
        legs = GetComponentsInChildren<ProceduralLegAnimator>();
    }
    private void Start()
    {
        lastPosition = transform.position;
    }
    private void Update()
    {
        moveDirection = (transform.position - lastPosition);
        lastPosition = transform.position;
        for (int i = 0; i < legs.Length; i++)
        {
            legs[i].Animate(moveDirection);
        }

        bodyOffset = 1;
        for (int i = 0; i < legs.Length; i++)
        {
            float legDistanceNeeded = legs[i].GetNeededBodyOffset();
            bodyOffset = Mathf.Min(bodyOffset, legDistanceNeeded);
        }
        root.transform.localPosition = new Vector3(0, -bodyOffset + footDepthOffset, 0);
    }
}
