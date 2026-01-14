using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LookAtTrigger : MonoBehaviour
{
    [SerializeField] MultiAimConstraint constraint;
    [SerializeField] float constraintLerpSpeed = 5f;
    bool isLooking;
    private void Update()
    {
        if(isLooking && constraint.weight < 1)
        {
            constraint.weight += constraintLerpSpeed * Time.deltaTime;
        }
        else if(!isLooking && constraint.weight > 0)
        {
            constraint.weight -= constraintLerpSpeed * Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerManager player))
        {
            isLooking = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerManager player))
        {
            isLooking = false;

        }
    }
}
