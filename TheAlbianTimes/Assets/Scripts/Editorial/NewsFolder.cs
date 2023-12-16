using System;
using System.Collections.Generic;
using System.Linq;
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

        private int _sentNewsHeadlines;
        
        private bool _folderEmpty;
        private bool _dragging;

        private void OnEnable()
        {
            EventsManager.OnAddNewsHeadlineToFolder += AddNewsHeadlineGameObject;
            EventsManager.OnReturnNewsHeadlineToFolder += AddNewsHeadlineComponent;

        }

        private void OnDisable()
        {
            EventsManager.OnAddNewsHeadlineToFolder -= AddNewsHeadlineGameObject;
            EventsManager.OnReturnNewsHeadlineToFolder -= AddNewsHeadlineComponent;
        }

        private void Start()
        {
            EditorialManager.Instance.SetNewsFolder(this);
        }

        private void AddNewsHeadlineGameObject(GameObject newsHeadlineGameObject)
        {
            NewsHeadline newsHeadlineComponent = newsHeadlineGameObject.GetComponent<NewsHeadline>();
            
            newsHeadlineComponent.SetNewsFolder(this);
            
            AddNewsHeadlineComponent(newsHeadlineGameObject.GetComponent<NewsHeadline>(), true);
        }

        private void AddNewsHeadlineComponent(NewsHeadline newsHeadline, bool start)
        {
            AddNewsHeadlineComponentToList(newsHeadline);

            _sentNewsHeadlines++;
            _folderEmpty = _newsHeadlines.Count == _sentNewsHeadlines;
            
            newsHeadline.AddToFolder();

            for (int i = 0; i < _newsHeadlines.Count; i++)
            {
                _newsHeadlines[i].transform.SetSiblingIndex((_newsHeadlines.Count - 1) - i);
            }

            if (start)
            {
                return;
            }
            
            PositionNewsHeadlinesByGivenIndex((_newsHeadlines.Count - _sentNewsHeadlines) + 1);
        }

        private void AddNewsHeadlineComponentToList(NewsHeadline newsHeadline)
        {
            bool atStartNoOneInFolder = _newsHeadlines.Count == _sentNewsHeadlines;
            
            if (_sentNewsHeadlines == 0)
            {
                _newsHeadlines.Add(newsHeadline);
                newsHeadline.SetFolderOrderIndex(_newsHeadlines.Count - 1);
            }
            else
            {
                if (atStartNoOneInFolder)
                {
                    ModifyInFrontProperties(false);
                }
                int indexToInsert = _newsHeadlines.Count - _sentNewsHeadlines;
                _newsHeadlines.Insert(indexToInsert, newsHeadline);
                ReindexFolderOrderInsideRange(indexToInsert, _newsHeadlines.Count);
                RedirectInComingNewsHeadlineToFolder();
            }

            if (!atStartNoOneInFolder)
            {
                return;
            }
            
            EditorialManager.Instance.TurnOnBiasContainer();
            ChangeBias(newsHeadline.GetChosenBiasIndex(), newsHeadline.GetBiasesNames(), newsHeadline.GetBiasesDescription());
        }

        private void PositionNewsHeadlinesByGivenIndex(int totalIndicesToPosition)
        {
            int newsHeadlineListLength = _newsHeadlines.Count;
            int countOfTotalNewsHeadline = newsHeadlineListLength - 1;
            
            if (newsHeadlineListLength == 1)
            {
                countOfTotalNewsHeadline = newsHeadlineListLength;
            }
            
            NewsHeadline newsHeadline;
            Vector2 newOrigin;
            
            for (int i = 0; i < totalIndicesToPosition; i++)
            {
                newsHeadline = _newsHeadlines[i];
                newsHeadline.transform.SetSiblingIndex((newsHeadlineListLength - 1) - i);
                newOrigin = new Vector2(0, GiveNewFolderYCoordinate(i, countOfTotalNewsHeadline));
                newsHeadline.SlideInFolder(newsHeadline.transform.localPosition, newOrigin);
                newsHeadline.SetOrigin(newOrigin);
            }

            UpdateHeadlineShading();
        }

        public void ReorderNewsHeadline(int newsHeadlineToSwitchIndex, int newSelectedBiasIndex, String[] biasesNames, String[] newBiasesDescriptions)
        {
            ChangeListOrderByGivenIndex(newsHeadlineToSwitchIndex);
            
            ChangeBias(newSelectedBiasIndex, biasesNames, newBiasesDescriptions);

            if (newsHeadlineToSwitchIndex == 0)
            {
                ReindexFolderOrderInsideRange(0, _newsHeadlines.Count);
                RedirectInComingNewsHeadlineToFolder();
                PositionNewsHeadlinesByGivenIndex(_newsHeadlines.Count - _sentNewsHeadlines);
                return;
            }
            
            ReindexFolderOrderInsideRange(0, newsHeadlineToSwitchIndex + 1);
            PositionNewsHeadlinesByGivenIndex(newsHeadlineToSwitchIndex + 1);
        }

        private void ChangeListOrderByGivenIndex(int newsHeadlineToSwitchIndex) 
        {        
            ModifyInFrontProperties(false);
            
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
            
            ModifyInFrontProperties(true);
        }

        private void ReindexFolderOrderInsideRange(int indexToStartReorder , int indexToEndReorder)
        {
            for (int i = indexToStartReorder; i < indexToEndReorder; i++)
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
            _folderEmpty = _newsHeadlines.Count == _sentNewsHeadlines;
            
            if (_newsHeadlines.Count != 1)
            {
                CheckCurrentNewsHeadlinesSent();

                if (!_folderEmpty)
                {
                    NewsHeadline frontNewsHeadline = _newsHeadlines[1];
                    ReorderNewsHeadline(0, frontNewsHeadline.GetSelectedBiasIndex(), frontNewsHeadline.GetBiasesNames(), frontNewsHeadline.GetBiasesDescription());    
                }
                else
                {
                    if (_sentNewsHeadlines > 1)
                    {
                        RedirectInComingNewsHeadlineToFolder();    
                    }
                    PositionNewsHeadlinesByGivenIndex(_newsHeadlines.Count - _sentNewsHeadlines);
                }
            }
            else
            {
                ModifyInFrontProperties(false);
                EditorialManager.Instance.TurnOffBiasContainer();
            }
        }

        private void CheckCurrentNewsHeadlinesSent()
        {
            if (_folderEmpty)
            {
                EditorialManager.Instance.TurnOffBiasContainer();
            }
            else
            {
                EditorialManager.Instance.TurnOnBiasContainer();
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

        public void SubtractOneToSentNewsHeadline(NewsHeadline newsHeadline, int folderOrderIndex)
        {
            bool atStartFolderEmpty = _newsHeadlines.Count == _sentNewsHeadlines;
            _sentNewsHeadlines--;
            _folderEmpty = _newsHeadlines.Count == _sentNewsHeadlines;
            
            newsHeadline.SetInFront(folderOrderIndex == 0);

            if (_dragging)
            {
                return;
            }
            
            EditorialManager.Instance.TurnOnBiasContainer();

            if (!atStartFolderEmpty)
            {
                return;
            }

            NewsHeadline frontNewsHeadline = _newsHeadlines[0];
            ChangeBias(frontNewsHeadline.GetChosenBiasIndex(), frontNewsHeadline.GetBiasesNames(), frontNewsHeadline.GetBiasesDescription());
        }

        public void SendNewsHeadlineToLayout()
        {
            NewsHeadline frontNewsHeadline = _newsHeadlines[0];
            
            frontNewsHeadline.SetSend(true);
            
            _newsHeadlines.RemoveAt(0);
            _folderEmpty = _newsHeadlines.Count == _sentNewsHeadlines;

            CheckCurrentNewsHeadlinesSent();
            
            if (_newsHeadlines.Count == 0)
            {
                return;
            }
            
            ReindexFolderOrderInsideRange(0, _newsHeadlines.Count);
                
            ModifyInFrontProperties(true);

            if (_sentNewsHeadlines > 0)
            {
                RedirectInComingNewsHeadlineToFolder();
                
                if (_folderEmpty)
                {
                    EditorialManager.Instance.TurnOffBiasContainer();
                    return;
                }
            }
            
            frontNewsHeadline = _newsHeadlines[0];
            ChangeBias(frontNewsHeadline.GetSelectedBiasIndex(), frontNewsHeadline.GetBiasesNames(), frontNewsHeadline.GetBiasesDescription());
            
            PositionNewsHeadlinesByGivenIndex(_newsHeadlines.Count - _sentNewsHeadlines);
        }

        private void UpdateHeadlineShading()
        {
            int s = 0;
            for (int i = 0; i < _newsHeadlines.Count; i++)
            {
                if (_newsHeadlines[i].isActiveAndEnabled)
                {
                    _newsHeadlines[i].UpdateShading(s);
                    s++;
                }
            }
        }

        private void ChangeBias(int newChosenBiasIndex, String[] biasesNames, String[] newBiasesDescriptions)
        {
            EventsManager.OnSettingNewBiases(biasesNames, newBiasesDescriptions);
            EventsManager.OnChangeFrontNewsHeadline(newChosenBiasIndex);
        }

        public int GetNewsHeadlinesLength()
        {
            return _newsHeadlines.Count;
        }

        public void SetDragging(bool dragging)
        {
            _dragging = dragging;
        }

        public bool IsFolderEmpty()
        {
            return _folderEmpty;
        }

        public NewsHeadline GetCurrentHeadline()
        {
            return _newsHeadlines[0];
        }
    }
}
