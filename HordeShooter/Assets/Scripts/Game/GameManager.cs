using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private enum RoundState {
        Mid,
        Post,
        Pre
    }
    private RoundState roundState = RoundState.Post;
    private bool IsRoundMidState() { return roundState.Equals(RoundState.Mid); }
    private bool IsRoundPostState() { return roundState.Equals(RoundState.Post); }
    private bool IsRoundPreState() { return roundState.Equals(RoundState.Pre); }
    
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
    public float enemySpawnTime = 1.050f;
    public float GetEnemySpawnTime() { return enemySpawnTime; }
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
    public int worldEnemyMaxEnemiesOnScreen = 25;
    public int GetWorldEnemyMaxEnemiesOnScreen() { return worldEnemyMaxEnemiesOnScreen; }
    public LayerMask worldEntityMask;
    public LayerMask GetWorldEntityMask() { return worldEntityMask; }
    public float worldGravity = 9.8f;
    public float GetWorldGravity() { return worldGravity; }
    public float worldRoundEnemySpawnTick = 0.5f;
    public float GetWorldRoundEnemySpawnTick() { return worldRoundEnemySpawnTick; }
    public float worldRoundPostDuration = 3.0f;
    public float GetWorldRoundPostDuration() { return worldRoundPostDuration; }
    public float worldRoundPreDuration = 3.0f;
    public float GetWorldRoundPreDuration() { return worldRoundPreDuration; }
    public float worldRoundTickOverTimer = 2.31f;
    public float GetWorldRoundTickOverTimer() { return worldRoundTickOverTimer; }

    private GameObject roundIndicator = null;
    private int roundEnemyCount = 0;
    private int roundEnemySpawnTickElapsed = 0;
    private int roundNumber = 0;
    private float roundPostTimeElapsed = 0;
    private float roundPreTimeElapsed = 0;
    private float roundTickOverTimeElapsed = 0;
    
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
        ProcessRoundState();
    }

    // Calculate the number of enemies for the round.
    private void CalculateRoundEnemyCount() {
        
    }

    // Execute spawn command.
    public void ExecuteSpawnCommand(string unit) {
        // Get unit designation and get unit prefab.
        GameObject prefab = null;
        if (unit.Equals(Constants.gameObjectRanged.ToLower())) {
            prefab = PrefabManager.instance.GetPrefab(Constants.gameObjectRanged);
        }

        if (prefab == null) {
            Debug.LogError("No unit prefab found.");
            return;
        }

        // Raycast to mouse location.
        GameObject spawnTile = InputManager.instance.GetTileRaycastToMouse();
        
        if (spawnTile == null) {
            Debug.LogError("Mouse not on a valid tile.");
            return;
        }

        // Call map manager to spawn a unit.
        MapManager.instance.SpawnUnitAtLocation(prefab, spawnTile);
    }

    // Process the game state.
    private void ProcessGameState() {
        GameState newGameState = GameState.Game;

        if (ConsoleManager.instance.IsConsoleOpen()) {
            newGameState = GameState.Console;
        }

        gameState = newGameState;
    }

    // Process round state.
    private void ProcessRoundState() {
        // Set the round indicator if it is not set.
        if (roundIndicator == null) {
            roundIndicator = GameObject.FindGameObjectWithTag(Constants.tagRoundIndicator);
        }
        
        // Pre
        if (IsRoundPreState()) {
            // Perform phase logic.
            if (roundPreTimeElapsed == 0) {
                // Set phase timer.
                roundPreTimeElapsed = GetWorldRoundPreDuration();
                
                // Set round tick over timer.
                roundTickOverTimeElapsed = GetWorldRoundTickOverTimer();

                // Play round indicator initialize animation.
                roundIndicator.GetComponent<Animator>().Play(Constants.animationInitialize);

                // Calculate number of enemies to appear this round.
                CalculateRoundEnemyCount();
            }
            roundPreTimeElapsed -= Time.deltaTime;
            roundTickOverTimeElapsed -= Time.deltaTime;

            // Update round number in sync with animation.
            if (roundTickOverTimeElapsed <= 0) {
                roundIndicator.transform.Find(Constants.gameObjectBackground).Find(Constants.gameObjectRoundText).GetComponent<Text>().text = roundNumber.ToString();
            }
            
            // Do nothing if timer is running down.
            if (roundPreTimeElapsed <= 0) {
                // When timer is up, default timer value and step the round state up.
                roundPreTimeElapsed = 0;
                roundState = RoundState.Mid;
            }
        }

        // Mid
        if (IsRoundMidState()) {
            // Set enemy spawn timer.
            if (roundEnemySpawnTickElapsed == 0 && roundEnemyCount > 0) {
                roundEnemySpawnTickElapsed = GetWorldRoundEnemySpawnTick();
            }
            
            // Elapse timer if there are still enemies to spawn.
            if (roundEnemyCount > 0) {
                roundEnemySpawnTickElapsed -= Time.deltaTime;
            }

            // Spawn an enemy on the map when timer elapses.
            if (roundEnemySpawnTickElapsed <= 0) {
                roundEnemySpawnTickElapsed = 0;
                MapManager.instance.SpawnUnitAtRandom();
                roundEnemyCount--;
            }
            
            // Check for enemies in play.
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.tagEnemy);

            // If no enemies exist or are queued up, end the mid state.
            if (enemies.Length == 0 && roundEnemyCount == 0) {
                roundState = RoundState.Post;
            }
            return;
        }

        // Post
        if (IsRoundPostState()) {
            // Peform phase logic.
            if (roundPostTimeElapsed == 0) {
                // Set phase timer.
                roundPostTimeElapsed = GetWorldRoundPostDuration();

                // Play round indicator end animation.
                roundIndicator.GetComponent<Animator>().Play(Constants.animationEnd);
            }
            roundPostTimeElapsed -= Time.deltaTime;

            // Do nothing if the timer is running down.
            if (roundPostTimeElapsed > 0) {
                return;
            }

            // When timer is up, default timer value and restart the round state to pre.
            roundPostTimeElapsed = 0;
            roundState = RoundState.Pre;
            roundNumber++;
            return;
        }
    }
}
