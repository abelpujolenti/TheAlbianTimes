using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utility;

namespace Workspace.Editorial
{
    public class NewsFolder : MonoBehaviour
    {
        private const float FOLDER_NEWS_MIN_Y_COORDINATE = -35;
        private const float FOLDER_NEWS_MAX_Y_COORDINATE = 11;
        
        [SerializeField] private RectTransform _rectTransform;

        [SerializeField] private List<NewsHeadline> _newsHeadlines = new ();
        
        private readonly Vector3[] _corners = new Vector3[4];
        
        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;

        [SerializeField] private bool _isNewsModifying;
        private bool _dragging;

        private Camera _camera;

        [SerializeField] private int _newsHeadlinesOutOfFolder;

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
            _rectTransform.GetWorldCorners(_corners);
            _camera = Camera.main;
            SetContainerLimiters();
            EditorialManager.Instance.SetNewsFolder(this);
        }

        private void AddNewsHeadlineGameObject(GameObject newsHeadlineGameObject)
        {
            NewsHeadline newsHeadlineComponent = newsHeadlineGameObject.GetComponent<NewsHeadline>();
            
            newsHeadlineComponent.SetNewsFolder(this);
            
            AddNewsHeadlineComponent(newsHeadlineComponent);
        }

        public void AddNewsHeadlineComponent(NewsHeadline newsHeadline)
        {
            AddNewsHeadlineComponentToList(newsHeadline);
            
            newsHeadline.PrepareToAddToFolder();

            for (int i = 0; i < _newsHeadlines.Count; i++)
            {
                _newsHeadlines[i].transform.SetSiblingIndex((_newsHeadlines.Count - 1) - i);
            }
            
            PositionNewsHeadlinesByGivenIndex(_newsHeadlines.Count);
        }

        private void AddNewsHeadlineComponentToList(NewsHeadline newsHeadline)
        {
            _newsHeadlines.Add(newsHeadline);
            newsHeadline.SetFolderOrderIndex(_newsHeadlines.Count - 1);
            RedirectInComingNewsHeadlineToFolder();
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

        private void ModifyInFrontProperties(bool isFront)
        {
            NewsHeadline newsHeadline = _newsHeadlines[0];

            newsHeadline.SetInFront(isFront);
        }

        public void SendNewHeadlineToWriters()
        {
            _isNewsModifying = true;
            
            EditorialManager.Instance.TurnOffBiasContainer();
        }

        public void DropNewsHeadlineOutOfFolder()
        {
            _newsHeadlinesOutOfFolder++;
            _newsHeadlines.RemoveAt(0);
            
            ReindexFolderOrderInsideRange(_newsHeadlines.Count);

            if (_newsHeadlines.Count == 0)
            {
                EditorialManager.Instance.TurnOffBiasContainer();
                return;
            }     
            
            ModifyInFrontProperties(true);       
            
            NewsHeadline frontNewsHeadline = _newsHeadlines[0];
            ChangeBias(frontNewsHeadline.GetChosenBiasIndex(), frontNewsHeadline.GetBiasesNames(),
                frontNewsHeadline.GetBiasesDescription(), frontNewsHeadline.GetTotalBiasesToActivate());
            PositionNewsHeadlinesByGivenIndex(_newsHeadlines.Count);
        }

        public void ReorderNewsHeadline(int newsHeadlineToSwitchIndex, int newSelectedBiasIndex, 
            String[] biasesNames, String[] newBiasesDescriptions, int totalBiasesToActivate)
        {

            if (_isNewsModifying)
            {
                return;
            }
            
            ChangeListOrderByGivenIndex(newsHeadlineToSwitchIndex);
            
            ChangeBias(newSelectedBiasIndex, biasesNames, newBiasesDescriptions, totalBiasesToActivate);
            
            ReindexFolderOrderInsideRange(newsHeadlineToSwitchIndex + 1);
            PositionNewsHeadlinesByGivenIndex(newsHeadlineToSwitchIndex + 1);
        }

        private void ChangeListOrderByGivenIndex(int newsHeadlineToSwitchIndex) 
        {        
            ModifyInFrontProperties(false);
            
            NewsHeadline newsHeadlineToFront = _newsHeadlines[newsHeadlineToSwitchIndex];
                                                                   
            _newsHeadlines.RemoveAt(newsHeadlineToSwitchIndex);
                                                       
            _newsHeadlines.Insert(0, newsHeadlineToFront);   
            
            ModifyInFrontProperties(true);
        }

        private void ReindexFolderOrderInsideRange(int indexToEndReorder)
        {
            for (int i = 0; i < indexToEndReorder; i++)
            {
                ChangeFolderOrderIndex(i);
            }
            
            UpdateHeadlineShading();
        }

        private void ChangeFolderOrderIndex(int i)
        {
            _newsHeadlines[i].SetFolderOrderIndex(i);
        }

        public void CheckCurrentNewsHeadlinesSent()
        {
            if (_newsHeadlines.Count == 0)
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
                FOLDER_NEWS_MIN_Y_COORDINATE, FOLDER_NEWS_MAX_Y_COORDINATE);
        }

        public void ReturnNewsHeadline(NewsHeadline newsHeadline, int folderOrderIndex, bool wasOnFolder)
        {
            if (!wasOnFolder)
            {
                _newsHeadlinesOutOfFolder--;
            }
            else
            {
                _isNewsModifying = false;
            }
            
            newsHeadline.SetInFront(folderOrderIndex == 0);

            if (_dragging)
            {
                return;
            }
            
            EditorialManager.Instance.TurnOnBiasContainer();

            NewsHeadline frontNewsHeadline = _newsHeadlines[0];
            ChangeBias(frontNewsHeadline.GetChosenBiasIndex(), frontNewsHeadline.GetBiasesNames(), 
                frontNewsHeadline.GetBiasesDescription(), frontNewsHeadline.GetTotalBiasesToActivate());
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

        private void ChangeBias(int newChosenBiasIndex, String[] biasesNames, 
            String[] newBiasesDescriptions, int totalBiasesToActivate)
        {
            EventsManager.OnSettingNewBiases(biasesNames, newBiasesDescriptions, totalBiasesToActivate);
            EventsManager.OnChangeFrontNewsHeadline(newChosenBiasIndex);
        }

        public int GetNewsHeadlinesLength()
        {
            return _newsHeadlines.Count;
        }

        public int GetNewsInLayoutAmount()
        {
            return _newsHeadlinesOutOfFolder;
        }

        public void SetDragging(bool dragging)
        {
            _dragging = dragging;
        }

        private void SetContainerLimiters()
        {
            _containerMinCoordinates.x = _corners[0].x;
            _containerMaxCoordinates.y = _corners[1].y;
            _containerMaxCoordinates.x = _corners[2].x;
            _containerMinCoordinates.y = _corners[3].y;
        }

        public bool IsCoordinateInsideBounds(Vector3 position)
        {
            position = _camera.ScreenToWorldPoint(position);
            return position.x > _containerMinCoordinates.x && position.x < _containerMaxCoordinates.x &&
                   position.y > _containerMinCoordinates.y && position.y < _containerMaxCoordinates.y;
        }

        public NewsHeadline GetFrontHeadline()
        {
            return _newsHeadlines[0];
        }

        public bool IsNewsModifying()
        {
            return _isNewsModifying;
        }
    }
}