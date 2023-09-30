using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviorController : MonoBehaviour
{
    private EnemyAttackController enemyAttackController;
    private EnemyMovementController enemyMovementController;
    
    private enum EnemyBehaviorState {
        Attack,
        Move,
        Scatter
    }
    private EnemyBehaviorState enemyBehaviorState = EnemyBehaviorState.Move;

    private enum ScatterState {
        Move,
        Select
    }
    private ScatterState scatterState = ScatterState.Select;
    private float scatterMoveDuration = 0;
    private int scatterDirectionCount = 0;

    private float pauseDuration = 0;

    private int numberOfShots = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyAttackController = GetComponent<EnemyAttackController>();
        enemyMovementController = GetComponent<EnemyMovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessEnemyBehavior();
    }

    // Process enemy behavior loop.
    private void ProcessEnemyBehavior() {
        pauseDuration = pauseDuration > 0 ? pauseDuration - Time.deltaTime : 0;
        if (pauseDuration > 0) {
            return;
        }
        
        // Move state
        if (enemyBehaviorState.Equals(EnemyBehaviorState.Move)) {
            HandleMove();
            return;
        }

        // Attack state
        if (enemyBehaviorState.Equals(EnemyBehaviorState.Attack)) {
            HandleAttack();
            return;
        }

        // Scatter state
        if (enemyBehaviorState.Equals(EnemyBehaviorState.Scatter)) {
            HandleScatter();
            return;
        }
    }

    // Get tile below player.
    public GameObject GetTileBelowSelf() {
        // Raycast down.
        GameObject tileDown = null;
        Vector3 raycastPosition = transform.position;
        raycastPosition.y = GameManager.instance.GetEnemySphereCastHeight();
        Vector3 raycastDirection = Vector3.down;
        RaycastHit hit;
        if (Physics.Raycast(raycastPosition, raycastDirection, out hit, 5)) {
            if (hit.collider.gameObject.CompareTag(Constants.tagTile)) {
                tileDown = hit.collider.gameObject;
            }
        }

        // Return tile.
        return tileDown;
    }

    // Handle attack logic.
    private void HandleAttack() {
        // Get the number of shots the enemy will fire.
        if (numberOfShots == 0) {
            numberOfShots = Random.Range(1, GameManager.instance.GetEnemyMaxShots() + 1);
        }

        // Execute attack.
        enemyAttackController.AttackPlayer();
        numberOfShots--;
        if (numberOfShots > 0) {
            pauseDuration = GameManager.instance.GetEnemyFireRate();
        } else {
            enemyBehaviorState = EnemyBehaviorState.Scatter;
            pauseDuration = GameManager.instance.GetEnemyPauseDuration();
        }
    }

    // Handle move logic.
    private void HandleMove() {
        // Check for a sight line to the player.
        bool lineOfSight = IsPlayerVisible();

        // Switch state to attack if sight line is established.
        if (lineOfSight) {
            enemyBehaviorState = EnemyBehaviorState.Attack;
            pauseDuration = GameManager.instance.GetEnemyPauseDuration();
            return;
        }

        // Move to the player if sight line is not established.
        enemyMovementController.MoveTowardsPlayer();
    }

    // Handle scatter logic.
    private void HandleScatter() {
        // Set number of scatter cycles.
        if (scatterDirectionCount == 0) {
            scatterDirectionCount = Random.Range(1, 4);
        }
        
        // Select trajectory.
        if (scatterState.Equals(ScatterState.Select)) {
            // Get random trajectory.
            Vector3 trajectory = new Vector3(
                Random.Range(-10.0f, 10.0f),
                0,
                Random.Range(-10.0f, 10.0f)
            ).normalized;

            // Set target trajectory in movement controller.
            enemyMovementController.SetScatterDirection(trajectory);

            scatterMoveDuration = Random.Range(0.0f, GameManager.instance.GetEnemyScatterMoveMaxTime());
            scatterState = ScatterState.Move;
            return;
        }

        // Move the enemy.
        if (scatterState.Equals(ScatterState.Move)) {
            if (scatterMoveDuration <= 0) {
                scatterMoveDuration = 0;
                scatterState = ScatterState.Select;
                scatterDirectionCount--;

                // If on the last cycle, change state back to move.
                if (scatterDirectionCount == 0) {
                    enemyBehaviorState = EnemyBehaviorState.Move;
                    pauseDuration = GameManager.instance.GetEnemyPauseDuration();
                }
                return;
            }
            scatterMoveDuration -= Time.deltaTime;

            // Move the enemy.
            enemyMovementController.ScatterMove();
            return;
        }
    }

    // Check if the player is visible.
    private bool IsPlayerVisible() {
        bool playerInSight = false;
        
        // Raycast to the player.
        Vector3 raycastPosition = transform.position;
        raycastPosition.y = GameManager.instance.GetEnemySphereCastHeight();
        float radius = GameManager.instance.GetEnemySphereCastRadius();
        Vector3 playerRaycastPosition = GameObject.FindGameObjectWithTag(Constants.tagPlayer).transform.position;
        playerRaycastPosition.y = GameManager.instance.GetEnemySphereCastHeight();
        Vector3 sphereCastDirection = playerRaycastPosition - raycastPosition;
        RaycastHit sphereCastHit;
        if (Physics.Raycast(raycastPosition, sphereCastDirection, out sphereCastHit, GameManager.instance.GetEnemySightDistance())) {
            if (sphereCastHit.collider.gameObject.CompareTag(Constants.tagPlayer)) {
                playerInSight = true;
            }
        }

        return playerInSight;
    }
}
