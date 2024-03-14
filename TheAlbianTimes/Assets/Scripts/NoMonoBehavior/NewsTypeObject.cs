using System;
using UnityEngine;

namespace NoMonoBehavior
{
    public enum NewsType { 
        POLITICS,
        OPINION,
        ENTERTAINMENT,
        DIPLOMACY,
        ECONOMICS,
        MILITARY,
        CULTURAL,
        TECHNOLOGY,
        LABOR,
        IDEOLOGY,
        TUTORIAL
    }

    [Serializable]
    public class NewsTypeData {

        public NewsType type;
        public Vector2[] piecesCoordinatesFromRootPiece;
    }

    public class NewsTypeObject 
    {
        public NewsTypeData[] newsTypeData;
    }
}