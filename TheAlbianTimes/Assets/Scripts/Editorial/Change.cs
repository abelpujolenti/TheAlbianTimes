using System;
using Managers;
using UnityEngine;

namespace Editorial
{
    public class Change : NewsHeadlineAction
    {
        protected override void Action(NewsHeadline newsHeadline, Vector3 mousePosition)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            
            if (!IsCoordinateInsideBounds(mousePosition))
            {
                newsHeadline.DropOnFolder();
                gameObject.SetActive(false);
                return;
            }   
            
            newsHeadline.SetOrigin(newsHeadline.transform.localPosition);
            
            EventsManager.OnChangeNewsHeadlineContent(newsHeadline.GetSelectedBiasIndex());
            
            gameObject.SetActive(false);
        }
    }
}
