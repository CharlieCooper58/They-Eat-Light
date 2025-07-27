using System.Collections.Generic;
using UnityEngine;

public class SpawnablesManager : MonoBehaviour
{
    const float basePickupRadius = 4.5f;
    public float effectiveSquaredPickupRadius { get; private set; }

    Dictionary<Spawnable, Queue<Spawnable>> spawnablesPools;

    // Spawnables manager is dont destroy on load, so I need a separate object to parent new things to so that they get cleaned up when the scene is cleaned up; else, we start getting some really weird behaviors with the stragglers
    Transform nonDDOLTransform;

    // Unit circle stuff
    public Vector2[] unitCirclePoints;
    public Quaternion[] rotations;
    int unitCircleIndex;
    int rotationIndex;

    private void Awake()
    {
        effectiveSquaredPickupRadius = basePickupRadius * basePickupRadius;

        spawnablesPools = new Dictionary<Spawnable, Queue<Spawnable>>();
    }
    private void Start()
    {
        unitCircleIndex = Random.Range(0, 999);
        rotationIndex = Random.Range(0, 99999);
    }
    public void Reset()
    {
        nonDDOLTransform = new GameObject("Non DDOL Transform").transform;
    }

    public Spawnable SpawnEntity(Spawnable spawn, Vector3 position, Quaternion rotation)
    {
        if (!spawnablesPools.ContainsKey(spawn))
        {
            spawnablesPools.Add(spawn, new Queue<Spawnable>());
        }
        Spawnable newSpawn;
        if (spawnablesPools[spawn].Count > 0)
        {
            newSpawn = spawnablesPools[spawn].Dequeue();
            if (nonDDOLTransform == null)
                nonDDOLTransform = new GameObject("Non DDOL Transform").transform;

            newSpawn.transform.parent = nonDDOLTransform;
            newSpawn.gameObject.SetActive(true);
        }
        else
        {
            newSpawn = Instantiate(spawn, position, rotation);
        }
        newSpawn.Initialize(position, rotation, spawn);
        return newSpawn;
    }
    public Spawnable SpawnEntityAsChildOfTransform(Spawnable spawn, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (!spawnablesPools.ContainsKey(spawn))
        {
            spawnablesPools.Add(spawn, new Queue<Spawnable>());
        }
        Spawnable newSpawn;
        if (spawnablesPools[spawn].Count > 0)
        {
            newSpawn = spawnablesPools[spawn].Dequeue();
            newSpawn.gameObject.SetActive(true);
            newSpawn.transform.parent = parent;
        }
        else
        {
            newSpawn = Instantiate(spawn, parent);
        }
        newSpawn.Initialize(position, rotation, spawn);
        return newSpawn;
    }
    public void RequeueSpawnable(Spawnable sp)
    {
        spawnablesPools[sp.spawnableReference].Enqueue(sp);
        sp.transform.parent = transform;
        sp.gameObject.SetActive(false);
    }

    public Vector2 GetUnitCirclePoint()
    {
        unitCircleIndex = (unitCircleIndex + 1) % 1000;
        return unitCirclePoints[unitCircleIndex];
    }
    public Vector3 GetUnitCirclePointVector3()
    {
        unitCircleIndex = (unitCircleIndex + 1) % 1000;
        return unitCirclePoints[unitCircleIndex];
    }

}