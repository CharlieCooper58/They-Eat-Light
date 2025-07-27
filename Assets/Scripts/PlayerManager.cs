using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerCamera playerCamera;
    public PlayerInputHandler inputHandler;
    public PlayerLocomotion playerLocomotion;

    public bool PlayerManager_IsLocalPlayer;
    private void Awake()
    {
        playerCamera = GetComponent<PlayerCamera>();
        inputHandler = GetComponent<PlayerInputHandler>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }
    // Start is called before the first frame update
    private void Start()
    {
        playerCamera.Initialize();
        playerLocomotion.Initialize();
    }


    private void LateUpdate()
    {
        playerCamera.UpdateCameraOrientation();
    }
    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }
}
