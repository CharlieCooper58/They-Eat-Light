using UnityEngine;

public class Spawnable : MonoBehaviour
{
    protected SpawnablesManager spawnablesManager;
    [HideInInspector] public Spawnable spawnableReference;

    protected virtual void Awake()
    {
        spawnablesManager = GameManager.instance.spawnablesManager;
    }
    public virtual void Initialize(Vector3 position, Quaternion rotation, Spawnable reference)
    {
        transform.position = position;
        transform.rotation = rotation;
        spawnableReference = reference;
    }
}