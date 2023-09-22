using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private PlayerAnimationController playerAnimationController;
    
    private bool isFiring = false;
    private float fireCooldownStartTime = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        playerAnimationController = GetComponent<PlayerAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessFire();

        HandleFire();
    }

    // Handle the fire input.
    private void HandleFire() {
        if (!isFiring) {
            return;
        }

        if (Time.time - fireCooldownStartTime < GameManager.instance.GetPlayerFireRate()) {
            return;
        }

        Debug.Log("Fire");
        playerAnimationController.CreateMuzzleFlare();

        fireCooldownStartTime = Time.time;
    }

    // Process fire input
    private void ProcessFire() {
        isFiring = InputManager.instance.GetLeftMouseDown();
        if (!isFiring && fireCooldownStartTime != 0) {
            fireCooldownStartTime = 0;
        }
    }
}
