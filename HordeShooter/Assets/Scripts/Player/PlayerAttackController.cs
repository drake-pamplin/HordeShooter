using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private PlayerAnimationController playerAnimationController;
    private PlayerMovementController playerMovementController;
    
    // Fire variables
    private bool isFiring = false;
    private float fireCooldownStartTime = 0;
    private float fireTimer = 0;

    // Ammo variables
    private int ammoInClip = 0;
    public int GetAmmoInClip() { return ammoInClip; }
    private bool reloadQueued = false;
    private bool isReloading = false;
    public bool IsReloading() { return isReloading; }
    private float reloadStartTime = 0;
    private bool isReloadPaused = false;
    public bool IsReloadPaused() { return isReloadPaused; }
    private float reloadPauseTime = 0;
    public void PauseReload() {
        reloadPauseTime = Time.time;
        isReloadPaused = true;
    }
    public void ResumeReload() {
        reloadStartTime += (Time.time - reloadPauseTime);
        reloadPauseTime = 0;
        isReloadPaused = false;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        playerAnimationController = GetComponent<PlayerAnimationController>();
        playerMovementController = GetComponent<PlayerMovementController>();

        ammoInClip = GameManager.instance.GetPlayerClipSize();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessFire();
        ProcessReload();

        HandleFire();
        HandleReload();
    }

    // Handle the fire input.
    private void HandleFire() {
        // Don't fire if the player is not firing.
        if (!isFiring) {
            return;
        }

        // Don't fire if reloading.
        if (isReloading) {
            return;
        }

        // Do not fire if rolling.
        if (playerMovementController.IsRolling()) {
            return;
        }

        // Do not fire if clip is empty.
        if (ammoInClip == 0) {
            return;
        }

        // If the player is firing, tick up the accuracy decay.
        fireTimer += Time.deltaTime;
        if (fireTimer > GameManager.instance.GetPlayerMaxAccuracyDecayFireTime()) {
            fireTimer = GameManager.instance.GetPlayerMaxAccuracyDecayFireTime();
        }
        
        // Do not fire if weapon is cycling.
        if (Time.time - fireCooldownStartTime < GameManager.instance.GetPlayerFireRate()) {
            return;
        }

        // Play muzzle flare.
        playerAnimationController.CreateMuzzleFlare();
        
        // Calculate accuracy deviance based on how long the player has been firing.
        GameObject rotationReferenceObject = transform.Find(Constants.gameObjectRotationReference).gameObject;
        Vector3 aimRotation = rotationReferenceObject.transform.rotation.eulerAngles;
        float aimDeviance = Random.Range(GameManager.instance.GetPlayerAccuracyMaxDeviance() * -1, GameManager.instance.GetPlayerAccuracyMaxDeviance());
        aimDeviance *= (fireTimer / GameManager.instance.GetPlayerMaxAccuracyDecayFireTime()) > 1 ? 1 : (fireTimer / GameManager.instance.GetPlayerMaxAccuracyDecayFireTime());
        aimRotation.y += aimDeviance;
        GameObject fireReference = transform.Find(Constants.gameObjectFireReference).gameObject;
        fireReference.transform.eulerAngles = aimRotation;

        // Raycast to create bullet hit.
        RaycastHit hit;
        Debug.DrawRay(transform.position, fireReference.transform.forward * 10, Color.red);
        if (Physics.Raycast(rotationReferenceObject.transform.position, fireReference.transform.forward, out hit, 100)) {
            playerAnimationController.CreateRicochet(hit);
        }

        // Subtract bullet from clip.
        ammoInClip--;

        fireCooldownStartTime = Time.time;
    }

    // Handle reload input.
    private void HandleReload() {
        // Don't reload if clip is full.
        if (ammoInClip == GameManager.instance.GetPlayerClipSize()) {
            return;
        }

        // Do not reload if not reloading.
        if (!isReloading) {
            return;
        }

        // Start reload if not reloading and player is attempting one.
        if (reloadStartTime == 0) {
            reloadStartTime = Time.time;
            playerAnimationController.CreateReloadIndicator();
        }

        // Don't proceed if time has not elapsed.
        if (Time.time - reloadStartTime < GameManager.instance.GetPlayerReloadTime()) {
            return;
        }

        // Do not proceed if reload is paused.
        if (isReloadPaused) {
            return;
        }

        ammoInClip = GameManager.instance.GetPlayerClipSize();
        isReloading = false;
        reloadStartTime = 0;
    }

    // Process fire input
    private void ProcessFire() {
        isFiring = InputManager.instance.GetLeftMouseDown();
        if (!isFiring) {
            if (fireCooldownStartTime != 0) {
                fireCooldownStartTime = 0;
            }
            if (fireTimer > 0) {
                fireTimer -= Time.deltaTime;
            }
            if (fireTimer < 0) {
                fireTimer = 0;
            }
        }
    }

    // Process reload input.
    private void ProcessReload() {
        reloadQueued = InputManager.instance.GetReloadDown();
        if (reloadQueued && !isReloading) {
            isReloading = true;
        }
    }
}
