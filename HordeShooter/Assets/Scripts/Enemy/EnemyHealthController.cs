using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    private EnemyAttributeController enemyAttributeController;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyAttributeController = GetComponent<EnemyAttributeController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Register hit.
    public void RegisterHit(int playerAttack, int playerPenetration) {
        int damageReduction = (enemyAttributeController.GetDefense() - playerPenetration < 0) ? 0 : (enemyAttributeController.GetDefense() - playerPenetration);
        int damage = playerAttack - damageReduction;
        Debug.Log("Hit enemy for " + damage + " damage.");
    }
}
