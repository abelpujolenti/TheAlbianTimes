using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class TESTReturnNewsHEadlineToFolder : InteractableRectTransform
{

    [SerializeField] private GameObject _newsHeadlineContainer;
    
    protected override void PointerClick(BaseEventData data)
    {
        if (_newsHeadlineContainer.transform.childCount > 0)
        {
            //GameManager.Instance.SendNewsHeadlineToEditorialManager(_newsHeadlineContainer.transform.GetChild(0).gameObject);    
        }
    }
}
