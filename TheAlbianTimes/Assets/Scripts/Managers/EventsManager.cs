using System;
using Editorial;
using Layout;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Managers
{
    public static class EventsManager
    {
        #region WorkspaceEvents

        public static Action <bool> OnStartEndDrag;
        public static Func <PointerEventData, GameObject> OnCrossMidPointWhileScrolling;
        public static Func <Vector2, Vector2> OnCheckDistanceToMouse;
        public static Action OnExceedCameraLimitsWhileDragging;
        public static Action <GameObject> OnAssignGameObjectToDrag;
        public static Action <bool> OnPressPanicButton;
        public static Action OnPressPanicButtonForPieces;

        #endregion        
        
        #region LayoutEvents
        
        public static Func <NewsHeadlineSubPiece, Vector2, NewsHeadlineSubPiece[], Cell[]> OnPreparingCells;
        public static Func <Cell[], Vector3> OnSuccessfulSnap;
        public static Func <NewsHeadlinePiece, bool> OnFailSnap;
        
        #endregion

        #region EditorialEvents
        
        public static Action OnChangeSelectedBias;
        public static Action <int> OnChangeSelectedBiasIndex;
        public static Action OnChangeNewsHeadlineContent;
        public static Action <GameObject> OnAddNewsHeadlineToFolder;
        public static Action <NewsHeadline, bool> OnReturnNewsHeadlineToFolder;
        public static Action <int> OnChangeFrontNewsHeadline;
        public static Action <String[], String[]> OnSettingNewBiases;
        public static Action OnChangeFolderOrderIndexWhenGoingToFolder;
        public static Action <NewsHeadline> OnPrepareNewsHeadlineActions;
        public static Action <NewsHeadline, Vector3> OnDropNewsHeadline;

        #endregion

        #region Mail

        public static Action <GameObject> OnAddEnvelope;
        public static Action <GameObject, bool> OnAddEnvelopeContentToList;
        public static Action<GameObject, GameObject> OnAddEnvelopeContent;

        #endregion
    }
}
