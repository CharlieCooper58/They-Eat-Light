using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public SoundEffectManager soundManager;
    public SpawnablesManager spawnablesManager;

    public static int playerLayer;
    public static int scanLayer;
    public static int scanColliderLayer;
    public static int creatureScansLayer;

    public float horizontalLookSensitivity;
    public float verticalLookSensitivity;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        playerLayer = LayerMask.NameToLayer("Player");
        scanLayer = LayerMask.NameToLayer("Scan");
        scanColliderLayer = LayerMask.NameToLayer("Scan Visual Colliders");
        creatureScansLayer = LayerMask.NameToLayer("Creature Scans");
        soundManager = GetComponentInChildren<SoundEffectManager>();
        spawnablesManager = GetComponentInChildren<SpawnablesManager>();
    }
    public static void SetGameObjectAndChildrenLayer(GameObject gameObject, int layer)
    {
        if (gameObject == null)
        {
            return;
        }
        gameObject.layer = layer;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            SetGameObjectAndChildrenLayer(gameObject.transform.GetChild(i).gameObject, layer);
        }
    }
}
