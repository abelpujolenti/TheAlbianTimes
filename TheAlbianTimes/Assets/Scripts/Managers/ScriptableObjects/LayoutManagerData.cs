using System.Collections.Generic;
using NoMonoBehavior;
using UnityEngine;

namespace Managers.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LayoutManagerData", menuName = "LayoutManagerData")]
    public class LayoutManagerData : ScriptableObject
    {
        public Dictionary<NewsType, Vector2[]> _newsPiecesCoordinates;
    }
}