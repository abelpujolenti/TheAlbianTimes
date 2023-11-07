using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utility;

namespace Editorial
{
    public class NewsFolder : MonoBehaviour
    {
        private const float FOLDER_MIN_Y_COORDINATE = -35;
        private const float FOLDER_MAX_Y_COORDINATE = 11;
        
        [SerializeField]private List<NewsHeadline> _newsHeadlines;

        private int _sentNewsHeadlines; 

        private void OnEnable()
        {
            ActionsManager.OnAddNewsHeadlineToFolder += AddNewsHeadlineGameObject;
        }

        private void OnDisable()
        {
            ActionsManager.OnAddNewsHeadlineToFolder -= AddNewsHeadlineGameObject;
        }

        private void AddNewsHeadlineGameObject(GameObject newsHeadlineGameObject)
        {
            NewsHeadline newsHeadlineComponent = newsHeadlineGameObject.GetComponent<NewsHeadline>();
            
            newsHeadlineComponent.SetNewsFolder(this);
            
            AddNewsHeadlineComponentToList(newsHeadlineGameObject.GetComponent<NewsHeadline>());
            
            PositionNewsHeadlines(_sentNewsHeadlines);
        }

        private void AddNewsHeadlineComponentToList(NewsHeadline newsHeadline)
        {
            if (_sentNewsHeadlines == 0)
            {
                _newsHeadlines.Add(newsHeadline);
            }
            else
            {
                if (_newsHeadlines.Count == 1)
                {
                    RelevantStuffOnSwitchingFrontNewsHeadline(false);
                }
                _newsHeadlines.Insert(_newsHeadlines.Count - _sentNewsHeadlines, newsHeadline);
                ReindexFolderOrder(_newsHeadlines.Count);
                RedirectInComingNewsHeadlineToFolder();
                CheckCurrentNewsHeadlinesSent();
            }

            if (_newsHeadlines.Count - _sentNewsHeadlines != 1)
            {
                return;
            }
            
            EditorialManager.Instance.ActivateBiasCanvas();
            ActionsManager.OnSendNewsHeadline += SendNewsHeadlineToLayout;
            ActionsManager.OnChangeToNewBias();
            ActionsManager.OnChangeFrontNewsHeadline(newsHeadline.GetChosenBiasIndex());
            ActionsManager.OnSettingNewBiasDescription(newsHeadline.GetShortBiasDescription());
            ActionsManager.OnChangeNewsHeadlineContent += newsHeadline.ChangeContent;
            newsHeadline.SetInFront(true);
        }

        private void FreeSpaceForInComingNewsHeadline()
        {
            PositionNewsHeadlines();
        }

        private void PositionNewsHeadlines(int totalIndicesToIgnore = 0)
        {
            int newsHeadlineListLength = _newsHeadlines.Count;
            
            if (newsHeadlineListLength == 1)
            {
                NewsHeadline newsHeadline = _newsHeadlines[0];
                
                ActionsManager.OnChangeToNewBias();
                ActionsManager.OnSettingNewBiasDescription(newsHeadline.GetShortBiasDescription());
                newsHeadline.transform.localPosition = new Vector3(0, FOLDER_MIN_Y_COORDINATE, 0);
                newsHeadline.SetFolderOrderIndex(0);
                newsHeadline.SetOrigin(new Vector2(0, FOLDER_MIN_Y_COORDINATE));
                return;
            }

            Vector2 newOrigin;
            
            for (int i = 0; i < newsHeadlineListLength - totalIndicesToIgnore; i++)
            {
                NewsHeadline newsHeadline = _newsHeadlines[i];
                
                newsHeadline.transform.SetSiblingIndex((newsHeadlineListLength - 1) - i);
                newsHeadline.SetFolderOrderIndex(i);
                if (i > (_newsHeadlines.Count - 1) - _sentNewsHeadlines)
                {
                    RedirectInComingNewsHeadlineToFolder();
                    continue;
                }
                newOrigin = new Vector2(0,GiveNewFolderYCoordinate(i, _newsHeadlines.Count - 1));
                newsHeadline.transform.localPosition = newOrigin;
                newsHeadline.SetOrigin(newOrigin);
            }
        }

        public void ReorderNewsHeadline(int newsHeadlineToSwitchIndex, int newChoseBiasIndex, String[] newShortBiasDescription)
        {
            ChangeFolderOrdersByGivenIndex(newsHeadlineToSwitchIndex, newChoseBiasIndex, newShortBiasDescription);
            
            PositionNewsHeadlines((_newsHeadlines.Count - 1) - newsHeadlineToSwitchIndex);
        }

        private void ChangeFolderOrdersByGivenIndex(int newsHeadlineToSwitchIndex, int newChoseBiasIndex, String[] newShortBiasDescription) 
        {        
            RelevantStuffOnSwitchingFrontNewsHeadline(false);
            
            ChangeOrders(newsHeadlineToSwitchIndex);
            
            RelevantStuffOnSwitchingFrontNewsHeadline(true);

            ActionsManager.OnChangeFrontNewsHeadline(newChoseBiasIndex);
            ActionsManager.OnSettingNewBiasDescription(newShortBiasDescription);        
        }

