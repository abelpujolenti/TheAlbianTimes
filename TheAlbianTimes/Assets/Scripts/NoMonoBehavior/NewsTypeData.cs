using System;
using UnityEngine;

namespace NoMonoBehavior
{
    public enum NewsType { MILITARY, ECONOMIC, CULTURAL, SPORT }

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