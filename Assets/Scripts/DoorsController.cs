using UnityEngine;

public class DoorsController : MonoBehaviour
{
    SlidingTrainDoor[] doors;
    private void Awake()
    {
        doors = transform.root.GetComponentsInChildren<SlidingTrainDoor>();
    }
    public void UpdateAllDoors(bool isOpen) 
    { 
        foreach(SlidingTrainDoor d  in doors) 
        {
            d.SetDoorsStatus(isOpen);
        }
    }
}
