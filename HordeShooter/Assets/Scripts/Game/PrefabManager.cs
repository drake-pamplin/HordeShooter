using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance = null;
    
    void Awake() {
        instance = this;
        
        prefabLibrary = new Dictionary<string, GameObject>();
        PopulatePrefabLibrary();
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

        // Map sprites
        objectName = Constants.gameObjectTileBase;
        filePath = Constants.fileTilePrefabPath + objectName;
        prefab = Resources.Load<GameObject>(filePath);
        prefabLibrary.Add(objectName, prefab);
        
        objectName = Constants.gameObjectWallObject;
        filePath = Constants.fileTilePrefabPath + objectName;
        prefab = Resources.Load<GameObject>(filePath);
        prefabLibrary.Add(objectName, prefab);

        // VFX prefabs
        objectName = Constants.gameObjectMuzzleFlare;
        filePath = Constants.fileVFXPath + objectName;
        prefab = Resources.Load<GameObject>(filePath);
        prefabLibrary.Add(objectName, prefab);

        objectName = Constants.gameObjectRicochetObject;
        filePath = Constants.fileVFXPath + objectName;
        prefab = Resources.Load<GameObject>(filePath);
        prefabLibrary.Add(objectName, prefab);

        // Tile sprites
        objectName = Constants.spriteFloorBase_0;
        filePath = Constants.fileTileSpritePath + objectName;
        prefab = Resources.Load<GameObject>(filePath);
        prefabLibrary.Add(objectName, prefab);

        objectName = Constants.spriteFloorBase_1;
        filePath = Constants.fileTileSpritePath + objectName;
        prefab = Resources.Load<GameObject>(filePath);
        prefabLibrary.Add(objectName, prefab);

        objectName = Constants.spriteFloorBase_2;
        filePath = Constants.fileTileSpritePath + objectName;
        prefab = Resources.Load<GameObject>(filePath);
        prefabLibrary.Add(objectName, prefab);

        objectName = Constants.spriteWallInnerCorner;
        filePath = Constants.fileTileSpritePath + objectName;
        prefab = Resources.Load<GameObject>(filePath);
        prefabLibrary.Add(objectName, prefab);

        objectName = Constants.spriteWallOuterCorner;
        filePath = Constants.fileTileSpritePath + objectName;
        prefab = Resources.Load<GameObject>(filePath);
        prefabLibrary.Add(objectName, prefab);

        objectName = Constants.spriteWallSide_0;
        filePath = Constants.fileTileSpritePath + objectName;
        prefab = Resources.Load<GameObject>(filePath);
        prefabLibrary.Add(objectName, prefab);

        objectName = Constants.spriteWallSide_1;
        filePath = Constants.fileTileSpritePath + objectName;
        prefab = Resources.Load<GameObject>(filePath);
        prefabLibrary.Add(objectName, prefab);

        // Object sprites
        objectName = Constants.gameObjectPillar;
        filePath = Constants.fileObjectPath + objectName;
        prefab = Resources.Load<GameObject>(filePath);
        prefabLibrary.Add(objectName, prefab);
    }
}
