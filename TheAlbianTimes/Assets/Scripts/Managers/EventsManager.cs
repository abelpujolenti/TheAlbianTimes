using System;
using System.Collections.Generic;
using Editorial;
using Layout;
using UnityEngine;

namespace Managers
{
    public static class EventsManager
    {
        #region LayoutEvents
        
        public static Action <GameObject, int> OnAddNewsHeadlinePieceToLayout;
        public static Action OnDragNewsHeadlinePiece;
        public static Action <NewsHeadlineSubPiece, Vector2> OnDropNewsHeadlinePiece;
        public static Func <NewsHeadlineSubPiece, Vector2, NewsHeadlineSubPiece[], Cell[]> OnPreparingCells;
        public static Func <Cell[], Vector2, Vector3> OnSuccessFulSnap;
        public static Action <NewsHeadlinePiece, Vector2> OnFailSnap;
        public static Action OnSendNewsHeadlinePiece;
        public static Action <GameObject> OnSendNewsHeadlinePieceToEditorial;
        
        #endregion

        #region EditorialEvents
        
        public static Action OnChangeSelectedBias;
        public static Action <int> OnChangeSelectedBiasIndex;
        public static Action <int> OnChangeNewsHeadlineContent;
        public static Action <GameObject> OnAddNewsHeadlineToFolder;
        public static Action <int> OnChangeFrontNewsHeadline;
        public static Action <String[], String[]> OnSettingNewBiases;
        public static Action OnChangeToNewBias;
        public static Action OnChangeFolderOrderIndexWhenGoingToFolder;
        public static Action OnSendNewsHeadline;
        public static Action <NewsHeadline> OnDragNewsHeadline;
        public static Action <NewsHeadline, Vector3> OnDropNewsHeadline;

        #endregion
    }
}
