using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Workspace.Notebook;
using Workspace.Notebook.Pages;

namespace Managers
{
    public class NotebookManager : MonoBehaviour
    {
        private static NotebookManager _instance;
        public static NotebookManager Instance => _instance;
        
        private const string PATH_COUNTRIES_CONTENT_CONTAINER = "/Json/Notebook/CountriesContainer.json";
        //private const string PATH_INTERNATIONALS_CONTENT_CONTAINER = "/Json/Notebook/InternationalsContainer.json";
        private const string PATH_PEOPLE_CONTENT_CONTAINER = "/Json/Notebook/PeopleContainer.json";

        private const float BOOKMARK_WIDTH = 22;

        private const int MAP_RANGE_OF_PAGES = 2;
        private const int COUNTRY_RANGE_OF_PAGES = 6;
        //private const int INTERNATIONAL_RANGE_OF_PAGES = 1;
        private const int PERSON_RANGE_OF_PAGES = 1;
        
        private readonly Vector2 _notebookBookmarkRootPosition = new Vector2(10, 0);
        private readonly Vector2 _notebookBookmarkDifferencePosition = new Vector2(45, 0);

        [SerializeField] private GameObject _notebookBookmarkPrefab;
        [SerializeField] private GameObject _coverPrefab;
        [SerializeField] private GameObject[] _mapPagesPrefabs;
        [SerializeField] private GameObject[] _countryPagesPrefabs;
        //[SerializeField] private GameObject[] _internationalPagesPrefabs;
        [SerializeField] private GameObject[] _personPagesPrefabs;

        [SerializeField] private Transform _pageMarkers;
        [SerializeField] private Transform _activePageMarker;
        
        private List<NotebookBookmark> _bookmarks = new List<NotebookBookmark>();

        [SerializeField] private NotebookPage _leftPage;
        [SerializeField] private NotebookPage _flipPage;
        [SerializeField] private NotebookPage _rightPage;

        [SerializeField] private Notebook _notebook;

        private NotebookBookmark _currentBookmark;

        private Dictionary<int, BaseNotebookPage> _notebookPages = new Dictionary<int, BaseNotebookPage>();
        private Dictionary<string, int> _countryIndices = new Dictionary<string, int>();
        private Dictionary<NotebookContentType, int> _notebookIndices = new Dictionary<NotebookContentType, int>();

        private Dictionary<NotebookContentType, NotebookContentPage[]> _notebookContentPages =
            new Dictionary<NotebookContentType, NotebookContentPage[]>();

        private GameObject _cover;

        private int _currentPageNumber;
        private int _totalPages;

        private bool _noteBookOpen;

        private Action _midPointFlip = () => { };
        private Action _endFlip = () => { };

        private Func<int, int, bool> _shouldBeOnRightSide =>
            (index, nextPage) => _bookmarks[index].IsOnRightSide() && nextPage > _notebookIndices[(NotebookContentType)index];
        
        private Func<int, int, bool> _shouldBeOnLeftSide =>
            (index, nextPage) => !_bookmarks[index].IsOnRightSide() && nextPage <= _notebookIndices[(NotebookContentType)index];

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this; 
                LoadNotebookContents();
                _cover = Instantiate(_coverPrefab);
                _leftPage.ChangeContent(_cover);
                _cover.transform.localRotation = new Quaternion(0, 180, 0, 1);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeNotebookContentPages(NotebookContentType notebookContentType, GameObject[] prefabs)
        {
            NotebookContentPage[] notebookContentPages = new NotebookContentPage[prefabs.Length];

            for (int i = 0; i < notebookContentPages.Length; i++)
            {
                notebookContentPages[i] = Instantiate(prefabs[i].GetComponent<NotebookContentPage>());
            }
            
            _notebookContentPages.Add(notebookContentType, notebookContentPages);
        }

        private TContent[] LoadNotebookContentsFromJson <TContainer, TContent>(string path)
            where TContainer : BaseNotebookContainer
            where TContent : BaseNotebookContent
        {
            TContainer container = LoadBaseContainer<TContainer>(path);

            if (container == null)
            {
                return null;
            }

            TContent[] contents;

            try
            {
                contents = (TContent[])container.GetContent();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }

            return contents;
        }

