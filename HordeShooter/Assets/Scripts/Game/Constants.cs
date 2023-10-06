using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public readonly static string animationEnd = "End";
    public readonly static string animationInitialize = "Initialize";
    public readonly static string animationMetalRicochet = "MetalRicochet";
    public readonly static string animationReload = "Reload";

    public readonly static List<string> commandUnits = new List<string> {
        "ranged"
    };
    public readonly static string commandSpawn = "spawn";
    
    public readonly static string extensionTxt = ".txt";
    
    public readonly static string fileEnemiesPath = "Prefabs/Entities/Enemies/";
    public readonly static string fileMapDirPath = "Assets/Resources/Maps/";
    public readonly static string fileMunitionsPath = "Prefabs/Munitions/";
    public readonly static string fileObjectPath = "Prefabs/Tile/Objects/";
    public readonly static string fileTilePrefabPath = "Prefabs/Tile/";
    public readonly static string fileTileSpritePath = "Prefabs/Tile/Sprites/";
    public readonly static string fileUiPath = "Prefabs/UI/";
    public readonly static string fileVFXPath = "Prefabs/VFX/";

    public readonly static string gameObjectAmmoCounter = "AmmoCounter";
    public readonly static string gameObjectBackground = "Background";
    public readonly static string gameObjectBurrow = "Burrow";
    public readonly static string gameObjectConsole = "Console";
    public readonly static string gameObjectFireReference = "FireReference";
    public readonly static string gameObjectInputField = "InputField";
    public readonly static string gameObjectMetalRicochet = "MetalRicochet";
    public readonly static string gameObjectMuzzleFlare = "MuzzleFlare";
    public readonly static string gameObjectMuzzleFlarePoints = "MuzzleFlarePoints";
    public readonly static string gameObjectPillar = "Pillar";
    public readonly static string gameObjectPlasmaRicochet = "PlasmaRicochetObject";
    public readonly static string gameObjectProjectile = "Projectile";
    public readonly static string gameObjectRanged = "Ranged";
    public readonly static string gameObjectReloadIndicator = "ReloadIndicator";
    public readonly static string gameObjectRicochetObject = "RicochetObject";
    public readonly static string gameObjectRotationReference = "RotationReference";
    public readonly static string gameObjectRoundText = "RoundText";
    public readonly static string gameObjectSprite = "Sprite";
    public readonly static string gameObjectText = "Text";
    public readonly static string gameObjectTileBase = "TileObject";
    public readonly static string gameObjectWallObject = "WallObject";

    public readonly static string mapBase = "Base";
    public readonly static string mapLayerObjects = "Objects";
    public readonly static string mapLayerTerrain = "Terrain";

    public readonly static char splitCharSpace = ' ';
    public readonly static char splitCharUnderscore = '_';

    public readonly static string spriteFloorBase_0 = "FloorBase_0";
    public readonly static string spriteFloorBase_1 = "FloorBase_1";
    public readonly static string spriteFloorBase_2 = "FloorBase_2";
    public readonly static string spriteWallInnerCorner = "WallInnerCorner";
    public readonly static string spriteWallOuterCorner = "WallOuterCorner";
    public readonly static string spriteWallSide_0 = "WallSide_0";
    public readonly static string spriteWallSide_1 = "WallSide_1";

    public readonly static string tagCanvas = "Canvas";
    public readonly static string tagEnemy = "Enemy";
    public readonly static string tagObject = "Object";
    public readonly static string tagPlayer = "Player";
    public readonly static string tagTile = "Tile";
    public readonly static string tagRoundIndicator = "RoundIndicator";
    public readonly static string tagWall = "Wall";

    public readonly static char tileFill = '.';
    public readonly static char tileFloor = '0';
    public readonly static char tilePillar = 'I';
    public readonly static char tileWall = 'X';

    public readonly static string valueInner = "INNER";
    public readonly static string valueNada = "NADA";
    public readonly static string valueOuter = "OUTER";
    public readonly static string valueSide = "SIDE";
    public readonly static string valueWall = "WALL";
}
