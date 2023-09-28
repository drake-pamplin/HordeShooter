using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    private EnemyBehaviorController enemyBehaviorController;

    private float pathfindingTickElapsedTime = 0;
    private CharacterController controller;
    private List<Vector3> route = new List<Vector3>();
    private int routeIndex = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyBehaviorController = GetComponent<EnemyBehaviorController>();

        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessPathfinding();
    }

    // Move towards the player.
    public void MoveTowardsPlayer() {
        if (route.Count == 0) {
            return;
        }
        
        // Check if the route index needs to be updated.
        if (routeIndex < route.Count - 1 && Vector3.Distance(transform.position, route[routeIndex]) < 0.5f) {
            routeIndex++;
        }

        // Calculate how much to move this frame and move the enemy.
        Vector3 movementVector = (route[routeIndex] - transform.position).normalized;
        controller.Move(movementVector * Time.deltaTime * GameManager.instance.GetEnemyMoveSpeed());
    }

    // Process the path to the player.
    private void ProcessPathfinding() {
        pathfindingTickElapsedTime += Time.deltaTime;
        if (pathfindingTickElapsedTime < GameManager.instance.GetEnemyPathfindingTick()) {
            return;
        }

        // Get path.
        GameObject tileBelowSelf = enemyBehaviorController.GetTileBelowSelf();
        GameObject tileBelowPlayer = MapManager.instance.GetTileBelowPlayer();
        route = MapManager.instance.GetRouteBetweenPoints(tileBelowSelf, tileBelowPlayer);
        routeIndex = 0;
        pathfindingTickElapsedTime = 0;
    }
}
