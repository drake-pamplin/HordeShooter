using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileDirection {
        Bottom,
        Left,
        Right,
        Top
    }
    
    private Dictionary<TileDirection, GameObject> neighborTiles;
    public void AddTile(TileDirection direction, GameObject tile) { neighborTiles.Add(direction, tile); }
    public bool DoesTileExistInDirection(TileDirection direction) { return neighborTiles.ContainsKey(direction); }

    private bool isTraversable = false;
    public bool IsTraversable() { return isTraversable; }
    
    // Start is called before the first frame update
    void Start()
    {
        neighborTiles = new Dictionary<TileDirection, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
