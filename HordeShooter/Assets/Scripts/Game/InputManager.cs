using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    void Awake() {
        instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Get console button down.
    public bool GetConsoleButtonPressed() {
        bool input = false;

        if (Keyboard.current.slashKey.wasPressedThisFrame) {
            input = true;
        }

        return input;
    }

    // Get console command next button down.
    public bool GetConsoleCommandNextButtonPressed() {
        bool input = false;

        if (Keyboard.current.downArrowKey.wasPressedThisFrame) {
            input = true;
        }

        return input;
    }

    // Get console command previous button down.
    public bool GetConsoleCommandPreviousPressed() {
        bool input = false;

        if (Keyboard.current.upArrowKey.wasPressedThisFrame) {
            input = true;
        }

        return input;
    }

    // Get console confirm button down.
    public bool GetConsoleConfirmButtonPressed() {
        bool input = false;

        if (Keyboard.current.enterKey.wasPressedThisFrame) {
            input = true;
        }

        return input;
    }

    // Get raycast to mouse on environment layer.
    public Vector3 GetEnvironmentRaycastToMouse() {
        Vector3 mousePosition = Vector3.zero;
        
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            mousePosition = hit.point;
        }
        
        return mousePosition;
    }

    // Get raycast to mouse on environment layer.
    public GameObject GetTileRaycastToMouse() {
        GameObject tile = null;
        
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            if (hit.collider.gameObject.CompareTag(Constants.tagTile)) {
                tile = hit.collider.gameObject;
            }
        }
        
        return tile;
    }

    // Get left mouse button down.
    public bool GetFireButtonPressed() {
        bool pressed = false;

        if (Mouse.current.leftButton.isPressed) {
            pressed = true;
        }

        return pressed;
    }

    // Get horizontal input.
    public int GetHorizontalInput() {
        int input = 0;

        if (Keyboard.current.aKey.isPressed) {
            input = -1;
        }
        if (Keyboard.current.dKey.isPressed) {
            input = 1;
        }

        return input;
    }

    // Get reload input.
    public bool GetReloadDown() {
        bool pressed = false;

        if (Keyboard.current.rKey.isPressed) {
            pressed = true;
        }

        return pressed;
    }

    // Get roll input.
    public bool GetRollInput() {
        bool pressed = false;

        if (Keyboard.current.spaceKey.isPressed) {
            pressed = true;
        }

        return pressed;
    }

    // Get vertical input.
    public int GetVerticalInput() {
        int input = 0;

        if (Keyboard.current.wKey.isPressed) {
            input = 1;
        }
        if (Keyboard.current.sKey.isPressed) {
            input = -1;
        }

        return input;
    }
}
