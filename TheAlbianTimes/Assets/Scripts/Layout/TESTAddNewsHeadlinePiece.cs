using Editorial;
using Managers;
using NoMonoBehavior;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class TESTAddNewsHeadlinePiece : InteractableRectTransform
{
    
    [SerializeField] private GameObject _news;

    protected override void PointerClick(BaseEventData data)
    {
        NewsHeadline newsHeadlineComponent = _news.GetComponent<NewsHeadline>();
        
        newsHeadlineComponent.SetNewsType((NewsType)Random.Range(0, 10));
        
        GameManager.Instance.SendNewsHeadlineToLayoutManager(_news, 3);
    }
}
