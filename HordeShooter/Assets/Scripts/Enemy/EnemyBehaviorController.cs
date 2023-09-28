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

        // Test pathing.
        // GameObject tileBelowSelf = GetTileBelowSelf();
        // GameObject tileBelowPlayer = MapManager.instance.GetTileBelowPlayer();
        // List<Vector3> route = MapManager.instance.GetRouteBetweenPoints(tileBelowSelf, tileBelowPlayer);
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
                // Debug.Log("Enemy tile: " + tileDown.name);
            }
        }

        // Return tile.
        return tileDown;
    }

    // Handle move logic.
    private void HandleMove() {
        // Check for a sight line to the player.
        bool lineOfSight = IsPlayerVisible();

        // Switch state to attack if sight line is established.
        if (lineOfSight) {
            // enemyBehaviorState = EnemyBehaviorState.Attack;
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
