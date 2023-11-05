using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utility;

namespace Editorial
{
    public class NewsFolder : MonoBehaviour
    {
        private const float CHANGE_CONTENT_Y_COODINATE = 1000;
        private const float FOLDER_MIN_Y_COORDINATE = -35;
        private const float FOLDER_MAX_Y_COORDINATE = 11;
        
        [SerializeField] private List<NewsHeadline> _newsHeadlines;

        private void OnEnable()
        {
            ActionsManager.OnAddNewsHeadlineToFolder += AddNewsHeadlineToList;
        }

        private void OnDisable()
        {
            ActionsManager.OnAddNewsHeadlineToFolder -= AddNewsHeadlineToList;
        }

        public void AddNewsHeadline(GameObject newsHeadlineGameObject)
        {
            NewsHeadline newsHeadlineComponent = newsHeadlineGameObject.GetComponent<NewsHeadline>();
            
            newsHeadlineComponent.SetNewsFolder(this);
            
            AddNewsHeadlineToList(newsHeadlineGameObject.GetComponent<NewsHeadline>());
            
            PositionNewsHeadlines(_newsHeadlines.Count - 1);
        }

        private void AddNewsHeadlineToList(NewsHeadline newsHeadline)
        {
            _newsHeadlines.Add(newsHeadline);

            if (_newsHeadlines.Count == 1)
            {
                ActionsManager.OnChangeNewsHeadlineContent += newsHeadline.ChangeContent;
            }
        }

        private void FreeSpaceForInComingNewsHeadline()
        {
            PositionNewsHeadlines(_newsHeadlines.Count);
        }

        private void PositionNewsHeadlines(int countOfTotalNewsHeadline)
        {
            int newsHeadlineListLength = _newsHeadlines.Count;
            
            if (newsHeadlineListLength == 1)
            {
                _newsHeadlines[0].transform.localPosition = new Vector3(0, FOLDER_MIN_Y_COORDINATE, 0);
                _newsHeadlines[0].SetOrigin(new Vector2(0, FOLDER_MIN_Y_COORDINATE));
                return;
            }

            Vector2 newOrigin;
            
            for (int i = 0; i < newsHeadlineListLength; i++)
            {
                newOrigin = new Vector2(0,
                    MathUtil.Map(i, 0, countOfTotalNewsHeadline, FOLDER_MIN_Y_COORDINATE, FOLDER_MAX_Y_COORDINATE));
                _newsHeadlines[i].transform.localPosition = newOrigin;
                _newsHeadlines[i].SetOrigin(newOrigin);
                _newsHeadlines[i].transform.SetSiblingIndex((newsHeadlineListLength - 1) - i);
            }
        }

        private void ChangeSiblingIndex(int i, int childCount)
        {
            _newsHeadlines[i].transform.SetSiblingIndex(childCount - 1 - _newsHeadlines[i].GetFolderOrderIndex());   
        }

        private void ChangeLocalPosition(int i)
        {
            _newsHeadlines[i].SetOrigin(_newsHeadlines[i + 1].GetOrigin());
        }

        private void ChangeNewsHeadlinesPositions(int newsHeadlineToSwitchIndex)
        {
            
            Vector3 inFrontPosition = _newsHeadlines[0].GetOrigin();

            int childCount = transform.childCount;
            
            for (int i = 0; i < newsHeadlineToSwitchIndex; i++)
            {
                ChangeLocalPosition(i);
                ChangeSiblingIndex(i, childCount);
            }
            _newsHeadlines[newsHeadlineToSwitchIndex].SetOrigin(inFrontPosition);
            _newsHeadlines[newsHeadlineToSwitchIndex].transform.SetAsLastSibling();
        }

        public void SwitchInFrontNewsHeadline(int newsHeadlineToSwitchIndex)
        {
            ActionsManager.OnChangeNewsHeadlineContent -= _newsHeadlines[0].ChangeContent;
            
            ChangeNewsHeadlinesPositions(newsHeadlineToSwitchIndex);
            
            ChangeOrders(newsHeadlineToSwitchIndex);
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

        public Vector2 SendNewsHeadlineToRewriteContent()
        {
            _newsHeadlines[0].SetFolderOrderIndex(_newsHeadlines.Count - 1);
            
            _newsHeadlines.RemoveAt(0);
            
            for (int i = 0; i < _newsHeadlines.Count; i++)
            {
                ChangeFolderOrder(i);
            }

            NewsHeadline frontNewsHeadline = _newsHeadlines[0];
            
            frontNewsHeadline.SetInFront(true);
            
            /*ActionsManager.OnChangeNewsHeadlineContent += frontNewsHeadline.ChangeContent;

            ActionsManager.OnChangeNewsHeadlineContent(frontNewsHeadline.GetChosenBiasIndex());*/
            
            FreeSpaceForInComingNewsHeadline();
            
            return new Vector2(0, CHANGE_CONTENT_Y_COODINATE);
        }

        public float GetFolderMaxYCoordinate()
        {
            return FOLDER_MAX_Y_COORDINATE;
        }
    }
}
