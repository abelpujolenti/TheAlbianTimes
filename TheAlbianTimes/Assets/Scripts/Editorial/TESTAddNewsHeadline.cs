using Editorial;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class TESTAddNewsHeadline : MovableRectTransform
{
    [SerializeField] private GameObject _news;

    [SerializeField] private NewsFolder _newsFolder;

    protected override void PointerClick(BaseEventData data)
    {
        GameObject newsHeadlineGameObject = Instantiate(_news, _newsFolder.transform);

        NewsHeadline newsHeadlineComponent = newsHeadlineGameObject.GetComponent<NewsHeadline>();
        
        newsHeadlineComponent.SetShortBiasDescription(new []{"Lo soy", "Lo eres", "Lo es"});
        newsHeadlineComponent.SetBiasContent(new []{"Soy Puto", "Sos Puto", "Òscar García Pañella, PhD."});

        ActionsManager.OnAddNewsHeadlineToFolder(newsHeadlineGameObject);
    }
}
