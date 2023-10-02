using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributeController : MonoBehaviour
{
    private int attack;
    public int GetAttack() { return attack; }
    private int penetration;
    public int GetPenetration() { return penetration; }
    
    // Start is called before the first frame update
    void Start()
    {
        attack = GameManager.instance.GetPlayerBaseAttack();
        penetration = GameManager.instance.GetPlayerBasePenetration();
    }
}
