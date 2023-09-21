using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public readonly static string extensionTxt = ".txt";
    
    public readonly static string fileMapDirPath = "Assets/Resources/Maps/";
    public readonly static string fileTilePrefabPath = "Prefabs/Tile/";
    public readonly static string fileTileSpritePath = "Prefabs/Tile/Sprites/";

    public readonly static string gameObjectRotationReference = "RotationReference";
    public readonly static string gameObjectSprite = "Sprite";
    public readonly static string gameObjectTileBase = "TileObject";

    public readonly static string mapBase = "Base";

    public readonly static char splitCharUnderscore = '_';

    public readonly static string spriteFloorBase_0 = "FloorBase_0";
    public readonly static string spriteFloorBase_1 = "FloorBase_1";
    public readonly static string spriteFloorBase_2 = "FloorBase_2";
    public readonly static string spriteWallInnerCorner = "WallInnerCorner";
    public readonly static string spriteWallOuterCorner = "WallOuterCorner";
    public readonly static string spriteWallSide_0 = "WallSide_0";
    public readonly static string spriteWallSide_1 = "WallSide_1";

    public readonly static char tileFill = '.';
    public readonly static char tileFloor = '0';
    public readonly static char tileWall = 'X';

    public readonly static string valueInner = "INNER";
    public readonly static string valueNada = "NADA";
    public readonly static string valueOuter = "OUTER";
    public readonly static string valueSide = "SIDE";
    public readonly static string valueWall = "WALL";
}
