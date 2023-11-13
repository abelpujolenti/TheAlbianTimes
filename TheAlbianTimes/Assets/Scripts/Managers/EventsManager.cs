using System;
using Editorial;
using Layout;
using UnityEngine;

namespace Managers
{
    public static class EventsManager
    {
        #region LayoutEvents
        
        public static Action <GameObject, int> OnAddNewsHeadlinePieceToLayout;
        public static Action <NewsHeadlineSubPiece, Vector2> OnDropNewsHeadlinePiece;
        public static Func <NewsHeadlineSubPiece, Vector2, NewsHeadlineSubPiece[], Cell[]> OnPreparingCells;
        public static Func <Cell[], Vector2, Vector3> OnSuccessFulSnap;
        public static Action <NewsHeadlinePiece, Vector2> OnFailSnap;
        public static Action <GameObject> OnSendNewsHeadlinePieceToEditorial;
        
        #endregion

        #region EditorialEvents
        
        public static Action OnChangeSelectedBias;
        public static Action <int> OnChangeSelectedBiasIndex;
        public static Action <int> OnChangeNewsHeadlineContent;
        public static Action <GameObject> OnAddNewsHeadlineToFolder;
        public static Action <NewsHeadline> OnReturnNewsHeadlineToFolder;
        public static Action <int> OnChangeFrontNewsHeadline;
        public static Action <String[], String[]> OnSettingNewBiases;
        public static Action OnChangeToNewBias;
        public static Action OnChangeFolderOrderIndexWhenGoingToFolder;
        public static Action <NewsHeadline> OnPrepareNewsHeadlineActions;
        public static Action <NewsHeadline, Vector3> OnDropNewsHeadline;
        public static Action OnDragNewsHeadline;

        #endregion
    }
}
