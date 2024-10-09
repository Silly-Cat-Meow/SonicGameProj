using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpAct : PlayerAction
{
    // Animator reference
    [SerializeField] Animator animator;

    public AudioSource jump;
    // On Jump
    public void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
            Jump();
    }

    // On Enable
    void OnEnable()
    {
        PlayerPhysics.onGroundEnter += OnGroundEnter;
        PlayerPhysics.onGroundExit += OnGroundExit;
    }

    // On Disable
    void OnDisable()
    {
        PlayerPhysics.onGroundEnter -= OnGroundEnter;
        PlayerPhysics.onGroundExit -= OnGroundExit;
    }

    // OnGroundEnter
    void OnGroundEnter()
    {
        currentJumps = jumps;
        animator.SetBool("Spring", false);
        animator.SetBool("IsJumping", false); // Set IsJumping to false when grounded
    }

    // OnGroundExit
    void OnGroundExit()
    {
        animator.SetBool("IsJumping", true);

    }

    // Jump
    [SerializeField] int jumps;
    [SerializeField] float jumpForce;
    [SerializeField] float airJumpForce;

    int currentJumps;

    void Jump()
    {
        if (currentJumps <= 0) return;

        currentJumps--;

        float jumpForce = groundInfo.ground ? this.jumpForce : airJumpForce;

        rb.velocity = (groundInfo.normal * jumpForce) + PlayerPhysics.horizontalVelocity;

        jump.Play();
    }

    // Update method to continuously check jumping state
    void Update()
    {
        // Check if the player is in the air based on your ground detection logic
        if (!groundInfo.ground) // Adjust based on your ground detection logic
        {
            animator.SetBool("IsJumping", true);
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }
    }
}
