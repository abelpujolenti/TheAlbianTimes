using Editorial;
using Managers;
using UnityEngine;

namespace AbstractClasses
{
    public abstract class NewsHeadlineAction : NewsAction
    {
        private void OnEnable()
        {
            EventsManager.OnDropNewsHeadline += Action;
        }

        private void OnDisable()
        {
            EventsManager.OnDropNewsHeadline -= Action;
        }

        protected abstract void Action(NewsHeadline newsHeadline, Vector3 mousePosition);
    }
}
