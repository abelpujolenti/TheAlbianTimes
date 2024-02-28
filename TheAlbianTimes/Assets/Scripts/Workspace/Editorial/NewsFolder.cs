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
        
        [SerializeField] private Tray _tray; 

        [SerializeField]private int _newsHeadlinesHasToReturnToFolder;
        [SerializeField]private int _newsHeadlinesOutOfFolder;

        [SerializeField]private List<NewsHeadline> _newsHeadlines = new ();
        
        private readonly Vector3[] _corners = new Vector3[4];
        
        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;
        
        private bool _folderEmpty;
        private bool _dragging;

        private Camera _camera;

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
        }

        private void AddNewsHeadlineGameObject(GameObject newsHeadlineGameObject)
        {
            NewsHeadline newsHeadlineComponent = newsHeadlineGameObject.GetComponent<NewsHeadline>();
            
            newsHeadlineComponent.SetNewsFolder(this);
            
            AddNewsHeadlineComponent(newsHeadlineComponent, true);
        }

        public void AddNewsHeadlineComponent(NewsHeadline newsHeadline, bool allAtOnce)
        {
            AddNewsHeadlineComponentToList(newsHeadline);
            _newsHeadlinesHasToReturnToFolder++;
            _folderEmpty = _newsHeadlines.Count == _newsHeadlinesHasToReturnToFolder;
            
            newsHeadline.PrepareToAddToFolder();

            for (int i = 0; i < _newsHeadlines.Count; i++)
            {
                _newsHeadlines[i].transform.SetSiblingIndex((_newsHeadlines.Count - 1) - i);
            }

            if (allAtOnce)
            {
                return;
            }
            
            PositionNewsHeadlinesByGivenIndex((_newsHeadlines.Count - _newsHeadlinesHasToReturnToFolder) + 1);
        }

        private void AddNewsHeadlineComponentToList(NewsHeadline newsHeadline)
        {
            if (newsHeadline.WasOnFolder())
            {
                if (_newsHeadlinesHasToReturnToFolder == 0)
                {
                    _newsHeadlines.Add(newsHeadline);
                    newsHeadline.SetFolderOrderIndex(_newsHeadlines.Count - 1);
                    return;
                }

                if (_newsHeadlines.Count == _newsHeadlinesHasToReturnToFolder)
                {
                    ModifyInFrontProperties(false);
                }
                InsertNewsHeadlineOnIndex(newsHeadline);
                return;
            }

            if (_newsHeadlinesOutOfFolder == 0)
            {
                newsHeadline.SetFolderOrderIndex(_newsHeadlines.Count - 1);
                return;
            }

            if (_newsHeadlines.Count == _newsHeadlinesHasToReturnToFolder && _newsHeadlines.Count != 0)
            {
                ModifyInFrontProperties(false);
            }
            InsertNewsHeadlineOnIndex(newsHeadline);
        }

        private void InsertNewsHeadlineOnIndex(NewsHeadline newsHeadline)
        {
            int indexToInsert = _newsHeadlines.Count - _newsHeadlinesHasToReturnToFolder;
            _newsHeadlines.Insert(indexToInsert, newsHeadline);
            ReindexFolderOrderInsideRange(indexToInsert, _newsHeadlines.Count);
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

        public void DropNewsHeadlineOutOfFolder(bool toModify)
        {
            NewsHeadline frontNewsHeadline;
            
            if (toModify)
            {
                _newsHeadlinesHasToReturnToFolder++;
                _folderEmpty = _newsHeadlines.Count == _newsHeadlinesHasToReturnToFolder;

                if (_folderEmpty)
                {
                    if (_newsHeadlinesHasToReturnToFolder > 1)
                    {
                        RedirectInComingNewsHeadlineToFolder();    
                    }
                    _tray.Hide(null);
                    ModifyInFrontProperties(false);
                    EditorialManager.Instance.TurnOffBiasContainer();
                }
                else
                {
                    frontNewsHeadline = _newsHeadlines[1];
                    ReorderNewsHeadline(0, frontNewsHeadline.GetSelectedBiasIndex(), frontNewsHeadline.GetBiasesNames(), 
                        frontNewsHeadline.GetBiasesDescription(), frontNewsHeadline.GetTotalBiasesToActivate());   
                    PositionNewsHeadlinesByGivenIndex(_newsHeadlines.Count - _newsHeadlinesHasToReturnToFolder);
                }
                return;
            }
            
            _newsHeadlinesOutOfFolder++;
            _newsHeadlines.RemoveAt(0);
            
            _folderEmpty = _newsHeadlines.Count == _newsHeadlinesHasToReturnToFolder;

            if (_newsHeadlinesHasToReturnToFolder > 0)
            {
                RedirectInComingNewsHeadlineToFolder();
            }
            
            ReindexFolderOrderInsideRange(0, _newsHeadlines.Count);

            if (_folderEmpty)
            {
                _tray.Hide(null);
                EditorialManager.Instance.TurnOffBiasContainer();
                return;
            }     
            
            ModifyInFrontProperties(true);       
            
            frontNewsHeadline = _newsHeadlines[0];
            ChangeBias(frontNewsHeadline.GetSelectedBiasIndex(), frontNewsHeadline.GetBiasesNames(),
                frontNewsHeadline.GetBiasesDescription(), frontNewsHeadline.GetTotalBiasesToActivate());
            PositionNewsHeadlinesByGivenIndex(_newsHeadlines.Count - _newsHeadlinesHasToReturnToFolder);
        }

        public void ReorderNewsHeadline(int newsHeadlineToSwitchIndex, int newSelectedBiasIndex, 
            String[] biasesNames, String[] newBiasesDescriptions, int totalBiasesToActivate)
        {
            ChangeListOrderByGivenIndex(newsHeadlineToSwitchIndex);
            
            ChangeBias(newSelectedBiasIndex, biasesNames, newBiasesDescriptions, totalBiasesToActivate);

            if (newsHeadlineToSwitchIndex == 0)
            {
                ReindexFolderOrderInsideRange(0, _newsHeadlines.Count);
                RedirectInComingNewsHeadlineToFolder();
                PositionNewsHeadlinesByGivenIndex(_newsHeadlines.Count - _newsHeadlinesHasToReturnToFolder);
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

            if (indexToStartReorder <= 0)
            {
                return;
            }
            UpdateHeadlineShading();
        }

        private void ChangeFolderOrderIndex(int i)
        {
            _newsHeadlines[i].SetFolderOrderIndex(i);
        }

        public void CheckCurrentNewsHeadlinesSent()
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
                FOLDER_NEWS_MIN_Y_COORDINATE, FOLDER_NEWS_MAX_Y_COORDINATE);
        }

        public void ReturnNewsHeadline(NewsHeadline newsHeadline, int folderOrderIndex, bool wasOnFolder)
        {
            bool atStartFolderEmpty = _newsHeadlines.Count == _newsHeadlinesHasToReturnToFolder;

            if (!wasOnFolder)
            {
                _newsHeadlinesOutOfFolder--;
            }
            _newsHeadlinesHasToReturnToFolder--;
            
            _folderEmpty = _newsHeadlines.Count == _newsHeadlinesHasToReturnToFolder;
            
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
            ChangeBias(frontNewsHeadline.GetSelectedBiasIndex(), frontNewsHeadline.GetBiasesNames(), 
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

        private void ChangeBias(int newSelectedBiasIndex, String[] biasesNames, 
            String[] newBiasesDescriptions, int totalBiasesToActivate)
        {
            EventsManager.OnSettingNewBiases(biasesNames, newBiasesDescriptions, totalBiasesToActivate);
            EventsManager.OnChangeFrontNewsHeadline(newSelectedBiasIndex);
        }

        public int GetNewsHeadlinesLength()
        {
            return _newsHeadlines.Count;
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
    }
}
