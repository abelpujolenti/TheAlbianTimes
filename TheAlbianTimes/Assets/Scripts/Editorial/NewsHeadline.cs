using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Editorial
{
    public class NewsHeadline : MovableRectTransform
    {

        [SerializeField] private NewsFolder _newsFolder;

        [SerializeField] private int _folderOrderIndex;

        private int _siblingsCount;
        
        private bool _inFront;
        // Start is called before the first frame update
        new void Start()
        {
            base.Start();

            int _siblingIndex = transform.GetSiblingIndex();
            
            _siblingsCount = transform.parent.childCount;
            
            _folderOrderIndex = (_siblingsCount - 1) - transform.GetSiblingIndex();

            _inFront = _siblingIndex == _siblingsCount - 1;

        }

        protected override void PointerEnter(BaseEventData data)
        {
            if (_inFront)
            {
                return;
            }
            base.PointerEnter(data);
        }

        protected override void PointerExit(BaseEventData data)
        {
            if (_inFront)
            {
                return;
            }
            base.PointerExit(data);
        }

        protected override void PointerClick(BaseEventData data)
        {
            base.PointerClick(data);
            if (_inFront)
            {
                return;
            }
            _newsFolder.SwitchInFrontNewsHeadline(_folderOrderIndex);
            _inFront = true;
        }

        public void SetFolderOrderIndex(int newFolderOrderIndex)
        {
            _folderOrderIndex = newFolderOrderIndex;
        }

        public int GetFolderOrderIndex()
        {
            return _folderOrderIndex;
        }

        public void SetInFront(bool isInFront)
        {
            _inFront = isInFront;
        }
    }
}
