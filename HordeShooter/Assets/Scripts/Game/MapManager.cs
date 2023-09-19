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
                new Vector3(mapIndex % mapWidth, 0, (mapIndex / mapHeight) * -1),
                Quaternion.identity
            );
        }

        Camera.main.transform.position = new Vector3((float)mapWidth / 2.0f, 10, (float)mapHeight / -2.0f);
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
