using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance = null;
    
    void Awake() {
        instance = this;
    }

    // Map variables
    private string[] mapLines;
    private string mapString;
    private int mapHeight;
    private int mapWidth;
    private List<GameObject> mapTiles = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Build map
    public void BuildMap() {
        if (mapTiles.Count != 0) {
            foreach (GameObject tile in mapTiles) {
                Destroy(tile);
            }
            mapTiles = new List<GameObject>();
        }

        if ("".Equals(mapString)) {
            return;
        }

        for (int mapIndex = 0; mapIndex < mapWidth * mapHeight; mapIndex++) {
            GameObject mapTile = Instantiate(
                PrefabManager.instance.GetPrefab(Constants.gameObjectTileBase),
                new Vector3(mapIndex % mapWidth, 0, (mapIndex / mapWidth) * -1),
                Quaternion.identity
            );
            mapTile.name = Constants.gameObjectTileBase + Constants.splitCharUnderscore + mapIndex;
            string spriteName = GetBaseSpriteName(mapIndex);
            if (spriteName.Equals(Constants.valueNada)) {
                continue;
            }
            GameObject sprite = Instantiate(
                PrefabManager.instance.GetTileSpritePrefab(spriteName),
                mapTile.transform
            );
        }

        Camera.main.transform.position = new Vector3((float)(mapWidth - 1) / 2.0f, 10, (float)(mapHeight - 1) / -2.0f);
    }

    // Get the sprite for the tile location.
    private string GetBaseSpriteName(int mapIndex) {
        // Check for fill.
        if (mapString[mapIndex].Equals(Constants.tileFill)) {
            return Constants.valueNada;
        }
        
        // Check for floor tile.
        if (mapString[mapIndex].Equals(Constants.tileFloor)) {
            return Constants.spriteFloorBase_0;
        }

        // Check for flat wall.
        if (mapString[mapIndex].Equals(Constants.tileWall)) {
            // Check for touching an inner corner
            if (!IsInInnerCorner(mapIndex)) {
                return Constants.spriteWallSide_0;
            }
        }

        // Check for outer corner.

        // Check for inner corner.
        
        return Constants.spriteFloorBase_1;
    }

    // Check if tile is part of an inner corner.
    private bool IsInInnerCorner(int mapIndex) {
        int checkIndex = mapIndex - mapWidth;
        // Check vertically for '.'
        if (checkIndex >= 0 && mapString[checkIndex].Equals(Constants.tileFill)) {
            checkIndex = (mapIndex - mapWidth) + 1;
            // Check horizontally from '.' for 'X'
            if ((checkIndex % mapWidth != 0 && checkIndex < (mapWidth * mapHeight)) && mapString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
            checkIndex = (mapIndex - mapWidth) - 1;
            if ((checkIndex % mapWidth != (mapWidth - 1) && checkIndex >= 0) && mapString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
        }
        checkIndex = mapIndex + mapWidth;
        if (checkIndex < (mapWidth * mapHeight) && mapString[checkIndex].Equals(Constants.tileFill)) {
            // Check horizontally from '.' for 'X'
            checkIndex = (mapIndex + mapWidth) + 1;
            if ((checkIndex % mapWidth != 0 && checkIndex < (mapWidth * mapHeight)) && mapString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
            checkIndex = (mapIndex + mapWidth) - 1;
            if ((checkIndex % mapWidth != (mapWidth - 1) && checkIndex >= 0) && mapString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
        }

        // Check horizontally for '.'
        checkIndex = mapIndex + 1;
        if ((checkIndex % mapWidth != 0 && checkIndex < (mapWidth * mapHeight)) && mapString[checkIndex].Equals(Constants.tileFill)) {
            // Check vertically from '.' for 'X'
            checkIndex = (mapIndex + 1) - mapWidth;
            if (checkIndex >= 0 && mapString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
            checkIndex = (mapIndex + 1) + mapWidth;
            if (checkIndex < (mapWidth * mapHeight) && mapString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
        }
        checkIndex = mapIndex - 1;
        if (checkIndex % mapWidth != (mapWidth - 1) && mapString[checkIndex].Equals(Constants.tileFill)) {
            // Check vertically from '.' for 'X'
            checkIndex = (mapIndex - 1) - mapWidth;
            if (checkIndex >= 0 && mapString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
            checkIndex = (mapIndex - 1) + mapWidth;
            if (checkIndex < (mapWidth * mapHeight) && mapString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
        }

        // TODO: check for 'X' on top and side for wall corner origins.

        return false;
    }
    
    // Load map
    public void LoadMap(string mapName) {
        mapLines = System.IO.File.ReadAllLines(Constants.fileMapDirPath + mapName + ".txt");
        mapString = "";
        foreach (string line in mapLines) {
            mapString += line;
        }
        mapWidth = mapLines[0].Length;
        mapHeight = mapLines.Length;

        RenderMapInConsole();
    }

    // Render map in console
    public void RenderMapInConsole() {
        string mapRender = "";
        for (int mapIndex = 0; mapIndex < mapWidth * mapHeight; mapIndex++) {
            if (mapIndex > 0 && mapIndex % mapWidth == 0) {
                mapRender += "\n";
            }
            mapRender += mapString[mapIndex];
        }
        Debug.Log(mapRender);
    }
}