        private void ChangeOrders(int newsHeadlineToSwitchIndex)
        {
            NewsHeadline newsHeadlineToFront = _newsHeadlines[newsHeadlineToSwitchIndex];
            
            _newsHeadlines.RemoveAt(newsHeadlineToSwitchIndex);

            if (newsHeadlineToSwitchIndex == 0)
            {
                _newsHeadlines.Add(newsHeadlineToFront);
            }
            else
            {
                _newsHeadlines.Insert(0, newsHeadlineToFront);    
            }
        }

        private void ReindexFolderOrder(int indexToReorder)
        {
            for (int i = 0; i < indexToReorder; i++)
            {
                ChangeFolderOrderIndex(i);
            }
        }

        private void ChangeFolderOrderIndex(int i)
        {
            _newsHeadlines[i].SetFolderOrderIndex(i);
        }

        private void RelevantStuffOnSwitchingFrontNewsHeadline(bool isFront)
        {
            NewsHeadline newsHeadline = _newsHeadlines[0];

            newsHeadline.SetInFront(isFront);
            if (isFront)
            {
                ActionsManager.OnChangeNewsHeadlineContent += newsHeadline.ChangeContent;
                return;
            }
            ActionsManager.OnChangeNewsHeadlineContent -= newsHeadline.ChangeContent;
        }

        public void ProcedureWhenSendNewsHeadlineToRewrite()
        {
            _sentNewsHeadlines++;
            
            if (_newsHeadlines.Count > 1)
            {
                NewsHeadline nextNewsHeadline = _newsHeadlines[1];
                ChangeFolderOrdersByGivenIndex(0, nextNewsHeadline.GetChosenBiasIndex(), nextNewsHeadline.GetShortBiasDescription());
            }
            else
            {
                NewsHeadline currentNewsHeadline = _newsHeadlines[0];

                ActionsManager.OnChangeNewsHeadlineContent += currentNewsHeadline.ChangeContent;
                ActionsManager.OnChangeFrontNewsHeadline(currentNewsHeadline.GetChosenBiasIndex());
                ActionsManager.OnSettingNewBiasDescription(currentNewsHeadline.GetShortBiasDescription());
            }
            
            FreeSpaceForInComingNewsHeadline();
            
            CheckCurrentNewsHeadlinesSent();
            
            RedirectInComingNewsHeadlineToFolder();
        }

        private void CheckCurrentNewsHeadlinesSent()
        {
            if (_newsHeadlines.Count <= _sentNewsHeadlines)
            {
                EditorialManager.Instance.DeactivateBiasCanvas();
                ActionsManager.OnSendNewsHeadline -= SendNewsHeadlineToLayout;
            }
            else
            {
                EditorialManager.Instance.ActivateBiasCanvas();
                ActionsManager.OnSendNewsHeadline += SendNewsHeadlineToLayout;
            }
        }

        private void RedirectInComingNewsHeadlineToFolder()
        {
            if (ActionsManager.OnChangeFolderOrderIndexWhenGoingToFolder != null)
            {
                ActionsManager.OnChangeFolderOrderIndexWhenGoingToFolder();
            }
        }

        public float GiveNewFolderYCoordinate(int index, int countOfTotalNewsHeadline)
        {
            return MathUtil.Map(index, 0, countOfTotalNewsHeadline, FOLDER_MIN_Y_COORDINATE, FOLDER_MAX_Y_COORDINATE);
        }

        public int GetNewsHeadlinesLength()
        {
            return _newsHeadlines.Count;
        }

        public void SubtractOneToSentNewsHeadline()
        {
            _sentNewsHeadlines--;
            if (_newsHeadlines.Count > _sentNewsHeadlines)
            {
                EditorialManager.Instance.ActivateBiasCanvas();
                ActionsManager.OnSendNewsHeadline += SendNewsHeadlineToLayout;
            }
        }

        private void SendNewsHeadlineToLayout() 
        {
            StartCoroutine(_newsHeadlines[0].SendToLayout());

            NewsHeadline frontNewsHeadline = _newsHeadlines[0];
            EditorialManager.Instance.SendNewsHeadlineToGameManager(frontNewsHeadline);

            _newsHeadlines.RemoveAt(0);

            if (_newsHeadlines.Count == 0)
            {
                EditorialManager.Instance.DeactivateBiasCanvas();
                ActionsManager.OnSendNewsHeadline -= SendNewsHeadlineToLayout;
                return;
            }

            frontNewsHeadline = _newsHeadlines[0];

            ReorderNewsHeadline(0, frontNewsHeadline.GetChosenBiasIndex(), frontNewsHeadline.GetShortBiasDescription());
        }
    }
}
