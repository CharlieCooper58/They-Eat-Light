using UnityEngine;
using UnityEngine.UIElements;

public class SlidingTrainDoor : MonoBehaviour
{
    [SerializeField] Transform leftDoor;
    [SerializeField] Transform rightDoor;
    [SerializeField] float doorTime = 1;
    float doorSpeed;
    // Due to the setup of the model, we're lerping doors on the y-axis boys
    [SerializeField] float doorOpenYPos;
    [SerializeField] float doorClosedYPos;

    float currentDoorPos;
    float targetDoorPos;
    bool doorsOpen;

    private void Start()
    {
        doorSpeed = (doorOpenYPos - doorClosedYPos) / doorTime;
    }

    private void FixedUpdate()
    {
        float doorDisplacement = targetDoorPos - currentDoorPos;
        float doorDist = Mathf.Abs(doorDisplacement);
        if(doorDist > 0)
        {
            float sign = doorDisplacement / doorDist;
            currentDoorPos += sign * Mathf.Min(doorDist, doorSpeed * Time.deltaTime);
            leftDoor.transform.localPosition = new Vector3(leftDoor.transform.localPosition.x, -currentDoorPos, leftDoor.transform.localPosition.z);
            rightDoor.transform.localPosition = new Vector3(rightDoor.transform.localPosition.x, currentDoorPos, rightDoor.transform.localPosition.z);
        }
    }
    public void SetDoorsStatus(bool isOpen)
    {
        doorsOpen = isOpen;
        targetDoorPos = isOpen?doorOpenYPos : doorClosedYPos;
    }
}
