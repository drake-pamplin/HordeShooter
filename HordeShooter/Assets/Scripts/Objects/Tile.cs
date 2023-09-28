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
    public GameObject GetTileInDirection(TileDirection direction) {
        GameObject tile = null;
        neighborTiles.TryGetValue(direction, out tile);
        return tile;
    }

    private int tileIndex = 0;
    public int GetTileIndex() { return tileIndex; }
    public void SetTileIndex(int tileIndex) { this.tileIndex = tileIndex; }

    private bool isTraversable = false;
    public bool IsTraversable() { return isTraversable; }
    public void SetTraversable(bool isTraversable) { this.isTraversable = isTraversable; }

    public int pathStep = 99999;
    public int GetPathStep() { return pathStep; }
    public void SetPathStep(int pathStep) { this.pathStep = pathStep; }
    public void ResetPathStep() { pathStep = 99999; }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
