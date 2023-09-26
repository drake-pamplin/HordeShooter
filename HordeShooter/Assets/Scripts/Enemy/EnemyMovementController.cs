using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    private Vector3 playerLocation = Vector3.zero;
    public void SetPlayerLocation(Vector3 playerLocation) { this.playerLocation = playerLocation; }
    public bool IsPlayerLocationSet() { return playerLocation != Vector3.zero; }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Move towards the player.
    public void MoveTowardsPlayer() {
        
    }
}
