using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    
    void Awake() {
        instance = this;
    }

    private enum GameState {
        Console,
        Game
    }
    private GameState gameState = GameState.Game;
    public bool IsGameConsoleState() { return gameState.Equals(GameState.Console); }
    
    [Header ("Enemy Variables")]
    public int enemyBaseDefense = 3;
    public int GetEnemyBaseDefense() { return enemyBaseDefense; }
    public int enemyBaseHitPoints = 10;
    public int GetEnemyBaseHitPoints() { return enemyBaseHitPoints; }
    public float enemyDeathDuration = 1.0f;
    public float GetEnemyDeathDuration() { return enemyDeathDuration; }
    public float enemyFadeDuration = 2.0f;
    public float GetEnemyFadeDuration() { return enemyFadeDuration; }
    public float enemyFireRate = 0.5f;
    public float GetEnemyFireRate() { return enemyFireRate; }
    public float enemyFireDuration = 0.39f;
    public float GetEnemyFireDuration() { return enemyFireDuration; }
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
    public float enemyTargetingOffset = 1.0f;
    public float GetEnemyTargetingOffset() { return enemyTargetingOffset; }
    
    [Header ("Player Variables")]
    public float playerAccuracyMaxDeviance = 50.0f;
    public float GetPlayerAccuracyMaxDeviance() { return playerAccuracyMaxDeviance; }
    public int playerBaseAttack = 5;
    public int GetPlayerBaseAttack() { return playerBaseAttack; }
    public int playerBasePenetration = 1;
    public int GetPlayerBasePenetration() { return playerBasePenetration; }
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
        ProcessGameState();
    }

    // Process the game state.
    private void ProcessGameState() {
        GameState newGameState = GameState.Game;

        if (ConsoleManager.instance.IsConsoleOpen()) {
            newGameState = GameState.Console;
        }

        gameState = newGameState;
    }
}
