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
    
    private Dictionary<TileDirection, GameObject> neighborTiles = new Dictionary<TileDirection, GameObject>();
    public void AddTile(TileDirection direction, GameObject tile) { neighborTiles.Add(direction, tile); }
    public bool DoesTileExistInDirection(TileDirection direction) { return neighborTiles.ContainsKey(direction); }
    public Dictionary<TileDirection, GameObject> GetNeighborTiles() { return neighborTiles; }

    private bool isTraversable = false;
    public bool IsTraversable() { return isTraversable; }
    public void SetTraversable(bool isTraversable) { this.isTraversable = isTraversable; }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
