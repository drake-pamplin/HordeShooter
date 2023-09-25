using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    PlayerAnimationController playerAnimationController;
    
    private CharacterController controller;
    private int horizontalInput = 0;
    private int verticalInput = 0;
    private bool playerGrounded = true;
    private Vector3 playerVelocity = Vector3.zero;
    private bool rollQueued = false;
    private bool isRolling = false;
    private float rollStartTime = 0;
    private Vector2 rollInput = Vector2.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        playerAnimationController = GetComponent<PlayerAnimationController>();
        
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();

        HandleInput();
        HandleRoll();
    }

    // Handle input.
    private void HandleInput() {
        // Cease vertical movement if grounded.
        playerGrounded = controller.isGrounded;
        if (playerGrounded && playerVelocity.y < 0) {
            playerVelocity.y = 0;
        }

        // Process move input and move player.
        float movementSpeed = GameManager.instance.GetPlayerMovementSpeed();
        Vector3 moveVector = new Vector3(horizontalInput, 0, verticalInput);
        if (isRolling) {
            movementSpeed = GameManager.instance.GetPlayerRollSpeed();
            moveVector = new Vector3(rollInput.x, 0, rollInput.y);
        }
        controller.Move(moveVector * Time.deltaTime * movementSpeed);

        // Process gravity.
        playerVelocity.y += GameManager.instance.GetWorldGravity() * -1 * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    // Handle roll input.
    private void HandleRoll() {
        // Do not roll if flag is not set.
        if (!isRolling) {
            return;
        }

        // Start roll.
        if (rollStartTime == 0) {
            // Set roll timer.
            rollStartTime = Time.time;

            // Gather roll input.
            rollInput = new Vector2(InputManager.instance.GetHorizontalInput(), InputManager.instance.GetVerticalInput());
        }

        // Roll no action is needed while in the roll.
        if (Time.time - rollStartTime < GameManager.instance.GetPlayerRollTime()) {
            return;
        }

        // Pause reload if active.

        isRolling = false;
        rollStartTime = 0;
    }

    // Check if player is moving.
    public bool IsMoving() {
        return horizontalInput != 0 || verticalInput != 0;
    }

    // Check if player is rolling.
    public bool IsRolling() {
        return isRolling;
    }

    // Process input from InputManager.
    private void ProcessInput() {
        horizontalInput = InputManager.instance.GetHorizontalInput();
        verticalInput = InputManager.instance.GetVerticalInput();

        rollQueued = InputManager.instance.GetRollInput();
        if (rollQueued && !isRolling) {
            isRolling = true;
        }
    }
}
