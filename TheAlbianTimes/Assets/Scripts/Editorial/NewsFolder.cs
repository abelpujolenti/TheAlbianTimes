using UnityEngine;

namespace Editorial
{
    public class NewsFolder : MonoBehaviour
    {

        [SerializeField] private NewsHeadline[] _newsHeadlines;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SwitchInFrontNewsHeadline(int newsHeadlineToSwitchIndex)
        {
            ChangeNewsHeadlinesPositions(newsHeadlineToSwitchIndex);
            
            ChangeOrders(newsHeadlineToSwitchIndex);
        }

        private void ChangeNewsHeadlinesPositions(int newsHeadlineToSwitchIndex)
        {
            
            Vector3 inFrontPosition = _newsHeadlines[0].transform.localPosition;

            int childCount = transform.childCount;
            
            for (int i = 0; i < newsHeadlineToSwitchIndex; i++)
            {
                ChangeLocalPosition(i);
                ChangeSiblingIndex(i, childCount);
            }
            _newsHeadlines[newsHeadlineToSwitchIndex].transform.localPosition = inFrontPosition;
            _newsHeadlines[newsHeadlineToSwitchIndex].transform.SetAsLastSibling();
        }

        private void ChangeLocalPosition(int i)
        {
            _newsHeadlines[i].transform.localPosition = new Vector2(0, _newsHeadlines[i + 1].transform.localPosition.y);
        }

        private void ChangeSiblingIndex(int i, int childCount)
        {
            _newsHeadlines[i].transform.SetSiblingIndex(childCount - 1 - _newsHeadlines[i].GetFolderOrderIndex());   
        }

        private void ChangeOrders(int newsHeadlineToSwitchIndex)
        {
            NewsHeadline newsHeadlineToFront = _newsHeadlines[newsHeadlineToSwitchIndex];

            _newsHeadlines[0].SetInFront(false);

            for (int i = newsHeadlineToSwitchIndex; i > 0; i--)
            {
                ChangeArrayOrder(i);
                ChangeFolderOrder(i);
            }

            _newsHeadlines[0] = newsHeadlineToFront;
            _newsHeadlines[0].SetFolderOrderIndex(0);
        }

        private void ChangeArrayOrder(int i)
        {
            _newsHeadlines[i] = _newsHeadlines[i - 1];
        }

        private void ChangeFolderOrder(int i)
        {
            _newsHeadlines[i].SetFolderOrderIndex(i);
        }
    }
}
