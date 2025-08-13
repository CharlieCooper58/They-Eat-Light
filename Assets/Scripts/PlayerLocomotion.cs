 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager characterManager;
    PlayerInputHandler playerInputHandler;

    [Header("Planar Movement")]
    Vector3 desiredMoveDirection;
    Vector3 planarVelocity;
    [SerializeField] float moveSpeed;
    [SerializeField] float moveLerpDelta;

    [Header("Falling and Jumping")]
    private bool isGrounded;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundCheckMask;
    [SerializeField] Transform groundCheckTransform;

    [SerializeField] private float jumpHeight;
    private float _jumpTimeoutDelta;
    [SerializeField] private float jumpTimeout;

    [SerializeField] private float footstepTimerMax;
    private float footstepTime;
    [SerializeField] AudioSource footstepSounds;
    [SerializeField] AudioClip[] footsteps;
    bool isMoving;

    Rigidbody rb;
    public virtual void Initialize()
    {
        characterManager = GetComponent<PlayerManager>();
        playerInputHandler = GetComponent<PlayerInputHandler>();
        rb = GetComponent<Rigidbody>();
    }

    public void SetMovementDirection()
    {
        desiredMoveDirection = Vector3.ProjectOnPlane(Camera.main.transform.forward * playerInputHandler.move.y + Camera.main.transform.right * playerInputHandler.move.x, Vector3.up);
        isMoving = desiredMoveDirection != Vector3.zero;
    }
    public void HandleAllMovement()
    {
        SetMovementDirection();
        CheckGrounded();
        HandleJumping();
        HandlePlanarMovement();
        HandleFootsteps();
        rb.linearVelocity = planarVelocity + Vector3.up*rb.linearVelocity.y;
    }
    private void HandlePlanarMovement()
    {
        planarVelocity = Vector3.Lerp(planarVelocity, desiredMoveDirection * moveSpeed, moveLerpDelta * Time.fixedDeltaTime);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundCheckMask);       
    }
    private void HandleJumping()
    {
        if (isGrounded)
        {
            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
            else if (playerInputHandler.jump)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpHeight, rb.linearVelocity.z);
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = jumpTimeout;
        }

    }

    private void HandleFootsteps()
    {
        if(!isMoving || !isGrounded)
        {
            footstepTime = 0.0f;
            return;
        }
        footstepTime -= Time.deltaTime;
        if(footstepTime <= 0)
        {
            footstepTime = footstepTimerMax;
            footstepSounds.clip = footsteps[Random.Range(0, footsteps.Length)];
            footstepSounds.Play();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
    }
}
