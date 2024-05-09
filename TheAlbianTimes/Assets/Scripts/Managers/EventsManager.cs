using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Workspace.Editorial;
using Workspace.Layout;

namespace Managers
{
    public static class EventsManager
    {
        #region WorkspaceEvents

        public static Action <bool> OnStartEndDrag;
        public static Func <PointerEventData, GameObject> OnCrossMidPointWhileScrolling;
        public static Func <Vector2, Vector2> OnCheckDistanceToMouse;
        public static Action OnThowSomething;
        public static Action OnArrangeSomething;
        public static Action OnPressPanicButton;
        public static Action OnPressPanicButtonForPieces;
        public static Action <String> OnClickLetter;
        public static Action OnOpenMapPages;
        public static Action OnCloseMapPages;

        #endregion        
        
        #region LayoutEvents
        
        public static Action <NewsHeadlineSubPiece, Vector2, NewsHeadlineSubPiece[]> OnDraggingPiece;
        public static Func <NewsHeadlineSubPiece, Vector2, NewsHeadlineSubPiece[], Cell[]> OnPreparingCells;
        public static Func <Cell[], NewsHeadline, GameObject, Vector3> OnSuccessfulSnap;
        public static Action <NewsHeadline> OnGrabSnappedPiece;
        
        #endregion

        #region EditorialEvents

        public static Action OnClickBias;
        public static Action OnChangeChosenBias;
        public static Action OnChangeChosenBiasIndex;
        public static Action <int> OnChangeNewsHeadlineContent;
        public static Action <GameObject> OnAddNewsHeadlineToFolder;
        public static Action <int> OnChangeFrontNewsHeadline;
        public static Action <String[], String[], int> OnSettingNewBiases;
        public static Action OnChangeFolderOrderIndexWhenGoingToFolder;
        public static Action <int> OnLinkFouthBiasWithNewsHeadline;

        #endregion

        #region Mail

        public static Action <GameObject> OnAddEnvelope;
        public static Action <GameObject, bool> OnAddEnvelopeContentToList;
        public static Action <GameObject, GameObject> OnAddEnvelopeContent;
        public static Action <GameObject> OnUseEnvelopeContent;

        #endregion
    }
}
