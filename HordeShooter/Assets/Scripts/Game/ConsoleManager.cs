using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager instance = null;
    void Awake() {
        instance = this;
    }
    
    private GameObject consoleObject = null;
    public bool IsConsoleOpen() { return consoleObject != null; }
    private InputField inputField = null;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessConsoleInput();
    }

    // Check if the player is inactive.
    private bool IsPlayerInactive() {
        bool playerInactive = true;
        GameObject player = GameObject.FindGameObjectWithTag(Constants.tagPlayer);

        // Check movement.
        if (player.GetComponent<PlayerMovementController>().IsMoving()) {
            playerInactive = false;
        }

        // Check active reload.
        if (player.GetComponent<PlayerAttackController>().IsReloading()) {
            playerInactive = false;
        }

        // Check firing.
        if (player.GetComponent<PlayerAttackController>().IsFiring()) {
            playerInactive = false;
        }

        return playerInactive;
    }

    // Create the console prefab.
    private void ToggleConsole() {
        if (consoleObject == null) {
            consoleObject = Instantiate(
                PrefabManager.instance.GetPrefab(Constants.gameObjectConsole),
                GameObject.FindGameObjectWithTag(Constants.tagCanvas).transform
            );
            inputField = consoleObject.transform.Find(Constants.gameObjectBackground).Find(Constants.gameObjectInputField).GetComponent<InputField>();
            inputField.Select();
            inputField.ActivateInputField();
            return;
        }
        if (consoleObject != null) {
            Destroy(consoleObject);
            inputField = null;
            return;
        }
    }

    // Process console input.
    private void ProcessConsoleInput() {
        // Button must be pressed and player must be inactive.
        if (InputManager.instance.GetConsoleButtonDown()) {
            if (!IsPlayerInactive()) {
                return;
            }
            
            ToggleConsole();
        }
    }
}
