using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    
    void Awake() {
        instance = this;
    }
    
    [Header ("Player Variables")]
    public float playerMovmentSpeed = 1.0f;
    public float GetPlayerMovementSpeed() { return playerMovmentSpeed; }

    [Header ("World Variables")]
    public float worldGravity = 9.8f;
    public float GetWorldGravity() { return worldGravity; }
    
    // Start is called before the first frame update
    void Start()
    {
        MapManager.instance.LoadMap(Constants.mapBase);
        MapManager.instance.BuildMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
