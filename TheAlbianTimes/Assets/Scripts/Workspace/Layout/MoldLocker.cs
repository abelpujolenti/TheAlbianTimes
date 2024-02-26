using UnityEngine;
using UnityEngine.EventSystems;
using Utility;
using Workspace.Layout;

public class MoldLocker : InteractableRectTransform
{
    [SerializeField] private NewspaperMold _newsPaperMold;

    protected override void PointerClick(BaseEventData data)
    {
        _newsPaperMold.SetLocked(!_newsPaperMold.IsLocked());
    }
}