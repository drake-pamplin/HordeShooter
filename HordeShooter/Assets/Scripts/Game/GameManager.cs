using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    
    void Awake() {
        instance = this;
    }
    
    [Header ("Player Variables")]
    public float playerAccuracyMaxDeviance = 50.0f;
    public float GetPlayerAccuracyMaxDeviance() { return playerAccuracyMaxDeviance; }
    public float playerMaxAccuracyDecayFireTime = 3.0f;
    public float GetPlayerMaxAccuracyDecayFireTime() { return playerMaxAccuracyDecayFireTime; }
    public float playerFireRate = 0.2f;
    public float GetPlayerFireRate() { return playerFireRate; }
    public float playerMovmentSpeed = 1.0f;
    public float GetPlayerMovementSpeed() { return playerMovmentSpeed; }
    public int playerClipSize = 30;
    public int GetPlayerClipSize() { return playerClipSize; }
    public float playerReloadTime = 2.0f;
    public float GetPlayerReloadTime() { return playerReloadTime; }

    [Header ("World Variables")]
    public float worldGravity = 9.8f;
    public float GetWorldGravity() { return worldGravity; }
    
    // Start is called before the first frame update
    void Start()
    {
        MapManager.instance.LoadMap(Constants.mapBase);
        MapManager.instance.BuildMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
