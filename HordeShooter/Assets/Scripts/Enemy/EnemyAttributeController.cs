using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttributeController : MonoBehaviour
{
    private int hitPoints;
    public int GetHitPoints() { return hitPoints; }
    private int defense;
    public int GetDefense() { return defense; }
    
    // Start is called before the first frame update
    void Start()
    {
        hitPoints = GameManager.instance.GetEnemyBaseHitPoints();
        defense = GameManager.instance.GetEnemyBaseDefense();
    }
}
