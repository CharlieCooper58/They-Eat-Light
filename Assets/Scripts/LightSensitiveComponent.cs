using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSensitiveComponent : MonoBehaviour
{
    public Vector3 scanDirection;
    public Vector3 scanPosition;
    // how many dots have we already received?
    [SerializeField] float irritation;

    [SerializeField, Tooltip("How many dots can we receive before we get mad?")] float irritationThreshold;

    [SerializeField, Tooltip("How fast does irritation fall off?")] float irritationFalloffRate;

    [SerializeField, Tooltip("How long before irritation starts falling off?")] float irritationMemoryTimer;
    float irritationFalloffTime;

    public void ReceiveScan(Vector3 direction, Vector3 position)
    {
        scanDirection = -direction;
        scanPosition = position;
        irritation++;
        irritationFalloffTime = Time.time + irritationMemoryTimer;
    }

    private void Update()
    {
        if(Time.time > irritationFalloffTime)
        {
            irritation -= (irritation * irritationFalloffRate * Time.deltaTime);
        }
    }

    public float GetIrritationRatio()
    {
        return irritation / irritationThreshold;
    }
}
