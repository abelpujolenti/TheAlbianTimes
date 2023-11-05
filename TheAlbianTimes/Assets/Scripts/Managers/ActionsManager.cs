using System;
using Editorial;
using Layout;
using UnityEngine;

namespace Managers
{
    public static class ActionsManager
    {
        public static Action<float> OnDragNewsHeadline;
        public static Action <NewsHeadlineSubPiece, Vector2> OnReleaseNewsHeadline;
        public static Func<NewsHeadlineSubPiece, Vector2, NewsHeadlineSubPiece[], Cell[]> OnPreparingCells;
        public static Func <Cell[], Vector2, Vector3> OnSuccessFul;
        public static Action OnChangeSelectedBias;
        public static Action <int> OnChangeSelectedBiasIndex;
        public static Action <int> OnChangeNewsHeadlineContent;
        public static Action <NewsHeadline> OnAddNewsHeadlineToFolder;
        public static Action <int> OnChangeFrontNewsHeadline;
        public static Action OnSendNewsHeadline;
    }
}
