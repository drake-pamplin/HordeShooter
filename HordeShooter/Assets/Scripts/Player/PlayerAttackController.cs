using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private PlayerAnimationController playerAnimationController;
    
    private bool isFiring = false;
    private float fireCooldownStartTime = 0;
    private float fireTimer = 0;
    
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

        fireTimer += Time.deltaTime;
        if (fireTimer > GameManager.instance.GetPlayerMaxAccuracyDecayFireTime()) {
            fireTimer = GameManager.instance.GetPlayerMaxAccuracyDecayFireTime();
        }
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
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject.CompareTag(Constants.tagWall)) {
                playerAnimationController.CreateRicochet(hit);
            }
        }

        fireCooldownStartTime = Time.time;
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
}
