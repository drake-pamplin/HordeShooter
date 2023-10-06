using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance = null;
    
    void Awake() {
        instance = this;
    }

    // Map variables
    private string[] mapTerrainLines;
    private string mapTerrainString;
    private string[] mapObjectLines;
    private string mapObjectString;
    private int mapHeight;
    private int mapWidth;
    private Dictionary<int, GameObject> mapTiles = new Dictionary<int, GameObject>();
    private List<GameObject> spawnableTiles = new List<GameObject>();
    
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
            foreach (KeyValuePair<int, GameObject> entry in mapTiles) {
                Destroy(entry.Value);
            }
            mapTiles = new Dictionary<int, GameObject>();
        }

        if ("".Equals(mapTerrainString)) {
            return;
        }

        // Create terrain layer.
        for (int mapIndex = 0; mapIndex < mapWidth * mapHeight; mapIndex++) {
            CreateTerrainTile(mapIndex);
            CreateObjectTile(mapIndex);
        }
    }

    // Create object tile.
    private void CreateObjectTile(int mapIndex) {
        // Get object sprite name.
        string spriteName = GetObjectSpriteName(mapIndex);
        if (spriteName.Equals(Constants.valueNada)) {
            return;
        }

        // Create object prefab.
        GameObject objectSprite = Instantiate(
            PrefabManager.instance.GetPrefab(spriteName),
            new Vector3(mapIndex % mapWidth, 0, (mapIndex / mapWidth) * -1),
            Quaternion.identity
        );
        objectSprite.name = spriteName;
        mapTiles[mapIndex].GetComponent<Tile>().SetTraversable(false);
    }

    // Create terrain tile.
    private void CreateTerrainTile(int mapIndex) {
        GameObject mapTile = Instantiate(
            PrefabManager.instance.GetPrefab(Constants.gameObjectTileBase),
            new Vector3(mapIndex % mapWidth, 0, (mapIndex / mapWidth) * -1),
            Quaternion.identity
        );
        mapTile.name = Constants.gameObjectTileBase + Constants.splitCharUnderscore + mapIndex;
        Tile tileScript = mapTile.GetComponent<Tile>();
        // Set the tile to be traversable if it is a floor tile.
        if (mapTerrainString[mapIndex].Equals(Constants.tileFloor)) {
            tileScript.SetTraversable(true);
            spawnableTiles.Add(mapTile);
        }
        tileScript.SetTileIndex(mapIndex);

        // Create a wall collider.
        if (mapTerrainString[mapIndex].Equals(Constants.tileWall)) {
            GameObject wallObject = Instantiate(
                PrefabManager.instance.GetPrefab(Constants.gameObjectWallObject),
                mapTile.transform
            );
            // Set tile to non-traversable if it is a wall.
            tileScript.SetTraversable(false);
        }
        
        string spriteName = GetBaseSpriteName(mapIndex);
        if (spriteName.Equals(Constants.valueNada)) {
            if (mapTerrainString[mapIndex].Equals(Constants.tileWall)) {
                SetTileNeighbors(mapIndex, mapTile);
                tileScript.SetTraversable(false);
                mapTiles.Add(mapIndex, mapTile);
            }
            return;
        }
        GameObject sprite = Instantiate(
            PrefabManager.instance.GetPrefab(spriteName),
            mapTile.transform
        );

        // Set tile neighbors.
        SetTileNeighbors(mapIndex, mapTile);
        
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

        mapTiles.Add(mapIndex, mapTile);
    }
    

    // Get the sprite for the tile location.
    private string GetBaseSpriteName(int mapIndex) {
        // Check for fill.
        if (mapTerrainString[mapIndex].Equals(Constants.tileFill)) {
            return Constants.valueNada;
        }
        
        // Check for floor tile.
        if (mapTerrainString[mapIndex].Equals(Constants.tileFloor)) {
            return Constants.spriteFloorBase_0;
        }

        // Check for flat wall.
        if (mapTerrainString[mapIndex].Equals(Constants.tileWall)) {
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

    // Get object sprite name.
    private string GetObjectSpriteName(int mapIndex) {
        string spriteName = Constants.valueNada;

        if (mapObjectString[mapIndex].Equals(Constants.tilePillar)) {
            spriteName = Constants.gameObjectPillar;
        }

        return spriteName;
    }

    // Get the route between points.
    public List<Vector3> GetRouteBetweenPoints(GameObject originTile, GameObject destinationTile) {
        // Clean up tiles.
        foreach (KeyValuePair<int, GameObject> entry in mapTiles) {
            entry.Value.GetComponent<Tile>().ResetPathStep();
        }

        if (originTile.GetComponent<Tile>().GetTileIndex() == destinationTile.GetComponent<Tile>().GetTileIndex()) {
            return new List<Vector3> { originTile.transform.position };
        }
        
        // Create queue and load with tile at origin point.
        Queue<GameObject> checkTiles = new Queue<GameObject>();
        originTile.GetComponent<Tile>().SetPathStep(0);
        checkTiles.Enqueue(originTile);

        Tile destinationTileScript = destinationTile.GetComponent<Tile>();

        int deadDropCounter = 0;
        while (deadDropCounter < 10000 && checkTiles.Count != 0) {
            deadDropCounter++;
            
            // For each tile, get each neighbor.
            GameObject checkTile = checkTiles.Dequeue();
            Tile tileScript = checkTile.GetComponent<Tile>();

            foreach (Tile.TileDirection tileDirection in Enum.GetValues(typeof(Tile.TileDirection))) {
                // For each neighbor, check if the neighbor tile is eligible to be the next step in the path.
                // Eligible tiles have their path step value set and are added to the queue for the next round.
                GameObject neighborTile = tileScript.GetTileInDirection(tileDirection);
                if (neighborTile == null) {
                    continue;
                }
                Tile neighborTileScript = neighborTile.GetComponent<Tile>();
                
                if (neighborTileScript.IsTraversable() && tileScript.GetPathStep() + 1 < neighborTileScript.GetPathStep()) {
                    neighborTileScript.SetPathStep(tileScript.GetPathStep() + 1);
                    checkTiles.Enqueue(neighborTile);
                }
            }
        }
        if (deadDropCounter == 10000) {
            Debug.Log("Timed out in pathfinding.");
        }

        // Get the route starting from the destination.
        List<GameObject> route = new List<GameObject>();
        GameObject pathTile = destinationTile;
        route.Add(pathTile);
        deadDropCounter = 0;
        while (deadDropCounter < 10000) {
            deadDropCounter++;
            
            Tile tileScript = pathTile.GetComponent<Tile>();
            if (tileScript.GetPathStep() == 0) {
                break;
            }
            foreach (Tile.TileDirection tileDirection in Enum.GetValues(typeof(Tile.TileDirection))) {
                GameObject neighborTile = tileScript.GetTileInDirection(tileDirection);
                if (neighborTile == null) {
                    continue;
                }
                Tile neighborTileScript = neighborTile.GetComponent<Tile>();

                if (neighborTileScript.GetPathStep() == tileScript.GetPathStep() - 1) {
                    pathTile = neighborTile;
                    route.Add(pathTile);
                    break;
                }
            }
        }
        if (deadDropCounter == 10000) {
            Debug.Log("Timed out in routing.");
        }

        // Reverse path order and remove the tile the player is on.
        route.Reverse();
        route.RemoveAt(route.Count - 1);

        // Simplify route.
        List<GameObject> simpleRoute = new List<GameObject>();
        GameObject checkPoint = route[0];
        int checkPointIndex = 1;
        GameObject previousPoint = checkPoint;
        deadDropCounter = 0;
        while (deadDropCounter < 10000) {
            deadDropCounter++;

            // From the check point, move up in the route and raycast until an obstacle is detected.
            if (route.Count == 1) {
                break;
            }
            simpleRoute.Add(checkPoint);
            for (int checkIndex = checkPointIndex; checkIndex < route.Count; checkIndex++) {
                // Check for end of route.
                if (checkIndex == route.Count - 1) {
                    checkPointIndex = checkIndex;
                    break;
                }
                
                GameObject nextPoint = route[checkIndex];
                Vector3 checkOriginPosition = checkPoint.transform.position;
                checkOriginPosition.y = GameManager.instance.GetEnemySphereCastHeight();
                Vector3 checkDestinationPosition = nextPoint.transform.position;
                checkDestinationPosition.y = GameManager.instance.GetEnemySphereCastHeight();
                float checkDistance = Vector3.Distance(checkOriginPosition, checkDestinationPosition);
                Vector3 raycastDirection = checkDestinationPosition - checkOriginPosition;
                LayerMask entityLayer = GameManager.instance.GetWorldEntityMask();
                RaycastHit hit;
                bool blocked = false;
                if (Physics.SphereCast(checkOriginPosition, GameManager.instance.GetEnemySphereCastRadius(), raycastDirection, out hit, checkDistance, ~entityLayer)) {
                    blocked = true;
                }
                if (blocked) {
                    checkPoint = previousPoint;
                    checkPointIndex = checkIndex;
                    break;
                } else {
                    previousPoint = route[checkIndex];
                }
            }
            if (checkPointIndex == route.Count - 1) {
                break;
            }
        }
        simpleRoute.Add(route[route.Count - 1]);
        if (deadDropCounter == 10000) {
            Debug.Log(
                "Check point: " + checkPoint.name +
                "\nCheck point index: " + checkPointIndex +
                "\nPrevious point: " + previousPoint.name +
                "\nSimplified route count: " + simpleRoute.Count
            );
            Debug.Log("Timed out at simplification.");
        }

        // Get tile locations for route.
        List<Vector3> routePoints = new List<Vector3>();
        foreach (GameObject tile in simpleRoute) {
            routePoints.Add(tile.transform.position);
        }

        return routePoints;
    }

    // Get the tile below the player.
    public GameObject GetTileBelowPlayer() {
        // Raycast down.
        GameObject tileDown = null;
        Vector3 raycastPosition = GameObject.FindGameObjectWithTag(Constants.tagPlayer).transform.position;
        raycastPosition.y = GameManager.instance.GetEnemySphereCastHeight();
        Vector3 raycastDirection = Vector3.down;
        RaycastHit hit;
        if (Physics.Raycast(raycastPosition, raycastDirection, out hit, 5)) {
            if (hit.collider.gameObject.CompareTag(Constants.tagTile)) {
                tileDown = hit.collider.gameObject;
                // Debug.Log("Player tile: " + tileDown.name);
            }
        }

        // Return tile.
        return tileDown;
    }

    // Get the orientation of a wall inner corner piece.
    private int GetWallInnerCornerOrientation(int mapIndex) {
        // Check diagonals for a space tile.
        int upperRightIndex = mapIndex - mapWidth + 1;
        if ((upperRightIndex >= 0 && upperRightIndex % mapWidth != 0) && mapTerrainString[upperRightIndex].Equals(Constants.tileFill)) {
            return 90;
        }

        int bottomRightIndex = mapIndex + mapWidth + 1;
        if ((bottomRightIndex < (mapWidth * mapHeight) && bottomRightIndex % mapWidth != 0) && mapTerrainString[bottomRightIndex].Equals(Constants.tileFill)) {
            return 180;
        }

        int bottomLeftIndex = mapIndex + mapWidth - 1;
        if ((bottomLeftIndex < (mapWidth * mapHeight) && (bottomLeftIndex % mapWidth != (mapWidth - 1)) && mapTerrainString[bottomLeftIndex].Equals(Constants.tileFill))) {
            return 270;
        }

        int upperLeftIndex = mapIndex - mapWidth - 1;
        if ((upperLeftIndex >= 0 && upperLeftIndex % mapWidth != (mapWidth - 1)) && mapTerrainString[upperLeftIndex].Equals(Constants.tileFill)) {
            return 0;
        }

        return -1;
    }

    // Get the orientation of a wall outer corner piece.
    private int GetWallOuterCornerOrientation(int mapIndex) {
        // Check diagonals for a floor tile.
        int upperRightIndex = mapIndex - mapWidth + 1;
        if ((upperRightIndex >= 0 && upperRightIndex % mapWidth != 0) && mapTerrainString[upperRightIndex].Equals(Constants.tileFloor)) {
            return 270;
        }

        int bottomRightIndex = mapIndex + mapWidth + 1;
        if ((bottomRightIndex < (mapWidth * mapHeight) && bottomRightIndex % mapWidth != 0) && mapTerrainString[bottomRightIndex].Equals(Constants.tileFloor)) {
            return 0;
        }

        int bottomLeftIndex = mapIndex + mapWidth - 1;
        if ((bottomLeftIndex < (mapWidth * mapHeight) && (bottomLeftIndex % mapWidth != (mapWidth - 1)) && mapTerrainString[bottomLeftIndex].Equals(Constants.tileFloor))) {
            return 90;
        }

        int upperLeftIndex = mapIndex - mapWidth - 1;
        if ((upperLeftIndex >= 0 && upperLeftIndex % mapWidth != (mapWidth - 1)) && mapTerrainString[upperLeftIndex].Equals(Constants.tileFloor)) {
            return 180;
        }

        return -1;
    }

    // Get the orientation of a wall side.
    private int GetWallSidePieceOrientation(int mapIndex) {
        // Check each side for a floor tile.
        int checkIndex = mapIndex + 1;
        if ((checkIndex % mapWidth != 0 && checkIndex < (mapWidth * mapHeight)) && mapTerrainString[checkIndex].Equals(Constants.tileFloor)) {
            return 0;
        }

        checkIndex = mapIndex - 1;
        if ((checkIndex % mapWidth != (mapWidth - 1) && checkIndex >= 0) && mapTerrainString[checkIndex].Equals(Constants.tileFloor)) {
            return 180;
        }

        checkIndex = mapIndex - mapWidth;
        if (checkIndex >= 0 && mapTerrainString[checkIndex].Equals(Constants.tileFloor)) {
            return 270;
        }

        checkIndex = mapIndex + mapWidth;
        if (checkIndex < (mapWidth * mapHeight) && mapTerrainString[checkIndex].Equals(Constants.tileFloor)) {
            return 90;
        }

        return -1;
    }

    // Check if tile is part of an inner corner.
    private bool IsInInnerCorner(int mapIndex) {
        int checkIndex = mapIndex - mapWidth;
        // Check vertically for '.'
        if (checkIndex >= 0 && mapTerrainString[checkIndex].Equals(Constants.tileFill)) {
            checkIndex = (mapIndex - mapWidth) + 1;
            // Check horizontally from '.' for 'X'
            if ((checkIndex % mapWidth != 0 && checkIndex < (mapWidth * mapHeight)) && mapTerrainString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
            checkIndex = (mapIndex - mapWidth) - 1;
            if ((checkIndex % mapWidth != (mapWidth - 1) && checkIndex >= 0) && mapTerrainString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
        }
        checkIndex = mapIndex + mapWidth;
        if (checkIndex < (mapWidth * mapHeight) && mapTerrainString[checkIndex].Equals(Constants.tileFill)) {
            // Check horizontally from '.' for 'X'
            checkIndex = (mapIndex + mapWidth) + 1;
            if ((checkIndex % mapWidth != 0 && checkIndex < (mapWidth * mapHeight)) && mapTerrainString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
            checkIndex = (mapIndex + mapWidth) - 1;
            if ((checkIndex % mapWidth != (mapWidth - 1) && checkIndex >= 0) && mapTerrainString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
        }

        // Check horizontally for '.'
        checkIndex = mapIndex + 1;
        if ((checkIndex % mapWidth != 0 && checkIndex < (mapWidth * mapHeight)) && mapTerrainString[checkIndex].Equals(Constants.tileFill)) {
            // Check vertically from '.' for 'X'
            checkIndex = (mapIndex + 1) - mapWidth;
            if (checkIndex >= 0 && mapTerrainString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
            checkIndex = (mapIndex + 1) + mapWidth;
            if (checkIndex < (mapWidth * mapHeight) && mapTerrainString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
        }
        checkIndex = mapIndex - 1;
        if (checkIndex % mapWidth != (mapWidth - 1) && mapTerrainString[checkIndex].Equals(Constants.tileFill)) {
            // Check vertically from '.' for 'X'
            checkIndex = (mapIndex - 1) - mapWidth;
            if (checkIndex >= 0 && mapTerrainString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
            checkIndex = (mapIndex - 1) + mapWidth;
            if (checkIndex < (mapWidth * mapHeight) && mapTerrainString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
        }

        // Check vertically for 'X'
        checkIndex = mapIndex - mapWidth;
        if (checkIndex >= 0 && mapTerrainString[checkIndex].Equals(Constants.tileWall)) {
            checkIndex = mapIndex + 1;
            // Check horizontally from mapTile for 'X'
            if ((checkIndex % mapWidth != 0 && checkIndex < (mapWidth * mapHeight)) && mapTerrainString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
            checkIndex = mapIndex - 1;
            if ((checkIndex % mapWidth != (mapWidth - 1) && checkIndex >= 0) && mapTerrainString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
        }
        checkIndex = mapIndex + mapWidth;
        if (checkIndex < (mapWidth * mapHeight) && mapTerrainString[checkIndex].Equals(Constants.tileWall)) {
            // Check horizontally from mapTile for 'X'
            checkIndex = mapIndex + 1;
            if ((checkIndex % mapWidth != 0 && checkIndex < (mapWidth * mapHeight)) && mapTerrainString[checkIndex].Equals(Constants.tileWall)) {
                return true;
            }
            checkIndex = mapIndex - 1;
            if ((checkIndex % mapWidth != (mapWidth - 1) && checkIndex >= 0) && mapTerrainString[checkIndex].Equals(Constants.tileWall)) {
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
        if (((leftIndex % mapWidth != (mapWidth - 1) && leftIndex >= 0) && mapTerrainString[leftIndex].Equals(Constants.tileWall)) &&
            (belowIndex < (mapWidth * mapHeight) && mapTerrainString[belowIndex].Equals(Constants.tileWall)) &&
            ((bottomLeftIndex < (mapWidth * mapHeight) && (bottomLeftIndex % mapWidth != (mapWidth - 1)) && mapTerrainString[bottomLeftIndex].Equals(Constants.tileFill)))
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
        if ((aboveIndex >= 0 && mapTerrainString[aboveIndex].Equals(Constants.tileWall)) &&
            ((leftIndex % mapWidth != (mapWidth - 1) && leftIndex >= 0) && mapTerrainString[leftIndex].Equals(Constants.tileWall)) &&
            ((upperLeftIndex >= 0 && upperLeftIndex % mapWidth != (mapWidth - 1)) && mapTerrainString[upperLeftIndex].Equals(Constants.tileFill))
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
        if ((aboveIndex >= 0 && mapTerrainString[aboveIndex].Equals(Constants.tileWall)) &&
            ((rightIndex % mapWidth != 0 && rightIndex < (mapWidth * mapHeight)) && mapTerrainString[rightIndex].Equals(Constants.tileWall)) &&
            ((upperRightIndex >= 0 && upperRightIndex % mapWidth != 0) && mapTerrainString[upperRightIndex].Equals(Constants.tileFill))
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
        if (((rightIndex % mapWidth != 0 && rightIndex < (mapWidth * mapHeight)) && mapTerrainString[rightIndex].Equals(Constants.tileWall)) &&
            (belowIndex < (mapWidth * mapHeight) && mapTerrainString[belowIndex].Equals(Constants.tileWall)) &&
            ((bottomRightIndex < (mapWidth * mapHeight) && bottomRightIndex % mapWidth != 0) && mapTerrainString[bottomRightIndex].Equals(Constants.tileFill))
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
        if ((aboveIndex >= 0 && mapTerrainString[aboveIndex].Equals(Constants.tileWall)) &&
            ((rightIndex % mapWidth != 0 && rightIndex < (mapWidth * mapHeight)) && mapTerrainString[rightIndex].Equals(Constants.tileWall)) &&
            ((upperRightIndex >= 0 && upperRightIndex % mapWidth != 0) && mapTerrainString[upperRightIndex].Equals(Constants.tileFloor))
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
        if (((rightIndex % mapWidth != 0 && rightIndex < (mapWidth * mapHeight)) && mapTerrainString[rightIndex].Equals(Constants.tileWall)) &&
            (belowIndex < (mapWidth * mapHeight) && mapTerrainString[belowIndex].Equals(Constants.tileWall)) &&
            ((bottomRightIndex < (mapWidth * mapHeight) && bottomRightIndex % mapWidth != 0) && mapTerrainString[bottomRightIndex].Equals(Constants.tileFloor))
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
        if (((leftIndex % mapWidth != (mapWidth - 1) && leftIndex >= 0) && mapTerrainString[leftIndex].Equals(Constants.tileWall)) &&
            (belowIndex < (mapWidth * mapHeight) && mapTerrainString[belowIndex].Equals(Constants.tileWall)) &&
            ((bottomLeftIndex < (mapWidth * mapHeight) && (bottomLeftIndex % mapWidth != (mapWidth - 1)) && mapTerrainString[bottomLeftIndex].Equals(Constants.tileFloor)))
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
        if ((aboveIndex >= 0 && mapTerrainString[aboveIndex].Equals(Constants.tileWall)) &&
            ((leftIndex % mapWidth != (mapWidth - 1) && leftIndex >= 0) && mapTerrainString[leftIndex].Equals(Constants.tileWall)) &&
            ((upperLeftIndex >= 0 && upperLeftIndex % mapWidth != (mapWidth - 1)) && mapTerrainString[upperLeftIndex].Equals(Constants.tileFloor))
        ) {
            return true;
        }

        return false;
    }
    
    // Load map
    public void LoadMap(string mapName) {
        mapTerrainLines = System.IO.File.ReadAllLines(Constants.fileMapDirPath + mapName + Constants.splitCharUnderscore + Constants.mapLayerTerrain + ".txt");
        mapObjectLines = System.IO.File.ReadAllLines(Constants.fileMapDirPath + mapName + Constants.splitCharUnderscore + Constants.mapLayerObjects + ".txt");
        
        // Load the terrain strings.
        mapTerrainString = "";
        foreach (string line in mapTerrainLines) {
            mapTerrainString += line;
        }
        mapWidth = mapTerrainLines[0].Length;
        mapHeight = mapTerrainLines.Length;

        // Load the object strings.
        mapObjectString = "";
        foreach (string line in mapObjectLines) {
            mapObjectString += line;
        }
    }

    // Render map in console
    public void RenderMapInConsole() {
        string mapRender = "";
        for (int mapIndex = 0; mapIndex < mapWidth * mapHeight; mapIndex++) {
            if (mapIndex > 0 && mapIndex % mapWidth == 0) {
                mapRender += "\n";
            }
            mapRender += mapTerrainString[mapIndex];
        }
        Debug.Log(mapRender);
    }

    // Set the neighbors of a tile.
    private void SetTileNeighbors(int mapIndex, GameObject mapTile) {
        // Top
        GameObject topTile = null;
        mapTiles.TryGetValue(mapIndex - mapWidth, out topTile);
        if (topTile != null) {
            // Set the above tile's lower neighbor.
            topTile.GetComponent<Tile>().AddTile(Tile.TileDirection.Bottom, mapTile);

            // Set the current tile's upper neighbor.
            mapTile.GetComponent<Tile>().AddTile(Tile.TileDirection.Top, topTile);
        }
        
        // Left
        GameObject leftTile = null;
        mapTiles.TryGetValue(mapIndex - 1, out leftTile);
        if (leftTile != null && (mapIndex - 1) % mapWidth != mapWidth - 1) {
            // Set the left tile's right neighbor.
            leftTile.GetComponent<Tile>().AddTile(Tile.TileDirection.Right, mapTile);

            // Set the current tile's left neighbor.
            mapTile.GetComponent<Tile>().AddTile(Tile.TileDirection.Left, leftTile);
        }
    }

    // Spawn a given unit at the given location.
    public void SpawnUnitAtLocation(GameObject unit, GameObject tile) {
        Debug.Log("Spawning " + unit.name + " unit at tile " + tile.GetComponent<Tile>().GetTileIndex());

        // Create burrow at location and start borrow animation.
        GameObject burrowObject = Instantiate(
            PrefabManager.instance.GetPrefab(Constants.gameObjectBurrow),
            tile.transform.position,
            Quaternion.identity
        );

        // Set burrow for destruction.
        Destroy(burrowObject, 2);
        
        // Spawn enemy.
        GameObject enemyObject = Instantiate(
            unit,
            tile.transform.position,
            Quaternion.identity
        );
    }

    // Spawn a random unit at a random location.
    public void SpawnUnitAtRandom() {
        // Get unit.
        GameObject prefab = PrefabManager.instance.GetRandomEnemyPrefab();

        // Get available tile.
        GameObject tile = spawnableTiles[Random.Range(0, spawnableTiles.Count)];
        
        Debug.Log("Spawning " + unit.name + " unit at tile " + tile.GetComponent<Tile>().GetTileIndex());

        // Create burrow at location and start borrow animation.
        GameObject burrowObject = Instantiate(
            PrefabManager.instance.GetPrefab(Constants.gameObjectBurrow),
            tile.transform.position,
            Quaternion.identity
        );

        // Set burrow for destruction.
        Destroy(burrowObject, 2);
        
        // Spawn enemy.
        GameObject enemyObject = Instantiate(
            unit,
            tile.transform.position,
            Quaternion.identity
        );
    }
}
