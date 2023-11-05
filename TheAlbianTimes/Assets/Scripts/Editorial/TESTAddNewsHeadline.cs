using System.Collections;
using System.Collections.Generic;
using Editorial;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class TESTAddNewsHeadline : MovableRectTransform
{
    [SerializeField] private GameObject _news;

    [SerializeField] private NewsFolder _newsFolder;

    protected override void PointerClick(BaseEventData data)
    {
        _newsFolder.AddNewsHeadline(Instantiate(_news, _newsFolder.transform));
    }
}
