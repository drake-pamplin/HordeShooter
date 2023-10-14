using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private enum RoomSide {
        Bottom,
        Left,
        Right,
        Top
    }
    private enum RoomSideEnd {
        Bottom,
        Left,
        Right,
        Top
    }
    private struct Room {
        public Room(int id, Vector2 coordinates, int width, int height) : this() {
            this.id = id;
            this.coordinates = coordinates;
            this.width = width;
            this.height = height;

            GenerateRoomTiles();
        }
        
        private int id;
        public int GetId() { return id; }
        
        // X, Y
        private Vector2 coordinates;
        public Vector2 GetCoordinates() { return coordinates; }

        private int width;
        public int GetWidth() { return width; }

        private int height;
        public int GetHeight() { return height; }

        private GameObject roomObject;
        public GameObject GetRoomObject() { return roomObject; }
        
        private List<GameObject> tiles;
        public GameObject GetTileAtIndex(int index) {
            GameObject tile = null;
            if (index < tiles.Count) {
                tile = tiles[index];
            }
            return tile;
        }
        public GameObject GetFirstTile() { return tiles[0]; }
        public GameObject GetLastTile() { return tiles[tiles.Count - 1]; }
        public GameObject GetTileAtPosition(Vector3 position) {
            foreach (GameObject tile in tiles) {
                if (tile.transform.position == position) {
                    return tile;
                }
            }
            return null;
        }
        public List<GameObject> GetAllTiles() { return tiles; }
        public int GetTileCount() { return tiles.Count; }

        private void GenerateRoomTiles() {
            // Create room object and add tiles.
            roomObject = new GameObject("Room_" + id);
            roomObject.transform.position = new Vector3(coordinates.x, 0, coordinates.y);
            tiles = new List<GameObject>();
            int maxTiles = width * height;
            Vector2 originPoint = new Vector2(Mathf.RoundToInt((width / 2) * -1), Mathf.RoundToInt(height / 2));
            for (int tileIndex = 0; tileIndex < maxTiles; tileIndex++) {
                int xCoord = (int)originPoint.x + (tileIndex % width);
                int yCoord = (int)originPoint.y - Mathf.RoundToInt(tileIndex / width);
                GameObject tile = Instantiate(
                    PrefabManager.instance.GetPrefab(Constants.spriteFloorBase_0),
                    roomObject.transform
                );
                tile.transform.localPosition = new Vector3(xCoord, 0, yCoord);
                tile.name = "Tile_" + tileIndex;
                tiles.Add(tile);
            }

            // Add collider for room connection and configure it.
            roomObject.AddComponent<BoxCollider>();
            roomObject.GetComponent<BoxCollider>().size = new Vector3(width, 1, height);
            float centerX = width % 2 == 0 ? -0.5f : 0;
            float centerY = height % 2 == 0 ? 0.5f : 0;
            roomObject.GetComponent<BoxCollider>().center = new Vector3(centerX, 0, centerY);
        }

        // Get the tile for hallway pathfinding based on the given other room.
        public GameObject GetTileForHallwayConnection(Room otherRoom) {
            // Get the side of the room the hallway will connect to.
            RoomSide roomSide;
            RoomSideEnd roomSideEnd;
            Vector3 direction = (otherRoom.GetRoomObject().transform.position - GetRoomObject().transform.position).normalized;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z)) {
                // Horizontal
                if (direction.x < 0) {
                    // Left
                    roomSide = RoomSide.Left;
                } else {
                    // Right
                    roomSide = RoomSide.Right;
                }
            } else {
                // Vertical
                if (direction.z > 0) {
                    // Top
                    roomSide = RoomSide.Top;
                } else {
                    // Bottom
                    roomSide = RoomSide.Bottom;
                }
            }

            // Get the end of the wall that the hall will connect.
            if (roomSide.Equals(RoomSide.Left) || roomSide.Equals(RoomSide.Right)) {
                // Left and right
                if (otherRoom.GetRoomObject().transform.position.z > GetRoomObject().transform.position.z) {
                    roomSideEnd = RoomSideEnd.Top;
                } else {
                    roomSideEnd = RoomSideEnd.Bottom;
                }
            } else {
                // Top and bottom
                if (otherRoom.GetRoomObject().transform.position.x > GetRoomObject().transform.position.x) {
                    roomSideEnd = RoomSideEnd.Right;
                } else {
                    roomSideEnd = RoomSideEnd.Left;
                }
            }
            Debug.Log("Side for hallaway connection between room " + GetRoomObject().name + " and other room " + otherRoom.GetRoomObject().name + " is " + roomSide + " " + roomSideEnd);
            
            // Get the tile to connect to.
            int tileIndex = 0;
            if (roomSide.Equals(RoomSide.Top)) {
                tileIndex = GetWidth() / 2;
            } else if (roomSide.Equals(RoomSide.Right)) {
                tileIndex = GetWidth() * (GetHeight() / 2) - 1;
            } else if (roomSide.Equals(RoomSide.Bottom)) {
                tileIndex = (GetTileCount() - 1) - (GetWidth() / 2);
            } else {
                tileIndex = GetWidth() * (GetHeight() / 2);
            }

            if (roomSideEnd.Equals(RoomSideEnd.Top)) {
                tileIndex -= GetWidth() * (GetHeight() / 4);
            } else if (roomSideEnd.Equals(RoomSideEnd.Right)) {
                tileIndex += GetWidth() / 4;
            } else if (roomSideEnd.Equals(RoomSideEnd.Bottom)) {
                tileIndex += GetWidth() * (GetHeight() / 4);
            } else {
                tileIndex -= GetWidth() / 4;
            }

            Debug.Log("Tile for hallway connection in room " + GetRoomObject().name + " is " + GetTileAtIndex(tileIndex).name);
            return GetTileAtIndex(tileIndex);
        }

        public bool IsOverlapping(Room otherRoom) {
            // Get top left point and bottom right point for each rectangle.
            int buffer = GameManager.instance.GetMapRoomBuffer();
            Vector2 roomTopLeft = new Vector2(GetFirstTile().transform.position.x - buffer, GetFirstTile().transform.position.z + buffer);
            Vector2 roomBottomRight = new Vector2(GetLastTile().transform.position.x + buffer, GetLastTile().transform.position.z - buffer);
            Vector2 otherRoomTopLeft = new Vector2(otherRoom.GetFirstTile().transform.position.x - buffer, otherRoom.GetFirstTile().transform.position.z + buffer);
            Vector2 otherRoomBottomRight = new Vector2(otherRoom.GetLastTile().transform.position.x + buffer, otherRoom.GetLastTile().transform.position.z - buffer);

            // Check if one rectangle is on the left side of the other.
            if (roomTopLeft.x > otherRoomBottomRight.x || otherRoomTopLeft.x > roomBottomRight.x) {
                return false;
            }

            // Check if one rectangle is on above the other.
            if (roomBottomRight.y > otherRoomTopLeft.y || otherRoomBottomRight.y > roomTopLeft.y) {
                return false;
            }
            
            // If no above conditions are met, the two are overlapping.
            return true;
        }

        // Move the room based on given input.
        public void Move(Vector3 moveAmount) {
            roomObject.transform.position += moveAmount;
        }
    }

    private struct Connection {
        public Connection(Room pointOne, Room pointTwo) {
            this.pointOne = pointOne;
            this.pointTwo = pointTwo;
            distance = Vector3.Distance(pointOne.GetRoomObject().transform.position, pointTwo.GetRoomObject().transform.position);
        }

        private Room pointOne;
        public Room GetPointOne() { return pointOne; }

        private Room pointTwo;
        public Room GetPointTwo() { return pointTwo; }

        private float distance;
        public float GetDistance() { return distance; }

        public Room GetOtherPoint(Room room) {
            return room.GetId() == GetPointOne().GetId() ? GetPointTwo() : GetPointOne();
        }
    }
    
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
    private Dictionary<Vector3, int> mapTilePositions = new Dictionary<Vector3, int>();
    private List<GameObject> spawnableTiles = new List<GameObject>();
    private List<Room> rooms = new List<Room>();
    private List<Connection> connections = new List<Connection>();
    private float mapGenerationDelay = 0;
    private enum MapGenerationStages {
        Generate,
        Separate,
        Select,
        Triangulate,
        Connect,
        Pathen,
        Done
    }
    private MapGenerationStages mapGenerationStage = MapGenerationStages.Generate;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GenerateRandomMap();
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

    // Connect the triangulated rooms together with flow in mind.
    private void ConnectRooms() {
        // Create a list of connections for the final map.
        List<Connection> mapConnections = new List<Connection>();
        
        // Set the first room as the start room and begin filling the map by looping through each room and connecting it to the start room.
        Room startRoom = rooms[0];
        mapConnections.Add(GetAllConnectionsForRoom(rooms[0]).OrderBy(roomConnection => roomConnection.GetDistance()).ToList()[0]);
        foreach (Room room in rooms) {
            // If the room to map is the start room, skip.
            if (room.GetId() == startRoom.GetId()) {
                continue;
            }

            // If room is already mapped, skip.
            if (RoomExistsInMap(mapConnections, room)) {
                continue;
            }

            // Work through the connections, using the shortest path between rooms until a room in the main map is connected to.
            Room checkRoom = room;
            List<Connection> roomPath = new List<Connection>();
            while (!RoomExistsInMap(mapConnections, checkRoom)) {
                // Get all connections for the room.
                List<Connection> roomConnections = GetAllConnectionsForRoom(checkRoom);

                // Sort connections by distance from shortest to longest.
                roomConnections = roomConnections.OrderBy(roomConnection => roomConnection.GetDistance()).ToList();

                // Loop through connections to find a valid connection with the shortest distance.
                Connection viableConnection = roomConnections[0];
                foreach (Connection roomConnection in roomConnections) {
                    // Any loop is invalid.
                    if (RoomExistsInMap(roomPath, roomConnection.GetOtherPoint(checkRoom))) {
                        continue;
                    }
                    
                    viableConnection = roomConnection;
                    break;
                }
                roomPath.Add(viableConnection);

                // Set the room 
                checkRoom = viableConnection.GetOtherPoint(checkRoom);
            }
            
            // Add the room path to the overall map.
            mapConnections.AddRange(roomPath);
        }

        // Set the main connections list as the now refined map connections.
        connections = mapConnections;
        
        foreach (Connection connection in connections) {
            Vector3 pointOne = connection.GetPointOne().GetRoomObject().transform.position;
            Vector3 pointTwo = connection.GetPointTwo().GetRoomObject().transform.position;
            Vector3 direction = pointTwo - pointOne;
            Debug.DrawRay(pointOne, direction, Color.red, GameManager.instance.GetMapCreationDelay());
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

    // Determine if a connection already exists.
    private bool DoesConnectionExist(Room room, Room otherRoom) {
        foreach (Connection connection in connections) {
            if (connection.GetPointOne().GetId() == room.GetId() && connection.GetPointTwo().GetId() == otherRoom.GetId()) {
                return true;
            }
            if (connection.GetPointOne().GetId() == otherRoom.GetId() && connection.GetPointTwo().GetId() == room.GetId()) {
                return true;
            }
        }
        return false;
    }

    // Determine if a connection has both points in the map.
    private bool DoesConnectionHaveBothPointsInMap(List<Connection> map, Connection checkConnection) {
        bool firstPointInMap = false;
        bool secondPointInMap = false;
        foreach (Connection connection in map) {
            if (checkConnection.GetPointOne().GetId() == connection.GetPointOne().GetId() ||
                checkConnection.GetPointOne().GetId() == connection.GetPointTwo().GetId()
            ) {
                firstPointInMap = true;
            }
            if (checkConnection.GetPointTwo().GetId() == connection.GetPointOne().GetId() ||
                checkConnection.GetPointTwo().GetId() == connection.GetPointTwo().GetId()
            ) {
                secondPointInMap = true;
            }
        }
        return firstPointInMap && secondPointInMap;
    }

    // Determine if a connection has one of its points in the map.
    private bool DoesConnectionHaveOnePointInMap(List<Connection> map, Connection checkConnection) {
        // Loop through map connections to see if the given connection has a point present.
        foreach (Connection connection in map) {
            if (checkConnection.GetPointOne().GetId() == connection.GetPointOne().GetId() || checkConnection.GetPointOne().GetId() == connection.GetPointTwo().GetId()) {
                return true;
            }
            if (checkConnection.GetPointTwo().GetId() == connection.GetPointOne().GetId() || checkConnection.GetPointTwo().GetId() == connection.GetPointTwo().GetId()) {
                return true;
            }
        }
        return false;
    }

    // Determine if overlap in rooms exists.
    private bool DoesOverlapExist() {
        foreach (Room room in rooms) {
            foreach (Room otherRoom in rooms) {
                if (room.GetId() == otherRoom.GetId()) {
                    continue;
                }

                if (room.IsOverlapping(otherRoom)) {
                    return true;
                }
            }
        }
        
        return false;
    }

    // Fill the map in with tiles.
    private void FillInMap() {
        Debug.Log("Filling in map.");
        
        // Get left most coordinate.
        int leftCoord = 0;
        // Get right most coordinate.
        int rightCoord = 0;
        // Get top most coordinate.
        int topCoord = 0;
        // Get bottom most coordinate.
        int bottomCoord = 0;
        foreach (Room room in rooms) {
            GameObject firstTile = room.GetFirstTile();
            GameObject lastTile = room.GetLastTile();

            leftCoord = Mathf.RoundToInt(firstTile.transform.position.x < leftCoord ? firstTile.transform.position.x : leftCoord);
            rightCoord = Mathf.RoundToInt(lastTile.transform.position.x > rightCoord ? lastTile.transform.position.x : rightCoord);
            topCoord = Mathf.RoundToInt(firstTile.transform.position.z > topCoord ? firstTile.transform.position.z : topCoord);
            bottomCoord = Mathf.RoundToInt(lastTile.transform.position.z < bottomCoord ? lastTile.transform.position.z : bottomCoord);
        }
        leftCoord -= GameManager.instance.GetMapBorderBuffer();
        rightCoord += GameManager.instance.GetMapBorderBuffer();
        topCoord += GameManager.instance.GetMapBorderBuffer();
        bottomCoord -= GameManager.instance.GetMapBorderBuffer();
        // Calculate width.
        int mapWidth = rightCoord - leftCoord + 1;
        // Calculate height.
        int mapHeight = topCoord - bottomCoord + 1;

        // Assign each tile to a dictionary.
        Dictionary<Vector3, GameObject> roomTileDictionary = new Dictionary<Vector3, GameObject>();
        foreach (Room room in rooms) {
            List<GameObject> tiles = room.GetAllTiles();
            foreach (GameObject tile in tiles) {
                roomTileDictionary.Add(tile.transform.position, tile);
            }
        }

        // Starting from the top right, insert a tile object at each coordinate, inserting room tiles as they are encountered.
        mapTiles = new Dictionary<int, GameObject>();
        Vector3 startPos = new Vector3(leftCoord, 0, topCoord);
        for (int tileIndex = 0; tileIndex < mapWidth * mapHeight; tileIndex++) {
            Vector3 tilePosition = new Vector3(startPos.x + tileIndex % mapWidth, 0, startPos.z - tileIndex / mapWidth);
            GameObject newTileObject = Instantiate(
                PrefabManager.instance.GetPrefab(Constants.gameObjectTileBase),
                tilePosition,
                Quaternion.identity
            );
            newTileObject.name = Constants.gameObjectTileBase + Constants.splitCharUnderscore + tileIndex;
            Tile newTile = newTileObject.GetComponent<Tile>();
            if (!roomTileDictionary.ContainsKey(tilePosition)) {
                newTileObject.GetComponent<Tile>().SetTraversable(true);
            }
            newTile.SetTileIndex(tileIndex);

            // Connect the new tile to the surrounding tiles.
            int leftTileIndex = tileIndex - 1;
            int topTileIndex = tileIndex - mapWidth;
            GameObject leftTileObject = null;
            mapTiles.TryGetValue(leftTileIndex, out leftTileObject);
            GameObject topTileObject = null;
            mapTiles.TryGetValue(topTileIndex, out topTileObject);

            if (leftTileObject != null) {
                Tile leftTile = leftTileObject.GetComponent<Tile>();
                newTile.AddTile(Tile.TileDirection.Left, leftTileObject);
                leftTile.AddTile(Tile.TileDirection.Right, newTileObject);
            }
            if (topTileObject != null) {
                Tile topTile = topTileObject.GetComponent<Tile>();
                newTile.AddTile(Tile.TileDirection.Top, topTileObject);
                topTile.AddTile(Tile.TileDirection.Bottom, newTileObject);
            }
            mapTiles.Add(tileIndex, newTileObject);
            mapTilePositions.Add(newTileObject.transform.position, tileIndex);
        }

        Debug.Log("Map filled in.");
    }
    
    // Generate a random map.
    private void GenerateRandomMap() {
        if (mapGenerationStage.Equals(MapGenerationStages.Generate)) {
            if (mapGenerationDelay == 0) {
                mapGenerationDelay = GameManager.instance.GetMapCreationDelay();
            }

            if (mapGenerationDelay > 0) {
                mapGenerationDelay -= Time.deltaTime;
            }

            if (mapGenerationDelay <= 0) {
                // Generate randomly sized rooms within radius of origin.
                Debug.Log("Generating rooms.");
                GenerateRandomRooms();

                mapGenerationDelay = 0;
                mapGenerationStage = MapGenerationStages.Separate;
            }
        }

        // Separate rooms.
        /*
            Separate rooms using the following logic:
                - direction = (otherRoomPos - roomPos).normalized
                - room.Move(-direction, tileSizeUnit)
                - otherRoom.Move(direction, tileSizeUnit)
            Repeat while any rooms overlap.
        */
        if (mapGenerationStage.Equals(MapGenerationStages.Separate)) {
            if (mapGenerationDelay == 0) {
                mapGenerationDelay = GameManager.instance.GetMapCreationDelay();
            }

            if (mapGenerationDelay > 0) {
                mapGenerationDelay -= Time.deltaTime;
            }

            if (mapGenerationDelay <= 0) {
                // Generate randomly sized rooms within radius of origin.
                Debug.Log("Separating rooms.");
                SeparateRooms();

                mapGenerationDelay = 0;
                mapGenerationStage = MapGenerationStages.Select;
            }
        }

        // Select rooms above a certain size.
        if (mapGenerationStage.Equals(MapGenerationStages.Select)) {
            if (mapGenerationDelay == 0) {
                mapGenerationDelay = GameManager.instance.GetMapCreationDelay();
            }

            if (mapGenerationDelay > 0) {
                mapGenerationDelay -= Time.deltaTime;
            }

            if (mapGenerationDelay <= 0) {
                // Filter out the non-viable rooms.
                Debug.Log("Selecting viable rooms.");
                SelectViableRooms();

                mapGenerationDelay = 0;
                mapGenerationStage = MapGenerationStages.Triangulate;
            }
        }

        // Triangulate rooms.
        if (mapGenerationStage.Equals(MapGenerationStages.Triangulate)) {
            if (mapGenerationDelay == 0) {
                mapGenerationDelay = GameManager.instance.GetMapCreationDelay();
            }

            if (mapGenerationDelay > 0) {
                mapGenerationDelay -= Time.deltaTime;
            }

            if (mapGenerationDelay <= 0) {
                // Triangulate the generated rooms.
                Debug.Log("Triangulating rooms.");
                TriangulateRooms();

                mapGenerationDelay = 0;
                mapGenerationStage = MapGenerationStages.Connect;
            }
        }

        // Connect rooms using minimal spanning tree.
        if (mapGenerationStage.Equals(MapGenerationStages.Connect)) {
            if (mapGenerationDelay == 0) {
                mapGenerationDelay = GameManager.instance.GetMapCreationDelay();
            }

            if (mapGenerationDelay > 0) {
                mapGenerationDelay -= Time.deltaTime;
            }

            if (mapGenerationDelay <= 0) {
                // Connect rooms in the map in a minimum spanning tree.
                Debug.Log("Connecting rooms.");
                ConnectRooms();

                mapGenerationDelay = 0;
                mapGenerationStage = MapGenerationStages.Pathen;
            }
        }

        // Generate paths between rooms using tree.
        if (mapGenerationStage.Equals(MapGenerationStages.Pathen)) {
            if (mapGenerationDelay == 0) {
                mapGenerationDelay = GameManager.instance.GetMapCreationDelay();
            }

            if (mapGenerationDelay > 0) {
                mapGenerationDelay -= Time.deltaTime;
            }

            if (mapGenerationDelay <= 0) {
                // Generate halls between rooms.
                Debug.Log("Generating halls for rooms.");
                GenerateRoomHalls();

                mapGenerationDelay = 0;
                mapGenerationStage = MapGenerationStages.Done;
            }
        }
    }

    // Generate random rooms around origin.
    private void GenerateRandomRooms() {
        rooms = new List<Room>();
        
        // Generate a random number of rooms within range of origin.
        for (int roomIndex = 0; roomIndex < GameManager.instance.GetMapMaxNumberOfRooms(); roomIndex++) {
            Vector2 coordinates = new Vector2(
                Mathf.RoundToInt(UnityEngine.Random.Range(GameManager.instance.GetMapMaxDevianceFromOrigin() * -1, GameManager.instance.GetMapMaxDevianceFromOrigin())),
                Mathf.RoundToInt(UnityEngine.Random.Range(GameManager.instance.GetMapMaxDevianceFromOrigin() * -1, GameManager.instance.GetMapMaxDevianceFromOrigin()))
            );
            int width = UnityEngine.Random.Range(GameManager.instance.GetMapMinRoomWidth(), GameManager.instance.GetMapMaxRoomWidth());
            int height = UnityEngine.Random.Range(GameManager.instance.GetMapMinRoomHeight(), GameManager.instance.GetMapMaxRoomHeight());
            Room room = new Room(roomIndex, coordinates, width, height);
            rooms.Add(room);
        }
    }
    
    // Generate halls between the rooms.
    private void GenerateRoomHalls() {
        // Fill in map with empty tiles to allow pathfinding.
        FillInMap();

        // Loop through map connections and connect the rooms with hallways.
        foreach (Connection connection in connections) {
            // Clean up tiles.
            foreach (KeyValuePair<int, GameObject> entry in mapTiles) {
                entry.Value.GetComponent<Tile>().ResetPathStep();
            }
            
            // For both rooms in the connection, do the following:
            // - Compare the rooms' positions and determine which side the halls should connect on.
            // - Also determine what end (top/bottom, left/right) of that side the hall should connect on to allow for multiple halls on one side.
            // - Determine a tile for each room based on the side and end from the above steps.
            GameObject startTileObject = GetTileAtPosition(connection.GetPointOne().GetTileForHallwayConnection(connection.GetPointTwo()).transform.position);
            Tile startTile = startTileObject.GetComponent<Tile>();
            startTile.SetTraversable(true);
            startTile.SetPathStep(0);
            GameObject endTileObject = GetTileAtPosition(connection.GetPointTwo().GetTileForHallwayConnection(connection.GetPointOne()).transform.position);
            Tile endTile = endTileObject.GetComponent<Tile>();
            endTile.SetTraversable(true);

            // - Determine a path between the two tiles.
            Queue<GameObject> tileQueue = new Queue<GameObject>();
            tileQueue.Enqueue(startTileObject);
            while (tileQueue.Count > 0) {
                GameObject tileObject = tileQueue.Dequeue();
                Tile tile = tileObject.GetComponent<Tile>();

                foreach (Tile.TileDirection tileDirection in Enum.GetValues(typeof(Tile.TileDirection))) {
                    GameObject otherTileObject = tile.GetTileInDirection(tileDirection);
                    if (otherTileObject == null) {
                        continue;
                    }
                    Tile otherTile = otherTileObject.GetComponent<Tile>();

                    if (otherTile.IsTraversable() && tile.GetPathStep() + 1 < otherTile.GetPathStep()) {
                        otherTile.SetPathStep(tile.GetPathStep() + 1);
                        tileQueue.Enqueue(otherTileObject);
                    }
                }
            }

            List<GameObject> path = new List<GameObject>();
            path.Add(endTileObject);
            GameObject checkTileObject = endTileObject;
            Tile checkTile = checkTileObject.GetComponent<Tile>();
            Debug.Log(checkTile.GetTileIndex() + ", " + startTile.GetTileIndex());
            while (checkTile.GetTileIndex() != startTile.GetTileIndex()) {
                foreach (Tile.TileDirection tileDirection in Enum.GetValues(typeof(Tile.TileDirection))) {
                    GameObject otherTileObject = checkTile.GetTileInDirection(tileDirection);
                    if (otherTileObject == null) {
                        continue;
                    }
                    Tile otherTile = otherTileObject.GetComponent<Tile>();

                    Debug.Log(otherTile.GetPathStep() + ", " + (checkTile.GetPathStep() - 1));
                    if (otherTile.GetPathStep() == checkTile.GetPathStep() - 1) {
                        path.Add(otherTileObject);
                        checkTileObject = otherTileObject;
                        checkTile = checkTileObject.GetComponent<Tile>();
                        break;
                    }
                }
            }
            Debug.Log(path.Count);
            path.Reverse();
            path.RemoveAt(path.Count - 1);
            path.RemoveAt(0);

            // - Build the path out.
            foreach (GameObject tileObject in path) {
                GameObject newTileObject = Instantiate(
                    PrefabManager.instance.GetPrefab(Constants.spriteFloorBase_0),
                    tileObject.transform.position,
                    Quaternion.identity
                );
            }
        }
    }
    
    // Get a list of all connections for a specific room.
    private List<Connection> GetAllConnectionsForRoom(Room room) {
        List<Connection> roomConnections = new List<Connection>();

        foreach (Connection connection in connections) {
            if (connection.GetPointOne().GetId() == room.GetId() || connection.GetPointTwo().GetId() == room.GetId()) {
                roomConnections.Add(connection);
            }
        }

        return roomConnections;
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

        if (originTile == null || destinationTile == null) {
            return new List<Vector3> { originTile.transform.position };
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
    
    // Get the tile at a given position.
    private GameObject GetTileAtPosition(Vector3 position) {
        GameObject tile = null;
        int tileIndex = 0;
        mapTilePositions.TryGetValue(position, out tileIndex);
        mapTiles.TryGetValue(tileIndex, out tile);
        return tile;
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

    // Check if room exists in the map path.
    private bool RoomExistsInMap(List<Connection> map, Room room) {
        foreach (Connection connection in map) {
            if (room.GetId() == connection.GetPointOne().GetId() || room.GetId() == connection.GetPointTwo().GetId()) {
                return true;
            }
        }
        return false;
    }
    
    // Select viable rooms.
    private void SelectViableRooms() {
        List<Room> nonViableRooms = new List<Room>();

        // Add rooms below the width and height threshold.
        foreach (Room room in rooms) {
            if (room.GetWidth() < GameManager.instance.GetMapViableRoomWidth() || room.GetHeight() < GameManager.instance.GetMapViableRoomHeight()) {
                nonViableRooms.Add(room);
            }
        }

        // Destroy non-viable rooms.
        foreach (Room room in nonViableRooms) {
            rooms.Remove(room);
            room.GetRoomObject().GetComponent<BoxCollider>().enabled = false;
            Destroy(room.GetRoomObject());
        }
    }

    // Separate rooms.
    private void SeparateRooms() {
        // Loop while any overlap exists
        int deadDropCounter = 0;
        while (DoesOverlapExist() && deadDropCounter < 10000) {
            deadDropCounter++;
            // Loop through rooms.
            foreach (Room room in rooms) {
                // For each room, check if overlap exists.
                foreach (Room otherRoom in rooms) {
                    if (room.GetId() == otherRoom.GetId()) {
                        continue;
                    }
                    
                    // If no overlap exists, move on to the next room.
                    if (!room.IsOverlapping(otherRoom)) {
                        continue;
                    }

                    // If overlap exists, move the two rooms.
                    Vector3 moveAmount = (otherRoom.GetRoomObject().transform.position - room.GetRoomObject().transform.position).normalized;
                    
                    // Round move direction to 0 or 1 for whole number based movement.
                    moveAmount.x = Mathf.RoundToInt(moveAmount.x);
                    moveAmount.z = Mathf.RoundToInt(moveAmount.z);

                    // Move the rooms.
                    room.Move(moveAmount * -1);
                    otherRoom.Move(moveAmount);
                }
            }
        }
        if (deadDropCounter == 10000) {
            Debug.LogError("Timed out at room separation.");
        }
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
        GameObject unit = PrefabManager.instance.GetRandomEnemyPrefab();

        // Get available tile.
        GameObject tile = spawnableTiles[UnityEngine.Random.Range(0, spawnableTiles.Count)];
        
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

    // Triangulate created rooms.
    private void TriangulateRooms() {
        connections = new List<Connection>();
        
        // Add the first room to the queue.
        Queue<Room> checkRooms = new Queue<Room>();
        checkRooms.Enqueue(rooms[0]);

        // While there are rooms in the queue.
        while (checkRooms.Count != 0) {
            Room room = checkRooms.Dequeue();

            // Check all other rooms.
            foreach (Room otherRoom in rooms) {
                if (room.GetId() == otherRoom.GetId()) {
                    continue;
                }
                // Debug.Log("Checking connection viability for " + room.GetId() + " and " + otherRoom.GetId());
                
                // Connect two rooms that meet the following criteria:
                // - Have clear line of sight.
                // - Are not already connected.
                RaycastHit hit;
                if (Physics.Linecast(room.GetRoomObject().transform.position, otherRoom.GetRoomObject().transform.position, out hit)) {
                    if (!hit.collider.gameObject.name.Equals(otherRoom.GetRoomObject().name)) {
                        // Debug.Log("Collision detected for " + room.GetId() + " and " + otherRoom.GetId() + " at " + hit.collider.gameObject.name);
                        continue;
                    }
                } else {
                    // Debug.Log("Continuing creation for connection between " + room.GetId() + " and " + otherRoom.GetId());
                }
                if (DoesConnectionExist(room, otherRoom)) {
                    // Debug.Log("Connection exists for " + room.GetId());
                    continue;
                }

                // Create new connection.
                // Debug.Log("Creating new connection for " + room.GetId() + " and " + otherRoom.GetId());
                Connection connection = new Connection(room, otherRoom);
                connections.Add(connection);
                
                // Add any rooms that were connected to the queue.
                checkRooms.Enqueue(otherRoom);
            }
        }

        // Debug.Log("Connections: " + connections.Count);
        foreach (Connection connection in connections) {
            Vector3 pointOne = connection.GetPointOne().GetRoomObject().transform.position;
            Vector3 pointTwo = connection.GetPointTwo().GetRoomObject().transform.position;
            Vector3 direction = pointTwo - pointOne;
            Debug.DrawRay(pointOne, direction, Color.red, GameManager.instance.GetMapCreationDelay());
        }
    }
}
