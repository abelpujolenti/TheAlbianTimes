using System;
using UnityEngine;

public enum NewsType { MILITARY, ECONOMIC, CULTURAL, SPORTS }

[System.Serializable]
public class NewsTypeData {

    public NewsType type;
    public Vector2[] piecesCoordinatesFromRootPiece;
}

public class NewsTypeObject 
{
    public NewsTypeData[] newsTypeData;
}