using System;
using Editorial;
using Layout;
using UnityEngine;

namespace Managers
{
    public static class EventsManager
    {
        #region WorkspaceEvents

        public static Action <bool> OnStartEndDrag;
        public static Func <bool, GameObject> OnCrossMidPointWhileScrolling;
        public static Func<Vector2, Vector2> OnCheckDistanceToMouse;

        #endregion
        
        
        #region LayoutEvents
        
        public static Action <NewsHeadlineSubPiece, Vector2> OnDropNewsHeadlinePiece;
        public static Func <NewsHeadlineSubPiece, Vector2, NewsHeadlineSubPiece[], Cell[]> OnPreparingCells;
        public static Func <Cell[], Vector2, Vector3> OnSuccessFulSnap;
        public static Action <NewsHeadlinePiece> OnFailSnap;
        
        #endregion

        #region EditorialEvents
        
        public static Action OnChangeSelectedBias;
        public static Action <int> OnChangeSelectedBiasIndex;
        public static Action <int> OnChangeNewsHeadlineContent;
        public static Action <GameObject> OnAddNewsHeadlineToFolder;
        public static Action <NewsHeadline> OnReturnNewsHeadlineToFolder;
        public static Action <int> OnChangeFrontNewsHeadline;
        public static Action <String[], String[]> OnSettingNewBiases;
        public static Action OnChangeFolderOrderIndexWhenGoingToFolder;
        public static Action <NewsHeadline> OnPrepareNewsHeadlineActions;
        public static Action <NewsHeadline, Vector3> OnDropNewsHeadline;

        #endregion
    }
}
