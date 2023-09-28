using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    private EnemyBehaviorController enemyBehaviorController;

    private float pathfindingTickElapsedTime = 0;
    private List<Vector3> route = new List<Vector3>();
    
    // Start is called before the first frame update
    void Start()
    {
        enemyBehaviorController = GetComponent<EnemyBehaviorController>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessPathfinding();
    }

    // Move towards the player.
    public void MoveTowardsPlayer() {
        // Calculate how much to move this frame and move the enemy.
        float step = GameManager.instance.GetEnemyMoveSpeed() * Time.deltaTime;
        if (route.Count == 0) {
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, route[1], step);
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
        pathfindingTickElapsedTime = 0;
    }
}
