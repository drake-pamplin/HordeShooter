using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviorController : MonoBehaviour
{
    private EnemyMovementController enemyMovementController;
    
    private enum EnemyBehaviorState {
        Attack,
        Move,
        Scatter
    }
    private EnemyBehaviorState enemyBehaviorState = EnemyBehaviorState.Move;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyMovementController = GetComponent<EnemyMovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessEnemyBehavior();
    }

    // Process enemy behavior loop.
    private void ProcessEnemyBehavior() {
        // Move state
        if (enemyBehaviorState.Equals(EnemyBehaviorState.Move)) {
            HandleMove();
        }

        // Attack state

        // Scatter state
    }

    // Handle move logic.
    private void HandleMove() {
        // Set player location in the movement controller if it is not set.
        if (!enemyMovementController.IsPlayerLocationSet()) {
            Vector3 playerLocation = GameObject.FindGameObjectWithTag(Constants.tagPlayer).transform.position;
            enemyMovementController.SetPlayerLocation(playerLocation);
        }

        // Check for a sight line to the player.
        bool lineOfSight = IsPlayerVisible();

        // Switch state to attack if sight line is established.
        if (lineOfSight) {
            enemyBehaviorState = EnemyBehaviorState.Attack;
            return;
        }

        // Move to the player if sight line is not established.
        enemyMovementController.MoveTowardsPlayer();
    }

    // Check if the player is visible.
    private bool IsPlayerVisible() {
        bool playerInSight = false;
        
        // Spherecast to the player.
        Vector3 sphereCastPosition = transform.position;
        sphereCastPosition.y = GameManager.instance.GetEnemySphereCastHeight();
        float radius = GameManager.instance.GetEnemySphereCastRadius();
        Vector3 playerSphereCastPosition = GameObject.FindGameObjectWithTag(Constants.tagPlayer).transform.position;
        playerSphereCastPosition.y = GameManager.instance.GetEnemySphereCastHeight();
        Vector3 sphereCastDirection = playerSphereCastPosition - sphereCastPosition;
        RaycastHit sphereCastHit;
        if (Physics.SphereCast(sphereCastPosition, radius, sphereCastDirection, out sphereCastHit, 100)) {
            if (sphereCastHit.collider.gameObject.CompareTag(Constants.tagPlayer)) {
                playerInSight = true;
            }
        }

        return playerInSight;
    }
}
