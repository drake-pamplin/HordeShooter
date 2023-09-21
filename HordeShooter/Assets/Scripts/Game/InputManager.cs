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
