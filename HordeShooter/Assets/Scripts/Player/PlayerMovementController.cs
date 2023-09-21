using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private CharacterController controller;
    
    private float movementSpeed;
    private int horizontalInput = 0;
    private int verticalInput = 0;
    private bool playerGrounded = true;
    private Vector3 playerVelocity = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        movementSpeed = GameManager.instance.GetPlayerMovementSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();

        HandleInput();
    }

    // Handle input.
    private void HandleInput() {
        // Cease vertical movement if grounded.
        playerGrounded = controller.isGrounded;
        if (playerGrounded && playerVelocity.y < 0) {
            playerVelocity.y = 0;
        }

        // Process move input and move player.
        Vector3 moveVector = new Vector3(horizontalInput, 0, verticalInput);
        controller.Move(moveVector * Time.deltaTime * movementSpeed);

        // Process gravity.
        playerVelocity.y += GameManager.instance.GetWorldGravity() * -1 * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    // Check if player is moving.
    public bool IsMoving() {
        return horizontalInput != 0 || verticalInput != 0;
    }

    // Process input from InputManager.
    private void ProcessInput() {
        horizontalInput = InputManager.instance.GetHorizontalInput();
        verticalInput = InputManager.instance.GetVerticalInput();
    }
}
