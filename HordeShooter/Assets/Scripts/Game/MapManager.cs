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

            // Create a wall collider.
            if (mapString[mapIndex].Equals(Constants.tileWall)) {
                GameObject wallObject = Instantiate(
                    PrefabManager.instance.GetPrefab(Constants.gameObjectWallObject),
                    mapTile.transform
                );
            }
            
            string spriteName = GetBaseSpriteName(mapIndex);
            if (spriteName.Equals(Constants.valueNada)) {
                continue;
            }
            GameObject sprite = Instantiate(
                PrefabManager.instance.GetTileSpritePrefab(spriteName),
                mapTile.transform
            );
            
            int orientation = -1;
            // Orient a wall piece
            if (spriteName.ToUpper().Contains(Constants.valueWall) && spriteName.ToUpper().Contains(Constants.valueSide)) {
                orientation = GetWallSidePieceOrientation(mapIndex);
            }
            if (spriteName.ToUpper().Contains(Constants.valueWall) && spriteName.ToUpper().Contains(Constants.valueOuter)) {
                orientation = GetWallOuterCornerOrientation(mapIndex);
            }
            if (spriteName.ToUpper().Contains(Constants.valueWall) && spriteName.ToUpper().Contains(Constants.valueInner)) {
                orientation = GetWallInnerCornerOrientation(mapIndex);
            }

            if (orientation != -1) {
                sprite.transform.eulerAngles = new Vector3(
                    sprite.transform.eulerAngles.x,
                    orientation,
                    sprite.transform.eulerAngles.z
                );
            }
        }
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
            
            // Check for outer corner.
            if (IsOuterCorner(mapIndex)) {
                return Constants.spriteWallOuterCorner;
            }

            // Check for inner corner.
            if (IsInnerCorner(mapIndex)) {
                return Constants.spriteWallInnerCorner;
            }
        }
        
        return Constants.valueNada;
    }

    // Get the orientation of a wall inner corner piece.
    private int GetWallInnerCornerOrientation(int mapIndex) {
        // Check diagonals for a space tile.
        int upperRightIndex = mapIndex - mapWidth + 1;
        if ((upperRightIndex >= 0 && upperRightIndex % mapWidth != 0) && mapString[upperRightIndex].Equals(Constants.tileFill)) {
            return 90;
        }

        int bottomRightIndex = mapIndex + mapWidth + 1;
        if ((bottomRightIndex < (mapWidth * mapHeight) && bottomRightIndex % mapWidth != 0) && mapString[bottomRightIndex].Equals(Constants.tileFill)) {
            return 180;
        }

        int bottomLeftIndex = mapIndex + mapWidth - 1;
        if ((bottomLeftIndex < (mapWidth * mapHeight) && (bottomLeftIndex % mapWidth != (mapWidth - 1)) && mapString[bottomLeftIndex].Equals(Constants.tileFill))) {
            return 270;
        }

        int upperLeftIndex = mapIndex - mapWidth - 1;
        if ((upperLeftIndex >= 0 && upperLeftIndex % mapWidth != (mapWidth - 1)) && mapString[upperLeftIndex].Equals(Constants.tileFill)) {
            return 0;
        }

        return -1;
    }

    // Get the orientation of a wall outer corner piece.
    private int GetWallOuterCornerOrientation(int mapIndex) {
        // Check diagonals for a floor tile.
        int upperRightIndex = mapIndex - mapWidth + 1;
        if ((upperRightIndex >= 0 && upperRightIndex % mapWidth != 0) && mapString[upperRightIndex].Equals(Constants.tileFloor)) {
            return 270;
        }

        int bottomRightIndex = mapIndex + mapWidth + 1;
        if ((bottomRightIndex < (mapWidth * mapHeight) && bottomRightIndex % mapWidth != 0) && mapString[bottomRightIndex].Equals(Constants.tileFloor)) {
            return 0;
        }

        int bottomLeftIndex = mapIndex + mapWidth - 1;
        if ((bottomLeftIndex < (mapWidth * mapHeight) && (bottomLeftIndex % mapWidth != (mapWidth - 1)) && mapString[bottomLeftIndex].Equals(Constants.tileFloor))) {
            return 90;
        }

        int upperLeftIndex = mapIndex - mapWidth - 1;
        if ((upperLeftIndex >= 0 && upperLeftIndex % mapWidth != (mapWidth - 1)) && mapString[upperLeftIndex].Equals(Constants.tileFloor)) {
            return 180;
        }

        return -1;
    }

    // Get the orientation of a wall side.
    private int GetWallSidePieceOrientation(int mapIndex) {
        // Check each side for a floor tile.
        int checkIndex = mapIndex + 1;
        if ((checkIndex % mapWidth != 0 && checkIndex < (mapWidth * mapHeight)) && mapString[checkIndex].Equals(Constants.tileFloor)) {
            return 0;
        }

        checkIndex = mapIndex - 1;
        if ((checkIndex % mapWidth != (mapWidth - 1) && checkIndex >= 0) && mapString[checkIndex].Equals(Constants.tileFloor)) {
            return 180;
        }

        checkIndex = mapIndex - mapWidth;
        if (checkIndex >= 0 && mapString[checkIndex].Equals(Constants.tileFloor)) {
            return 270;
        }

        checkIndex = mapIndex + mapWidth;
        if (checkIndex < (mapWidth * mapHeight) && mapString[checkIndex].Equals(Constants.tileFloor)) {
            return 90;
        }

        return -1;
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

        // Check vertically for 'X'
        checkIndex = mapIndex - mapWidth;
        if (checkIndex >= 0 && mapString[checkIndex].Equals(Constants.tileWall)) {
            checkIndex = mapIndex + 1;
            // Check horizontally from mapTile for 'X'
            if ((checkIndex % mapWidth != 0 && checkIndex < (mapWidth * mapHeight)) && mapString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
            checkIndex = mapIndex - 1;
            if ((checkIndex % mapWidth != (mapWidth - 1) && checkIndex >= 0) && mapString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
        }
        checkIndex = mapIndex + mapWidth;
        if (checkIndex < (mapWidth * mapHeight) && mapString[checkIndex].Equals(Constants.tileWall)) {
            // Check horizontally from mapTile for 'X'
            checkIndex = mapIndex + 1;
            if ((checkIndex % mapWidth != 0 && checkIndex < (mapWidth * mapHeight)) && mapString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
            checkIndex = mapIndex - 1;
            if ((checkIndex % mapWidth != (mapWidth - 1) && checkIndex >= 0) && mapString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
        }

        return false;
    }

    // Check if tile is an inner corner wall.
    private bool IsInnerCorner(int mapIndex) {
        /*
            Check for
            XX
            .X
        */
        // Check left for 'X'
        int leftIndex = mapIndex - 1;
        // Check below for 'X'
        int belowIndex = mapIndex + mapWidth;
        // Check bottom left for '.'
        int bottomLeftIndex = mapIndex + mapWidth - 1;
        if (((leftIndex % mapWidth != (mapWidth - 1) && leftIndex >= 0) && mapString[leftIndex].Equals(Constants.tileWall)) &&
            (belowIndex < (mapWidth * mapHeight) && mapString[belowIndex].Equals(Constants.tileWall)) &&
            ((bottomLeftIndex < (mapWidth * mapHeight) && (bottomLeftIndex % mapWidth != (mapWidth - 1)) && mapString[bottomLeftIndex].Equals(Constants.tileFill)))
        ) {
            return true;
        }

        /*
            Check for
            .X
            XX
        */
        // Check above for 'X'
        // Check upper left for '.'
        // Check above for 'X'
        int aboveIndex = mapIndex - mapWidth;
        // Check left for 'X'
        leftIndex = mapIndex - 1;
        // Check upper left for '0'
        int upperLeftIndex = mapIndex - mapWidth - 1;
        if ((aboveIndex >= 0 && mapString[aboveIndex].Equals(Constants.tileWall)) &&
            ((leftIndex % mapWidth != (mapWidth - 1) && leftIndex >= 0) && mapString[leftIndex].Equals(Constants.tileWall)) &&
            ((upperLeftIndex >= 0 && upperLeftIndex % mapWidth != (mapWidth - 1)) && mapString[upperLeftIndex].Equals(Constants.tileFill))
        ) {
            return true;
        }

        /*
            Check for
            X.
            XX
        */
        // Check above for 'X'
        aboveIndex = mapIndex - mapWidth;
        // Check to the right for 'X'
        int rightIndex = mapIndex + 1;
        // Check upper right for '.'
        int upperRightIndex = mapIndex - mapWidth + 1;
        if ((aboveIndex >= 0 && mapString[aboveIndex].Equals(Constants.tileWall)) &&
            ((rightIndex % mapWidth != 0 && rightIndex < (mapWidth * mapHeight)) && mapString[rightIndex].Equals(Constants.tileWall)) &&
            ((upperRightIndex >= 0 && upperRightIndex % mapWidth != 0) && mapString[upperRightIndex].Equals(Constants.tileFill))
        ) {
            return true;
        }

        /*
            Check for
            XX
            X.
        */
        // Check right for 'X'
        rightIndex = mapIndex + 1;
        // Check bottom for 'X'
        belowIndex = mapIndex + mapWidth;
        // Check bottom right for '.'
        int bottomRightIndex = mapIndex + mapWidth + 1;
        if (((rightIndex % mapWidth != 0 && rightIndex < (mapWidth * mapHeight)) && mapString[rightIndex].Equals(Constants.tileWall)) &&
            (belowIndex < (mapWidth * mapHeight) && mapString[belowIndex].Equals(Constants.tileWall)) &&
            ((bottomRightIndex < (mapWidth * mapHeight) && bottomRightIndex % mapWidth != 0) && mapString[bottomRightIndex].Equals(Constants.tileFill))
        ) {
            return true;
        }

        return false;
    }

    // Check if tile is an outer corner wall.
    private bool IsOuterCorner(int mapIndex) {
        /*
            Check for
            X0
            XX
        */
        // Check above for 'X'
        int aboveIndex = mapIndex - mapWidth;
        // Check to the right for 'X'
        int rightIndex = mapIndex + 1;
        // Check upper right for '0'
        int upperRightIndex = mapIndex - mapWidth + 1;
        if ((aboveIndex >= 0 && mapString[aboveIndex].Equals(Constants.tileWall)) &&
            ((rightIndex % mapWidth != 0 && rightIndex < (mapWidth * mapHeight)) && mapString[rightIndex].Equals(Constants.tileWall)) &&
            ((upperRightIndex >= 0 && upperRightIndex % mapWidth != 0) && mapString[upperRightIndex].Equals(Constants.tileFloor))
        ) {
            return true;
        }

        /*
            Check for
            XX
            X0
        */
        // Check right for 'X'
        rightIndex = mapIndex + 1;
        // Check bottom for 'X'
        int belowIndex = mapIndex + mapWidth;
        // Check bottom right for '0'
        int bottomRightIndex = mapIndex + mapWidth + 1;
        if (((rightIndex % mapWidth != 0 && rightIndex < (mapWidth * mapHeight)) && mapString[rightIndex].Equals(Constants.tileWall)) &&
            (belowIndex < (mapWidth * mapHeight) && mapString[belowIndex].Equals(Constants.tileWall)) &&
            ((bottomRightIndex < (mapWidth * mapHeight) && bottomRightIndex % mapWidth != 0) && mapString[bottomRightIndex].Equals(Constants.tileFloor))
        ) {
            return true;
        }

        /*
            Check for
            XX
            0X
        */
        // Check left for 'X'
        int leftIndex = mapIndex - 1;
        // Check below for 'X'
        belowIndex = mapIndex + mapWidth;
        // Check bottom left for '0'
        int bottomLeftIndex = mapIndex + mapWidth - 1;
        if (((leftIndex % mapWidth != (mapWidth - 1) && leftIndex >= 0) && mapString[leftIndex].Equals(Constants.tileWall)) &&
            (belowIndex < (mapWidth * mapHeight) && mapString[belowIndex].Equals(Constants.tileWall)) &&
            ((bottomLeftIndex < (mapWidth * mapHeight) && (bottomLeftIndex % mapWidth != (mapWidth - 1)) && mapString[bottomLeftIndex].Equals(Constants.tileFloor)))
        ) {
            return true;
        }

        /*
            Check for
            0X
            XX
        */
        // Check above for 'X'
        aboveIndex = mapIndex - mapWidth;
        // Check left for 'X'
        leftIndex = mapIndex - 1;
        // Check upper left for '0'
        int upperLeftIndex = mapIndex - mapWidth - 1;
        if ((aboveIndex >= 0 && mapString[aboveIndex].Equals(Constants.tileWall)) &&
            ((leftIndex % mapWidth != (mapWidth - 1) && leftIndex >= 0) && mapString[leftIndex].Equals(Constants.tileWall)) &&
            ((upperLeftIndex >= 0 && upperLeftIndex % mapWidth != (mapWidth - 1)) && mapString[upperLeftIndex].Equals(Constants.tileFloor))
        ) {
            return true;
        }

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
