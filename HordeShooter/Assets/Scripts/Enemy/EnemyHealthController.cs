using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    private EnemyBehaviorController enemyBehaviorController;
    private EnemyAttributeController enemyAttributeController;

    private int hitPoints;
    private bool hitPointsSet = false;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyBehaviorController = GetComponent<EnemyBehaviorController>();
        enemyAttributeController = GetComponent<EnemyAttributeController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Register hit.
    public void RegisterHit(int playerAttack, int playerPenetration) {
        // Do not handle hit registration if already dead.
        if (enemyBehaviorController.IsDead()) {
            return;
        }
        
        if (!hitPointsSet) {
            hitPoints = enemyAttributeController.GetHitPoints();
            hitPointsSet = true;
        }
        
        int damageReduction = (enemyAttributeController.GetDefense() - playerPenetration < 0) ? 0 : (enemyAttributeController.GetDefense() - playerPenetration);
        int damage = playerAttack - damageReduction;
        Debug.Log("Hit enemy for " + damage + " damage.");
        hitPoints -= damage;

        if (hitPoints <= 0) {
            enemyBehaviorController.TriggerDeath();
        }
    }
}
