using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
public class PlayerCamera : MonoBehaviour
{
    PlayerInputHandler playerInputHandler;
    [SerializeField] Transform cameraPivot;
    PlayerManager playerManager;

    [SerializeField] Transform cameraRotationHelper;
    float targetCameraXAxisRotation;

    float cameraXAngularVelocity;
    float cameraYAngularVelocity;

    [SerializeField] float cameraUpperRotationLimit;
    [SerializeField] float cameraLowerRotationLimit;

    public Vector2 cameraInput;

    public Vector3 cameraOffset;

    [SerializeField] float cameraSmoothTime;

    bool lockedcursor = true;

    public void Initialize()
    {
        playerManager = GetComponent<PlayerManager>();
        playerInputHandler = GetComponent<PlayerInputHandler>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraRotationHelper.localRotation = transform.rotation;
        FindObjectOfType<CinemachineVirtualCamera>().Follow = cameraPivot.transform;
        
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (lockedcursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                lockedcursor = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                lockedcursor = true;
            }
        }
    }

    public void UpdateCameraOrientation()
    {

        
        float lookX = (GameManager.instance.horizontalLookSensitivity * 100) * playerInputHandler.look.x * Time.deltaTime;
        float lookY = (GameManager.instance.verticalLookSensitivity * 100) * -1*playerInputHandler.look.y * Time.deltaTime;

        Vector2 lookVector = new Vector2(lookX, lookY);

        // Adjust the target camera rotation based on input
        targetCameraXAxisRotation = Mathf.Clamp(targetCameraXAxisRotation - lookVector.y, cameraLowerRotationLimit, cameraUpperRotationLimit);
        cameraRotationHelper.Rotate(Vector3.up * lookVector.x);

        // Update the camera pivot and player's transform rotation
        transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, cameraRotationHelper.localEulerAngles.y, ref cameraYAngularVelocity, cameraSmoothTime), 0);
        cameraPivot.transform.rotation = Quaternion.Euler(Mathf.SmoothDampAngle(cameraPivot.transform.localEulerAngles.x, targetCameraXAxisRotation, ref cameraXAngularVelocity, cameraSmoothTime), transform.eulerAngles.y, 0);
    }
}