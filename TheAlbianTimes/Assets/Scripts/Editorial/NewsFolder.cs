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
        [SerializeField] private List<GameObject> _instancedNewsHeadlineNotInSight = new ();

        [SerializeField]private int _sentNewsHeadlines;

        private bool _active; 
        private bool _allNewsHeadlinesSent = true; 

        private void OnEnable()
        {
            EventsManager.OnAddNewsHeadlineToFolder += AddNewsHeadlineGameObject;
        }

        private void OnDisable()
        {
            EventsManager.OnAddNewsHeadlineToFolder -= AddNewsHeadlineGameObject;
        }

        private void Start()
        {
            EditorialManager.Instance.SetNewsFolder(this);
        }

        private void AddNewsHeadlineGameObject(GameObject newsHeadlineGameObject)
        {
            NewsHeadline newsHeadlineComponent = newsHeadlineGameObject.GetComponent<NewsHeadline>();
            
            newsHeadlineComponent.SetNewsFolder(this);
            
            AddNewsHeadlineComponentToList(newsHeadlineGameObject.GetComponent<NewsHeadline>());

            _sentNewsHeadlines++;
            
            newsHeadlineComponent.AddToFolder();
            
            PositionNewsHeadlinesByGivenIndex(0);
        }

        private void AddNewsHeadlineComponentToList(NewsHeadline newsHeadline)
        {
            _allNewsHeadlinesSent = _newsHeadlines.Count == _sentNewsHeadlines;
            
            if (_sentNewsHeadlines == 0)
            {
                _newsHeadlines.Add(newsHeadline);
            }
            else
            {
                if (_newsHeadlines.Count == 1)
                {
                    ModifyInFrontProperties(false);
                }
                _newsHeadlines.Insert(_newsHeadlines.Count - _sentNewsHeadlines, newsHeadline);
                ReindexFolderOrder(_newsHeadlines.Count);
                RedirectInComingNewsHeadlineToFolder();
                CheckCurrentNewsHeadlinesSent();
            }

            if (!_allNewsHeadlinesSent)
            {
                newsHeadline.SetInFront(false);
                return;
            }
            
            TurnOn();
            EventsManager.OnChangeToNewBias();
            newsHeadline.SetInFront(true);
            ChangeBias(newsHeadline.GetChosenBiasIndex(), newsHeadline.GetShortBiasDescription());
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
                newsHeadline = _newsHeadlines[0];
                
                EventsManager.OnChangeToNewBias();
                EventsManager.OnSettingNewBiasDescription(newsHeadline.GetShortBiasDescription());
                countOfTotalNewsHeadline = newsHeadlineListLength;
            }
            
            Vector2 newOrigin;
            
            for (int i = 0; i < newsHeadlineListLength - totalIndicesToIgnore; i++)
            {
                newsHeadline = _newsHeadlines[i];
                
                newsHeadline.transform.SetSiblingIndex((newsHeadlineListLength - 1) - i);
                newsHeadline.SetFolderOrderIndex(i);
                if (i > (_newsHeadlines.Count - 1) - _sentNewsHeadlines)
                {
                    RedirectInComingNewsHeadlineToFolder();
                    continue;
                }
                newOrigin = new Vector2(0,GiveNewFolderYCoordinate(i, countOfTotalNewsHeadline));
                newsHeadline.SlideInFolder(newsHeadline.transform.localPosition, newOrigin);
                newsHeadline.SetOrigin(newOrigin);
            }
        }

        public void ReorderNewsHeadline(int newsHeadlineToSwitchIndex, int newChosenBiasIndex, String[] newShortBiasDescription)
        {
            ModifyInFrontProperties(false);
            
            ChangeListOrderByGivenIndex(newsHeadlineToSwitchIndex);
            
            ModifyInFrontProperties(true);
            
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

        private void ModifyInFrontProperties(bool isFront)
        {
            NewsHeadline newsHeadline = _newsHeadlines[0];

            newsHeadline.SetInFront(isFront);
        }

        public void ProcedureWhenSendNewsHeadlineToRewrite()
        {
            _sentNewsHeadlines++;

            NewsHeadline frontNewsHeadline;
            
            if (_newsHeadlines.Count > 1)
            {
                ModifyInFrontProperties(false);
                
                ChangeListOrderByGivenIndex(0);

                frontNewsHeadline = _newsHeadlines[0];
                
                ModifyInFrontProperties(true);
            
                ChangeBias(frontNewsHeadline.GetChosenBiasIndex(), frontNewsHeadline.GetShortBiasDescription());
            }
            else
            {
                frontNewsHeadline = _newsHeadlines[0];

                frontNewsHeadline.SetInFront(true);
                ChangeBias(frontNewsHeadline.GetChosenBiasIndex(), frontNewsHeadline.GetShortBiasDescription());
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
            if (_newsHeadlines.Count > _sentNewsHeadlines && !_active)
            {
                TurnOn();
            }
        }

        private void SendNewsHeadlineToLayout()
        {
            ModifyInFrontProperties(false);

            NewsHeadline frontNewsHeadline = _newsHeadlines[0];
            
            frontNewsHeadline.SendToLayout();
            
            _instancedNewsHeadlineNotInSight.Add(frontNewsHeadline.gameObject);

            _newsHeadlines.RemoveAt(0);

            if (_newsHeadlines.Count == 0)
            {
                TurnOff();
                return;
            }
            
            ReindexFolderOrder(_newsHeadlines.Count);
            
            ModifyInFrontProperties(true);

            frontNewsHeadline = _newsHeadlines[0];
            
            ChangeBias(frontNewsHeadline.GetChosenBiasIndex(), frontNewsHeadline.GetShortBiasDescription());

            if (_newsHeadlines.Count <= _sentNewsHeadlines)
            {
                EventsManager.OnChangeFolderOrderIndexWhenGoingToFolder();
                return;    
            }
            
            PositionNewsHeadlinesByGivenIndex(_sentNewsHeadlines);
        }

        private void ChangeBias(int newChosenBiasIndex, String[] newShortBiasDescription)
        {
            EventsManager.OnChangeFrontNewsHeadline(newChosenBiasIndex);
            EventsManager.OnSettingNewBiasDescription(newShortBiasDescription);
        }

        private void TurnOff()
        {
            _active = false;
            EditorialManager.Instance.DeactivateBiasCanvas();
            EventsManager.OnSendNewsHeadline -= SendNewsHeadlineToLayout;
        }

        private void TurnOn()
        {
            _active = true;
            EditorialManager.Instance.ActivateBiasCanvas();
            EventsManager.OnSendNewsHeadline += SendNewsHeadlineToLayout;
        }

        public int GetNewsHeadlinesLength()
        {
            return _newsHeadlines.Count;
        }

        public ref List<GameObject> GetInstancedNewsHeadlineNotInSightList()
        {
            return ref _instancedNewsHeadlineNotInSight;
        }
    }
}