        private TContainer LoadBaseContainer<TContainer>(string path)
        {
            string json = LoadFromJson(path);

            if (json == null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<TContainer>(json);
        }

        private string LoadFromJson(string path)
        {
            return !File.Exists(Application.streamingAssetsPath + path) ? null : FileManager.LoadJsonFile(path);
        }

        private void LoadNotebookContents()
        {
            CountryContent[] countriesContent =
                LoadNotebookContentsFromJson<CountriesContainer, CountryContent>(PATH_COUNTRIES_CONTENT_CONTAINER);
            /*InternationalContent[] internationalsContent =
                LoadNotebookContentsFromJson<InternationalsContainer, InternationalContent>(PATH_INTERNATIONALS_CONTENT_CONTAINER);*/
            PersonContent[] peopleContent =
                LoadNotebookContentsFromJson<PeopleContainer, PersonContent>(PATH_PEOPLE_CONTENT_CONTAINER);
            
            ReservePagesForMap();
            SaveCountriesContentsInDictionary(countriesContent);
            //SaveInternationalsContentsInDictionary(internationalsContent);
            SavePeopleContentsInDictionary(peopleContent);

            _totalPages = _notebookPages.Count;
        }

        private void InstantiateNotebookBookmark(NotebookContentType notebookContentType)
        {
            NotebookBookmark notebookBookmark = Instantiate(_notebookBookmarkPrefab, _notebook.gameObject.transform.GetChild(0)).GetComponent<NotebookBookmark>();
            notebookBookmark.SetPage(_notebookPages.Count);
            notebookBookmark.SetPageMarkerActiveParent(_activePageMarker);
            
            notebookBookmark.gameObject.transform.localPosition = _notebookBookmarkRootPosition + _notebookBookmarkDifferencePosition * _notebookIndices.Count;
            
            _notebookIndices.Add(notebookContentType, _notebookPages.Count);
            
            _bookmarks.Add(notebookBookmark);
        }

        private void ReservePagesForMap()
        {
            InstantiateNotebookBookmark(NotebookContentType.MAP);
            
            InitializeNotebookContentPages(NotebookContentType.MAP, _mapPagesPrefabs);
            
            _notebookPages.Add(_notebookPages.Count, null);
            _notebookPages.Add(_notebookPages.Count, null);
        }

        private void SaveCountriesContentsInDictionary(CountryContent[] contents)
        {
            InitializeNotebookContentPages(NotebookContentType.COUNTRY, _countryPagesPrefabs);

            int maxCountries = contents.Length;
            
            int round = GameManager.Instance.GetRound();

            if (round < 3)
            {
                maxCountries = 3;
            }
            else if (round == 3)
            {
                maxCountries = 4;
            }
            else if (round == 4)
            {
                maxCountries = 6;
            }
            else if (round == 5)
            {
                maxCountries = 8;
            }

            for (int i = 0; i < maxCountries; i++)
            {
                CountryContent content = contents[i];
                CountryContentPage0 page0 = new CountryContentPage0
                {
                    countryName = content.countryName,
                    description = content.description
                };
                
                CountryContentPage1 page1 = new CountryContentPage1
                {
                    countryName = content.countryName,
                    history = content.history
                };

                CountryContentPage2 page2 = new CountryContentPage2
                {
                    countryName = content.countryName,
                    importantPeople = content.importantPeople
                };

                CountryContentPage3 page3 = new CountryContentPage3
                {
                    countryName = content.countryName,
                    organizations = content.organizations
                };

                CountryContentPage4 page4 = new CountryContentPage4
                {
                    countryName = content.countryName,
                    ongoingConflicts = content.ongoingConflicts
                };
                
                _countryIndices.Add(content.countryName, _notebookPages.Count);
                _notebookPages.Add(_notebookPages.Count, page0);
                _notebookPages.Add(_notebookPages.Count, page1);
                _notebookPages.Add(_notebookPages.Count, page2);
                _notebookPages.Add(_notebookPages.Count, page3);
                _notebookPages.Add(_notebookPages.Count, page4);
                
                FillLastPageIfUneven();
            }

            MapPage0 mapPage0 = new MapPage0
            {
                hetiaClick = () => { MoveToPage(_countryIndices["Hetia"]); },
                terkanClick = () => { MoveToPage(_countryIndices["Terkan"]); },
                albiaClick = () => { MoveToPage(_countryIndices["Albia"]); },
                dalmeClick = () => { MoveToPage(_countryIndices["Dalme"]); },
                madiaClick = () => { MoveToPage(_countryIndices["Madia"]); }
            };
            MapPage1 mapPage1 = new MapPage1
            {
                xayaClick = () => { MoveToPage(_countryIndices["Xaya"]); },
                suokaClick = () => { MoveToPage(_countryIndices["Suoka"]); },
                zuaniaClick = () => { MoveToPage(_countryIndices["Zuania"]); },
                rekkaClick =  () => { MoveToPage(_countryIndices["Rekka"]); }
            };

            _notebookPages[0] = mapPage0;
            _notebookPages[1] = mapPage1;
        }

        /*private void SaveInternationalsContentsInDictionary(InternationalContent[] contents)
        {
            _notebookIndices.Add(NotebookContentType.INTERNATIONAL, _notebookPages.Count);

            foreach (InternationalContent content in contents)
            {
                InternationalContentPage0 page0 = new InternationalContentPage0
                {
                    name = content.name
                };

                _notebookPages.Add(_notebookPages.Count, page0);
            }

            FillLastPageIfUneven();
        }*/

        private void SavePeopleContentsInDictionary(PersonContent[] contents)
        {
            InstantiateNotebookBookmark(NotebookContentType.PERSON);
            
            InitializeNotebookContentPages(NotebookContentType.PERSON, _personPagesPrefabs);
            
            foreach (PersonContent content in contents)
            {
                PersonContentPage0 page0 = new PersonContentPage0
                {
                    name = content.name,
                    descriptions =  content.descriptions
                };
                
                _notebookPages.Add(_notebookPages.Count, page0);
            }
            
            FillLastPageIfUneven();
        }

        private void FillLastPageIfUneven()
        {
            if (_notebookPages.Count % 2 == 0)
            {
                return;
            }
            
            _notebookPages.Add(_notebookPages.Count, null);
        }

        public void NextPage()
        {
            int auxCurrentPage = _currentPageNumber;
            auxCurrentPage += 2;
            
            if (auxCurrentPage >= _totalPages)
            {
                return;
            }

            MoveToPage(auxCurrentPage);
        }

        public void PreviousPage()
        {
            int auxCurrentPage = _currentPageNumber;
            auxCurrentPage -= 2;
            
            if (auxCurrentPage < 0)
            {
                return;
            }

            MoveToPage(auxCurrentPage);
        } 

        private void CheckBookmark(int page) 
        {
            RemoveActiveBookmark();

            foreach (KeyValuePair<NotebookContentType, int> bookmarkPage in _notebookIndices)
            {
                if (page != bookmarkPage.Value)
                {
                    continue;
                }
                _currentBookmark = _bookmarks[(int)bookmarkPage.Key];
                _currentBookmark.transform.SetParent(_activePageMarker);
                break;
            }
        }

        public void OnClickBookmark(NotebookBookmark notebookBookmark, int page)
        {
            _currentBookmark = notebookBookmark;
            _currentBookmark.transform.SetParent(_activePageMarker);
            if (!_noteBookOpen)
            {
                _currentPageNumber = page;
                OpenNotebook();
                return;
            }
            MoveToPage(page);
        }

        private void RemoveActiveBookmark()
        {
            if (_currentBookmark == null)
            {
                return;
            }

            _currentBookmark.transform.SetParent(_pageMarkers);
            _currentBookmark = null;
        }

        private void MoveToPage(int page)
        {
            CheckBookmark(page);
            FlipPage(page);
        }

        private void FlipPage(int nextPage)
        {
            (Func<GameObject>, Func<GameObject>) instantiatePages;
            GameObject currentPage;

            _midPointFlip();
            _endFlip();
            
            _flipPage.UnattachPageContent();
            
            if (_currentPageNumber < nextPage)
            {
                if (_currentPageNumber == 0)
                {
                    EventsManager.OnCloseMapPages();
                }
                MoveBookmarks(_shouldBeOnRightSide, nextPage);
                currentPage = _rightPage.GetCurrentPage();
                _flipPage.ChangeContent(currentPage);
                _currentPageNumber = nextPage;
                instantiatePages = CheckContentToShow();
                _rightPage.UnattachPageContent();
                
                if (instantiatePages.Item2 != null)
                {
                    _rightPage.ChangeContent(instantiatePages.Item2());    
                }
                
                if (currentPage != null)
                {
                    currentPage.transform.localRotation = new Quaternion(0, 0, 0, 1);
                }

                _midPointFlip = () =>
                {
                    GameObject page = instantiatePages.Item1();
                    _flipPage.ChangeContent(page);
                    page.transform.localRotation = new Quaternion(0, 180, 0, 1);
                };

                _endFlip = () =>
                {
                    GameObject page = _flipPage.GetCurrentPage();
                    _leftPage.ChangeContent(page);
                    page.transform.localRotation = new Quaternion(0, 0, 0, 1);
                };

                _notebook.FlipPageLeft(_midPointFlip, _endFlip);
                return;
            }
            MoveBookmarks(_shouldBeOnLeftSide, nextPage);
            currentPage = _leftPage.GetCurrentPage();
            _flipPage.ChangeContent(currentPage);
            currentPage.transform.localRotation = new Quaternion(0, 180, 0, 1);
            _currentPageNumber = nextPage;
            instantiatePages = CheckContentToShow();
            _leftPage.UnattachPageContent();
            _leftPage.ChangeContent(instantiatePages.Item1());

            _midPointFlip = () =>
            {
                if (instantiatePages.Item2 == null)
                {
                    _flipPage.GetCurrentPage().SetActive(false);
                    _flipPage.UnattachPageContent();
                    return;
                }
                
                GameObject page = instantiatePages.Item2();
                _flipPage.ChangeContent(page);
                page.transform.localRotation = new Quaternion(0, 0, 0, 1);
            };

            _endFlip = () =>
            {
                GameObject page = _flipPage.GetCurrentPage();
                if (page == null)
                {
                    _rightPage.GetCurrentPage().SetActive(false);
                    _flipPage.transform.parent.localRotation = new Quaternion(0, 180, 0, 1);
                    return;
                }
                _rightPage.ChangeContent(page);
                _flipPage.transform.parent.localRotation = new Quaternion(0, 180, 0, 1);

                if (nextPage == 0)
                {
                    EventsManager.OnOpenMapPages();
                }
            };

            _notebook.FlipPageRight(_midPointFlip, _endFlip);
        }

        public void NotifyMidPointFlip()
        {
            _midPointFlip = () => { };
        }

        public void NotifyEndFlip()
        {
            _endFlip = () => { };
        }

        private void MoveBookmarks(Func<int, int, bool> sideChecker, int nextPage)
        {
            List<NotebookBookmark> notebookBookmarks = new List<NotebookBookmark>();

            for (int i = 0; i < _bookmarks.Count; i++)
            {
                if (sideChecker(i, nextPage))
                {
                    notebookBookmarks.Add(_bookmarks[i]);
                }
            }

            foreach (NotebookBookmark notebookBookmark in notebookBookmarks)
            {
                ///// HERE
                Transform bookmarkTransform = notebookBookmark.transform;
                Vector3 position = bookmarkTransform.localPosition;
                bookmarkTransform.localPosition = new Vector3(-position.x - BOOKMARK_WIDTH, position.y, position.z);
                /////
                notebookBookmark.SetIsOnRightSide(!notebookBookmark.IsOnRightSide());
            }
        }

        private (Func<GameObject>, Func<GameObject>) CheckContentToShow()
        {
            int mapIndex = _notebookIndices[NotebookContentType.MAP];
            int peopleIndex = _notebookIndices[NotebookContentType.PERSON];

            Func<GameObject> leftPage;
            Func<GameObject> rightPage;
            
            if (_currentPageNumber == mapIndex)
            {
                leftPage = PassContentToShow(NotebookContentType.MAP, MAP_RANGE_OF_PAGES, 0, true);
                rightPage = PassContentToShow(NotebookContentType.MAP, MAP_RANGE_OF_PAGES, 0, false);
                return (leftPage, rightPage);
            }
            
            if (_currentPageNumber < peopleIndex)
            {
                leftPage = PassContentToShow(NotebookContentType.COUNTRY, COUNTRY_RANGE_OF_PAGES, 
                    mapIndex + MAP_RANGE_OF_PAGES, true);
                
                rightPage = PassContentToShow(NotebookContentType.COUNTRY, COUNTRY_RANGE_OF_PAGES, 
                    mapIndex + MAP_RANGE_OF_PAGES, false);
                
                return (leftPage, rightPage);
            }
            
            leftPage = PassContentToShow(NotebookContentType.PERSON, PERSON_RANGE_OF_PAGES, peopleIndex, true);
            
            rightPage = PassContentToShow(NotebookContentType.PERSON, PERSON_RANGE_OF_PAGES, peopleIndex, false);

            return (leftPage, rightPage);
        }

        private Func<GameObject> PassContentToShow(NotebookContentType notebookContentType, int rangeOfPages, 
            int index, bool isLeftPage)
        {
            int pageToFill = _currentPageNumber;
            
            if (!isLeftPage)
            {
                pageToFill++;
            }
            
            BaseNotebookPage pageContent = _notebookPages[pageToFill];

            if (pageContent == null)
            {
                return null;
            }

            return () =>
            {
                NotebookContentPage page = _notebookContentPages[notebookContentType][(pageToFill - index) % rangeOfPages];

                if (!_noteBookOpen && isLeftPage)
                {
                    page.transform.localRotation = new Quaternion(0, 180, 0, 1);
                }
            
                page.FillPage(pageContent);

                return page.gameObject;
            };
        }

        private void SetIsNotebookOpen(bool notebookOpen)
        {
            _noteBookOpen = notebookOpen;
            
            OnOpenOrCloseNotebook();
        }

        private void OnOpenOrCloseNotebook()
        {
            CheckBookmark(_currentPageNumber);
            
            foreach (NotebookBookmark bookmark in _bookmarks)
            {
                if (bookmark.IsOnRightSide())
                {
                    break;
                }

                ///// HERE
                Transform bookmarkTransform = bookmark.transform;
                Vector3 position = bookmarkTransform.localPosition;
                bookmarkTransform.localPosition = new Vector3(-position.x - BOOKMARK_WIDTH, position.y, position.z);
                /////
            }
        }

        private Action OpenTransition()
        {
            (Func<GameObject>, Func<GameObject>) pagesToInstantiate = CheckContentToShow();
            
            if (pagesToInstantiate.Item2 != null)
            {
                _rightPage.ChangeContent(pagesToInstantiate.Item2());    
            }

            return () =>
            {
                GameObject page = pagesToInstantiate.Item1();
                _leftPage.ChangeContent(page);
                page.transform.localRotation = new Quaternion(0, 0, 0, 1);
            };;
        }

        public void OpenNotebook(bool move = true)
        {
            SetIsNotebookOpen(true);
            
            Action open = OpenTransition();
            
            _notebook.Open(open, move);
        }

        private Action CloseTransition()
        {
            if (_currentPageNumber == 0)
            {
                EventsManager.OnCloseMapPages();
            }
            
            return () =>
            {
                GameObject page = _cover;
                _leftPage.ChangeContent(page);
                page.transform.localRotation = new Quaternion(0, 180, 0, 1);
            };
        }

        private void CloseNotebook()
        {
            SetIsNotebookOpen(false);
            
            Action close = CloseTransition();
                
            _notebook.Close(close);
        }

        public void EndDragNotebook(float positionY, float dragVectorY, float closeThreshold)
        {
            if (_noteBookOpen && (positionY <= closeThreshold || positionY <= closeThreshold * 0.5f && dragVectorY < -0.04f))
            {
                CloseNotebook();
            }
            else if (!_noteBookOpen)
            {
                if (positionY > closeThreshold)
                {
                    OpenNotebook(false);
                    return;
                }
                _notebook.StartMoveDownCoroutine();
            }
        }
    }
}