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
        
        private List<NewsHeadline> _newsHeadlines;

        private int _sentNewsHeadlines;

        private bool _active; 

        private void OnEnable()
        {
            ActionsManager.OnAddNewsHeadlineToFolder += AddNewsHeadlineGameObject;
        }

        private void OnDisable()
        {
            ActionsManager.OnAddNewsHeadlineToFolder -= AddNewsHeadlineGameObject;
        }

        private void Start()
        {
            EditorialManager.Instance.SetNewsFolderCanvas(gameObject);
        }

        private void AddNewsHeadlineGameObject(GameObject newsHeadlineGameObject)
        {
            NewsHeadline newsHeadlineComponent = newsHeadlineGameObject.GetComponent<NewsHeadline>();
            
            newsHeadlineComponent.SetNewsFolder(this);
            
            AddNewsHeadlineComponentToList(newsHeadlineGameObject.GetComponent<NewsHeadline>());
            
            PositionNewsHeadlinesByGivenIndex(_sentNewsHeadlines);
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
            
            TurnOn();
            ActionsManager.OnChangeToNewBias();
            ChangeBias(newsHeadline.GetChosenBiasIndex(), newsHeadline.GetShortBiasDescription());
            ActionsManager.OnChangeNewsHeadlineContent += newsHeadline.ChangeContent;
            newsHeadline.SetInFront(true);
        }

        private void RepositionAllNewsHeadlines()
        {
            PositionNewsHeadlinesByGivenIndex();
        }

        private void PositionNewsHeadlinesByGivenIndex(int totalIndicesToIgnore = 0)
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

        public void ReorderNewsHeadline(int newsHeadlineToSwitchIndex, int newChosenBiasIndex, String[] newShortBiasDescription)
        {
            RelevantStuffOnSwitchingFrontNewsHeadline(false);
            
            ChangeListOrderByGivenIndex(newsHeadlineToSwitchIndex);
            
            RelevantStuffOnSwitchingFrontNewsHeadline(true);
            
            ChangeBias(newChosenBiasIndex, newShortBiasDescription);
            
            PositionNewsHeadlinesByGivenIndex((_newsHeadlines.Count - 1) - newsHeadlineToSwitchIndex);
        }

        private void ChangeListOrderByGivenIndex(int newsHeadlineToSwitchIndex) 
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
                RelevantStuffOnSwitchingFrontNewsHeadline(false);
                
                ChangeListOrderByGivenIndex(0);

                NewsHeadline frontNewsHeadline = _newsHeadlines[0];
            
                ChangeBias(frontNewsHeadline.GetChosenBiasIndex(), frontNewsHeadline.GetShortBiasDescription());
                
                RelevantStuffOnSwitchingFrontNewsHeadline(true);
            }
            else
            {
                NewsHeadline currentNewsHeadline = _newsHeadlines[0];

                ActionsManager.OnChangeNewsHeadlineContent += currentNewsHeadline.ChangeContent;
                ChangeBias(currentNewsHeadline.GetChosenBiasIndex(), currentNewsHeadline.GetShortBiasDescription());
            }
            
            RepositionAllNewsHeadlines();
            
            CheckCurrentNewsHeadlinesSent();
            
            RedirectInComingNewsHeadlineToFolder();
        }

        private void CheckCurrentNewsHeadlinesSent()
        {
            if (_newsHeadlines.Count <= _sentNewsHeadlines)
            {
                TurnOff();
            }
            else
            {
                if (_active)
                {
                    return;    
                }
                TurnOn();
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
            return MathUtil.Map(index, 0, countOfTotalNewsHeadline, 
                FOLDER_MIN_Y_COORDINATE, FOLDER_MAX_Y_COORDINATE);
        }

        public int GetNewsHeadlinesLength()
        {
            return _newsHeadlines.Count;
        }

        public void SubtractOneToSentNewsHeadline()
        {
            _sentNewsHeadlines--;
            if (_newsHeadlines.Count > _sentNewsHeadlines && !_active)
            {
                TurnOn();
            }
        }

        private void SendNewsHeadlineToLayout()
        {
            RelevantStuffOnSwitchingFrontNewsHeadline(false);
            
            StartCoroutine(_newsHeadlines[0].SendToLayout());

            _newsHeadlines.RemoveAt(0);

            if (_newsHeadlines.Count == 0)
            {
                TurnOff();
                return;
            }
            
            ReindexFolderOrder(_newsHeadlines.Count);
            
            RelevantStuffOnSwitchingFrontNewsHeadline(true);

            NewsHeadline frontNewsHeadline = _newsHeadlines[0];
            
            ChangeBias(frontNewsHeadline.GetChosenBiasIndex(), frontNewsHeadline.GetShortBiasDescription());

            if (_newsHeadlines.Count <= _sentNewsHeadlines)
            {
                ActionsManager.OnChangeFolderOrderIndexWhenGoingToFolder();
                return;    
            }
            
            PositionNewsHeadlinesByGivenIndex(_sentNewsHeadlines);
        }

        private void ChangeBias(int newChosenBiasIndex, String[] newShortBiasDescription)
        {
            ActionsManager.OnChangeFrontNewsHeadline(newChosenBiasIndex);
            ActionsManager.OnSettingNewBiasDescription(newShortBiasDescription);
        }

        private void TurnOff()
        {
            _active = false;
            EditorialManager.Instance.DeactivateBiasCanvas();
            ActionsManager.OnSendNewsHeadline -= SendNewsHeadlineToLayout;
        }

        private void TurnOn()
        {
            _active = true;
            EditorialManager.Instance.ActivateBiasCanvas();
            ActionsManager.OnSendNewsHeadline += SendNewsHeadlineToLayout;
        }
    }
}
