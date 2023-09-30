using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    
    void Awake() {
        instance = this;
    }
    
    [Header ("Enemy Variables")]
    public float enemyFireRate = 0.5f;
    public float GetEnemyFireRate() { return enemyFireRate; }
    public int enemyMaxShots = 5;
    public int GetEnemyMaxShots() { return enemyMaxShots; }
    public float enemyMoveSpeed = 1.0f;
    public float GetEnemyMoveSpeed() { return enemyMoveSpeed; }
    public float enemyPathfindingTick = 0.25f;
    public float GetEnemyPathfindingTick() { return enemyPathfindingTick; }
    public float enemyPauseDuration = 1.0f;
    public float GetEnemyPauseDuration() { return enemyPauseDuration; }
    public float enemyProjectileSpeed = 10.0f;
    public float GetEnemyProjectileSpeed() { return enemyProjectileSpeed; }
    public float enemyScatterMoveMaxTime = 10.0f;
    public float GetEnemyScatterMoveMaxTime() { return enemyScatterMoveMaxTime; }
    public float enemyScatterSpeed = 4;
    public float GetEnemyScatterSpeed() { return enemyScatterSpeed; }
    public float enemySightDistance = 5.0f;
    public float GetEnemySightDistance() { return enemySightDistance; }
    public float enemySphereCastHeight = 0.5f;
    public float GetEnemySphereCastHeight() { return enemySphereCastHeight; }
    public float enemySphereCastRadius = 0.3f;
    public float GetEnemySphereCastRadius() { return enemySphereCastRadius; }
    
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
    public float playerRollTime = 1.0f;
    public float GetPlayerRollTime() { return playerRollTime; }
    public float playerRollSpeed = 10.0f;
    public float GetPlayerRollSpeed() { return playerRollSpeed; }

    [Header ("World Variables")]
    public LayerMask worldEntityMask;
    public LayerMask GetWorldEntityMask() { return worldEntityMask; }
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
