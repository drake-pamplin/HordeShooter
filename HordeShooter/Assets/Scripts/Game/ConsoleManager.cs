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
    private List<string> inputHistory = new List<string>();
    private int inputHistoryIndex = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessConsoleInput();
    }

    // Execute a spawn command.
    private void ExecuteSpawnCommand(string[] commandParts) {
        // Validate spawn command.
        bool validated = ValidateSpawnCommand(commandParts);
        if (!validated) {
            Debug.LogError("Spawn command invalid.");
            return;
        }

        // Call GameManager to execute spawn.
        GameManager.instance.ExecuteSpawnCommand(commandParts[1]);
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

    // Parse the console input.
    private void ParseCommand() {
        // Do not parse if console is not active.
        if (!IsConsoleOpen()) {
            return;
        }
        
        string command = inputField.text;
        // Do not execute if command is empty.
        if (command.Equals("")) {
            Debug.LogError("No command entered.");
            return;
        }

        string[] commandParts = command.Split(Constants.splitCharSpace);
        // Do not execute if command parts are empty.
        if (commandParts.Length == 0) {
            Debug.LogError("No command entered.");
            return;
        }

        inputHistory.Add(command);
        inputHistoryIndex = inputHistory.Count;
        string commandPartLower = commandParts[0].ToLower();
        if (commandPartLower.Equals(Constants.commandSpawn)) {
            Debug.Log("Spawn command received.");
            ExecuteSpawnCommand(commandParts);
        } else {
            Debug.LogError("Command \"" + command + "\" not recognized.");
        }
    }

    // Process console input.
    private void ProcessConsoleInput() {
        // Console toggle button must be pressed and player must be inactive.
        if (InputManager.instance.GetConsoleButtonPressed()) {
            if (!IsPlayerInactive()) {
                return;
            }
            
            ToggleConsole();
            return;
        }

        // Execute command when console confirm button is pressed.
        if (InputManager.instance.GetConsoleConfirmButtonPressed()) {
            if (!IsConsoleOpen()) {
                return;
            }
            
            ParseCommand();
            ToggleConsole();
        }

        // Process next command input.
        if (InputManager.instance.GetConsoleCommandNextButtonPressed()) {
            if (inputHistory.Count == 0) {
                return;
            }

            if (inputHistoryIndex == inputHistory.Count) {
                return;
            }

            inputHistoryIndex++;

            if (inputHistoryIndex == inputHistory.Count) {
                inputField.text = "";
            }
            if (inputHistoryIndex < inputHistory.Count) {
                inputField.text = inputHistory[inputHistoryIndex];
            }
            return;
        }

        // Process previous command input.
        if (InputManager.instance.GetConsoleCommandPreviousPressed()) {
            if (inputHistory.Count == 0) {
                return;
            }

            if (inputHistoryIndex == 0) {
                return;
            }

            inputHistoryIndex--;
            inputField.text = inputHistory[inputHistoryIndex];
            return;
        }
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

    // Validate the spawn command entered is valid.
    private bool ValidateSpawnCommand(string[] commandParts) {
        // Length of two.
        if (commandParts.Length != 2) {
            return false;
        }

        // Second argument exists in the units list.
        if (!Constants.commandUnits.Contains(commandParts[1].ToLower())) {
            return false;
        }

        return true;
    }
}
