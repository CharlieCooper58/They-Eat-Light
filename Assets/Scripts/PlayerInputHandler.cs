using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerInputHandler : MonoBehaviour
{
    public PlayerControls playerControls;
    PlayerLocomotion playerLocomotion;
    //[Header("Character Input Values")]
    public Vector2 move { get; private set; }
    public Vector2 look { get; private set; }
    public bool jump { get; private set; }
    public bool sprint { get; private set; }
    public bool crouch { get; private set; }

    public bool scan { get; private set; }
    Scanner _scanner;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;
    private void Awake()
    {
        _scanner = GetComponentInChildren<Scanner>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }
    private void OnEnable()
    {
        SetCursorState(cursorLocked);
        if(playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.Enable();
            playerControls.Player.Move.performed += x => move = x.ReadValue<Vector2>();
            playerControls.Player.Look.performed += x => look = x.ReadValue<Vector2>();
            playerControls.Player.Jump.performed += x => Jump();
            playerControls.Player.Sprint.performed += x => Sprint();
            playerControls.Player.Scan.performed += x => Scan();
            playerControls.Player.ChangeScanSpread.performed += x => ChangeScanSpread(x.ReadValue<Vector2>());
            playerControls.Player.Crouch.performed += x => ToggleCrouch();
        }
        else
        {
            playerControls.Enable();

        }            
    }

    private void Jump()
    {
        jump = playerControls.Player.Jump.IsPressed();
    }
    private void Sprint()
    {
        sprint = playerControls.Player.Sprint.IsPressed();
    }
    private void Scan()
    {
        scan = playerControls.Player.Scan.IsPressed();
    }
    private void ToggleCrouch()
    {
        crouch = playerControls.Player.Crouch.IsPressed();
    }

    private void ChangeScanSpread(Vector2 scanChangeInput)
    {
        _scanner.ChangeSpread(scanChangeInput.y);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

