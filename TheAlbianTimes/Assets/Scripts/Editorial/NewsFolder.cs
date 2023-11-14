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
        
        [SerializeField]private List<NewsHeadline> _newsHeadlines = new ();

        [SerializeField] private GameObject _biasContainer; 

        private int _sentNewsHeadlines;
        
        private bool _noOneInFolder;
        private bool _dragging;

        private void OnEnable()
        {
            EventsManager.OnAddNewsHeadlineToFolder += AddNewsHeadlineGameObject;
            EventsManager.OnReturnNewsHeadlineToFolder += ReturnNewsHeadlineComponentToList;

        }

        private void OnDisable()
        {
            EventsManager.OnAddNewsHeadlineToFolder -= AddNewsHeadlineGameObject;
            EventsManager.OnReturnNewsHeadlineToFolder -= ReturnNewsHeadlineComponentToList;
        }

        private void Start()
        {
            EditorialManager.Instance.SetNewsFolder(this);
        }

        private void AddNewsHeadlineGameObject(GameObject newsHeadlineGameObject)
        {
            NewsHeadline newsHeadlineComponent = newsHeadlineGameObject.GetComponent<NewsHeadline>();
            
            newsHeadlineComponent.SetNewsFolder(this);
            
            ReturnNewsHeadlineComponentToList(newsHeadlineGameObject.GetComponent<NewsHeadline>());
        }

        private void ReturnNewsHeadlineComponentToList(NewsHeadline newsHeadline)
        {
            AddNewsHeadlineComponentToList(newsHeadline);

            _sentNewsHeadlines++;
            _noOneInFolder = _newsHeadlines.Count == _sentNewsHeadlines;
            
            newsHeadline.AddToFolder();
            
            RepositionAllNewsHeadlines();
        }

        private void AddNewsHeadlineComponentToList(NewsHeadline newsHeadline)
        {
            bool atStartNoOneInFolder = _newsHeadlines.Count == _sentNewsHeadlines;
            
            if (_sentNewsHeadlines == 0)
            {
                _newsHeadlines.Add(newsHeadline);
            }
            else
            {
                if (atStartNoOneInFolder)
                {
                    ModifyInFrontProperties(false);
                }
                _newsHeadlines.Insert(_newsHeadlines.Count - _sentNewsHeadlines, newsHeadline);
                ReindexFolderOrder(_newsHeadlines.Count);
                RedirectInComingNewsHeadlineToFolder();
                CheckCurrentNewsHeadlinesSent();
            }

            if (!atStartNoOneInFolder)
            {
                return;
            }
            
            TurnOn();
            ChangeBias(newsHeadline.GetChosenBiasIndex(), newsHeadline.GetBiasesNames(), newsHeadline.GetBiasesDescription());
        }

        private void RepositionAllNewsHeadlines()
        {
            PositionNewsHeadlinesByGivenIndex(0);
        }

        private void PositionNewsHeadlinesByGivenIndex(int totalIndicesToIgnore)
        {
            int newsHeadlineListLength = _newsHeadlines.Count;
            int countOfTotalNewsHeadline = newsHeadlineListLength - 1;

            NewsHeadline newsHeadline;
            
            if (newsHeadlineListLength == 1)
            {
                countOfTotalNewsHeadline = newsHeadlineListLength;
            }
            
            Vector2 newOrigin;
            
            for (int i = 0; i < newsHeadlineListLength - totalIndicesToIgnore; i++)
            {
                newsHeadline = _newsHeadlines[i];
                newsHeadline.transform.SetSiblingIndex((countOfTotalNewsHeadline) - i);
                newsHeadline.SetFolderOrderIndex(i);
                if (i > (_newsHeadlines.Count - 1) - _sentNewsHeadlines)
                {
                    RedirectInComingNewsHeadlineToFolder();
                    continue;
                }
                newOrigin = new Vector2(0, GiveNewFolderYCoordinate(i, countOfTotalNewsHeadline));
                newsHeadline.SlideInFolder(newsHeadline.transform.localPosition, newOrigin);
                newsHeadline.SetOrigin(newOrigin);
            }
        }

        public void ReorderNewsHeadline(int newsHeadlineToSwitchIndex, int newChosenBiasIndex, String[] biasesNames, String[] newBiasesDescriptions)
        {
            ModifyInFrontProperties(false);
            
            ChangeListOrderByGivenIndex(newsHeadlineToSwitchIndex);
            
            ModifyInFrontProperties(true);
            
            ChangeBias(newChosenBiasIndex, biasesNames, newBiasesDescriptions);

            if (newsHeadlineToSwitchIndex == 0)
            {
                RepositionAllNewsHeadlines();
                return;
            }
            
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

        private void ModifyInFrontProperties(bool isFront)
        {
            NewsHeadline newsHeadline = _newsHeadlines[0];

            newsHeadline.SetInFront(isFront);
        }

        public void ProcedureOnChangeBias()
        {
            _sentNewsHeadlines++;
            _noOneInFolder = _newsHeadlines.Count == _sentNewsHeadlines;
            
            if (_newsHeadlines.Count != 1)
            {
                NewsHeadline frontNewsHeadline = _newsHeadlines[1];
                
                ReorderNewsHeadline(0, frontNewsHeadline.GetChosenBiasIndex(), frontNewsHeadline.GetBiasesNames(), frontNewsHeadline.GetBiasesDescription());
                
                CheckCurrentNewsHeadlinesSent();
            
                RedirectInComingNewsHeadlineToFolder();
            }
            else
            {
                ModifyInFrontProperties(false);
                TurnOff();
            }
        }

        private void CheckCurrentNewsHeadlinesSent()
        {
            if (_noOneInFolder)
            {
                TurnOff();
            }
            else
            {
                TurnOn();
            }
        }

        private void RedirectInComingNewsHeadlineToFolder()
        {
            if (EventsManager.OnChangeFolderOrderIndexWhenGoingToFolder != null)
            {
                EventsManager.OnChangeFolderOrderIndexWhenGoingToFolder();
            }
        }

        public float GiveNewFolderYCoordinate(int index, int countOfTotalNewsHeadline)
        {
            return MathUtil.Map(index, 0, countOfTotalNewsHeadline, 
                FOLDER_MIN_Y_COORDINATE, FOLDER_MAX_Y_COORDINATE);
        }

        public void SubtractOneToSentNewsHeadline()
        {
            _sentNewsHeadlines--;
            _noOneInFolder = _newsHeadlines.Count == _sentNewsHeadlines;
            
            if (!_noOneInFolder && !_dragging)
            {
                TurnOn();
            }
        }

        public void SendNewsHeadlineToLayout()
        {
            _newsHeadlines.RemoveAt(0);
            _noOneInFolder = _newsHeadlines.Count == _sentNewsHeadlines;
            
            if (_newsHeadlines.Count != 0)
            {
                ReindexFolderOrder(_newsHeadlines.Count);
                ModifyInFrontProperties(true);

                NewsHeadline frontNewsHeadline = _newsHeadlines[0];
            
                ChangeBias(frontNewsHeadline.GetChosenBiasIndex(), frontNewsHeadline.GetBiasesNames(), frontNewsHeadline.GetBiasesDescription());
            }

            if (_noOneInFolder)
            {
                TurnOff();
                return;
            }
            
            PositionNewsHeadlinesByGivenIndex(_sentNewsHeadlines);
        }

        private void ChangeBias(int newChosenBiasIndex, String[] biasesNames, String[] newBiasesDescriptions)
        {
            EventsManager.OnSettingNewBiases(biasesNames, newBiasesDescriptions);
            EventsManager.OnChangeFrontNewsHeadline(newChosenBiasIndex);
        }

        public void TurnOff()
        {
            _biasContainer.SetActive(false);
        }

        public void TurnOn()
        {
            if (_biasContainer.activeSelf)
            {
                return;
            }
            _biasContainer.SetActive(true);
        }

        public int GetNewsHeadlinesLength()
        {
            return _newsHeadlines.Count;
        }

        public void SetDragging(bool dragging)
        {
            _dragging = dragging;
        }
    }
}
