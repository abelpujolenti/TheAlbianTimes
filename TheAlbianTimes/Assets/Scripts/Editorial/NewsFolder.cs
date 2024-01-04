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
        private const float FOLDER_NEWS_MIN_Y_COORDINATE = -35;
        private const float FOLDER_NEWS_MAX_Y_COORDINATE = 11;
        
        [SerializeField] private RectTransform _rectTransform;
        
        [SerializeField]private List<NewsHeadline> _newsHeadlines = new ();

        [SerializeField]private int _newsHeadlinesHasToReturnToFolder;
        [SerializeField]private int _newsHeadlinesOutOfFolder;

        private readonly Vector3[] _corners = new Vector3[4];
        
        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;
        
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
            _rectTransform.GetWorldCorners(_corners);
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
            bool atStartNoOneInFolder = _newsHeadlines.Count == _newsHeadlinesHasToReturnToFolder;

            if (newsHeadline.WasOnFolder())
            {
                if (_newsHeadlinesHasToReturnToFolder == 0)
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
                    int indexToInsert = _newsHeadlines.Count - _newsHeadlinesHasToReturnToFolder;
                    _newsHeadlines.Insert(indexToInsert, newsHeadline);
                    ReindexFolderOrderInsideRange(indexToInsert, _newsHeadlines.Count);
                    RedirectInComingNewsHeadlineToFolder();
                }
            }
            else
            {
                if (_newsHeadlinesOutOfFolder == 0)
                {
                    newsHeadline.SetFolderOrderIndex(_newsHeadlines.Count - 1);
                }
                else
                {
                    if (atStartNoOneInFolder && _newsHeadlines.Count != 0)
                    {
                        ModifyInFrontProperties(false);
                    }
                    int indexToInsert = _newsHeadlines.Count - _newsHeadlinesHasToReturnToFolder;
                    _newsHeadlines.Insert(indexToInsert, newsHeadline);
                    ReindexFolderOrderInsideRange(indexToInsert, _newsHeadlines.Count);
                    RedirectInComingNewsHeadlineToFolder();
                }
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
                    ModifyInFrontProperties(false);
                    EditorialManager.Instance.TurnOffBiasContainer();
                }
                else
                {
                    frontNewsHeadline = _newsHeadlines[1];
                    ReorderNewsHeadline(0, frontNewsHeadline.GetSelectedBiasIndex(), frontNewsHeadline.GetBiasesNames(), frontNewsHeadline.GetBiasesDescription());   
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

            if (_folderEmpty)
            {
                EditorialManager.Instance.TurnOffBiasContainer();
                return;
            }
            ReindexFolderOrderInsideRange(0, _newsHeadlines.Count);
                
            ModifyInFrontProperties(true);
            
            frontNewsHeadline = _newsHeadlines[0];
            ChangeBias(frontNewsHeadline.GetSelectedBiasIndex(), frontNewsHeadline.GetBiasesNames(), frontNewsHeadline.GetBiasesDescription());
            PositionNewsHeadlinesByGivenIndex(_newsHeadlines.Count - _newsHeadlinesHasToReturnToFolder);
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
            ChangeBias(frontNewsHeadline.GetSelectedBiasIndex(), frontNewsHeadline.GetBiasesNames(), frontNewsHeadline.GetBiasesDescription());
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

        private void ChangeBias(int newSelectedBiasIndex, String[] biasesNames, String[] newBiasesDescriptions)
        {
            EventsManager.OnSettingNewBiases(biasesNames, newBiasesDescriptions);
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

        public bool IsFolderEmpty()
        {
            return _folderEmpty;
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
            position = Camera.main.ScreenToWorldPoint(position);
            return position.x > _containerMinCoordinates.x && position.x < _containerMaxCoordinates.x &&
                   position.y > _containerMinCoordinates.y && position.y < _containerMaxCoordinates.y;
        }

        public NewsHeadline GetCurrentHeadline()
        {
            return _newsHeadlines[0];
        }
    }
}
