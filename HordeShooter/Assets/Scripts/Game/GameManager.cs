using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    
    void Awake() {
        instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        MapManager.instance.LoadMap(Constants.mapBase);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
