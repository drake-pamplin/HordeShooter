using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance = null;
    
    void Awake() {
        instance = this;
    }
    
    private Dictionary<string, GameObject> prefabLibrary;
    public GameObject GetPrefab(string prefabName) {
        GameObject prefab = null;
        prefabLibrary.TryGetValue(prefabName, out prefab);
        return prefab;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        prefabLibrary = new Dictionary<string, GameObject>();

        PopulatePrefabLibrary();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Populate prefab library with gameObjects.
    private void PopulatePrefabLibrary() {
        string filePath = "";
        string objectName = "";
        GameObject prefab;

        filePath = Constants.fileTilePrefabPath + Constants.gameObjectTileBase;
        objectName = Constants.gameObjectTileBase;
        prefab = Resources.Load<GameObject>(filePath);
        prefabLibrary.Add(objectName, prefab);
    }
}
