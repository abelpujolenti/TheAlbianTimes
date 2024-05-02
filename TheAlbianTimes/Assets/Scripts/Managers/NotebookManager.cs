using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Workspace.Notebook;
using Workspace.Notebook.Pages;
using Workspace.Notebook.Pages.Map;

namespace Managers
{
    public class NotebookManager : MonoBehaviour
    {
        private static NotebookManager _instance;
        public static NotebookManager Instance => _instance;
        
        private const string PATH_COUNTRIES_CONTENT_CONTAINER = "/Json/Notebook/CountriesContainer.json";
        private const string PATH_INTERNATIONALS_CONTENT_CONTAINER = "/Json/Notebook/InternationalsContainer.json";
        private const string PATH_PEOPLE_CONTENT_CONTAINER = "/Json/Notebook/PeopleContainer.json";

        private const float BOOKMARK_WIDTH = 22;

        private const int MAP_RANGE_OF_PAGES = 2;
        private const int COUNTRY_RANGE_OF_PAGES = 6;
        private const int INTERNATIONAL_RANGE_OF_PAGES = 1;
        private const int PERSON_RANGE_OF_PAGES = 1;

        [SerializeField] private GameObject _coverPrefab;
        [SerializeField] private GameObject[] _mapPagesPrefabs;
        [SerializeField] private GameObject[] _countryPagesPrefabs;
        [SerializeField] private GameObject[] _internationalPagesPrefabs;
        [SerializeField] private GameObject[] _personPagesPrefabs;

        [SerializeField] private Transform _pageMarkers;
        [SerializeField] private Transform _activePageMarker;
        
        [SerializeField] private NotebookBookmark[] _bookmarks;

        [SerializeField] private NotebookPage _leftPage;
        [SerializeField] private NotebookPage _flipPage;
        [SerializeField] private NotebookPage _rightPage;

        private Notebook _notebook;

        private NotebookBookmark _currentBookmark;

        private Dictionary<int, BaseNotebookPage> _notebookPages = new Dictionary<int, BaseNotebookPage>();
        private Dictionary<string, int> _countryIndices = new Dictionary<string, int>();
        private Dictionary<NotebookContentType, int> _notebookIndices = new Dictionary<NotebookContentType, int>();

        private int _currentPageNumber;
        private int _totalPages;

        private bool _noteBookOpen;

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
                _leftPage.ChangeContent(Instantiate(_coverPrefab));
                
                _currentBookmark = _bookmarks[0];
                _currentBookmark.transform.SetParent(_activePageMarker);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void SetNotebook(Notebook notebook)
        {
            _notebook = notebook;
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
            InternationalContent[] internationalsContent =
                LoadNotebookContentsFromJson<InternationalsContainer, InternationalContent>(PATH_INTERNATIONALS_CONTENT_CONTAINER);
            PersonContent[] peopleContent =
                LoadNotebookContentsFromJson<PeopleContainer, PersonContent>(PATH_PEOPLE_CONTENT_CONTAINER);
            
            ReservePagesForMap();
            SaveCountriesContentsInDictionary(countriesContent);
            SaveInternationalsContentsInDictionary(internationalsContent);
            SavePeopleContentsInDictionary(peopleContent);

            _totalPages = _notebookPages.Count;
        }

        private void ReservePagesForMap()
        {
            _notebookIndices.Add(NotebookContentType.MAP, _notebookPages.Count);
            _notebookPages.Add(_notebookPages.Count, null);
            _notebookPages.Add(_notebookPages.Count, null);
        }

        private void SaveCountriesContentsInDictionary(CountryContent[] contents)
        {
            foreach (CountryContent content in contents)
            {
                CountryContentPage0 page0 = new CountryContentPage0
                {
                    countryName = content.countryName,
                    flagImagePath = content.flagImagePath,
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

        private void SaveInternationalsContentsInDictionary(InternationalContent[] contents)
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
        }

        private void SavePeopleContentsInDictionary(PersonContent[] contents)
        {
            _notebookIndices.Add(NotebookContentType.PERSON, _notebookPages.Count);
            
            foreach (PersonContent content in contents)
            {
                PersonContentPage0 page0 = new PersonContentPage0
                {
                    name = content.name,
                    descriptions =  content.descriptions,
                    photoImagePath = content.photoImagePath
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

        public int AssignPageToBookmark(NotebookContentType notebookContentType)
        {
            return _notebookIndices[notebookContentType];
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

            Action midPointFlip;
            Action endFlip;
            
            if (_currentPageNumber < nextPage)
            {
                MoveBookmarks(_shouldBeOnRightSide, nextPage);
                currentPage = _rightPage.GetCurrentPage();
                _flipPage.ChangeContent(currentPage);
                _currentPageNumber = nextPage;
                instantiatePages = CheckContentToShow();
                
                if (instantiatePages.Item2 != null)
                {
                    _rightPage.ChangeContent(instantiatePages.Item2());    
                }
                
                if (currentPage != null)
                {
                    currentPage.transform.localRotation = new Quaternion(0, 0, 0, 1);
                }

                midPointFlip = () =>
                {
                    if (currentPage != null)
                    {
                        Destroy(currentPage);    
                    }
                    GameObject page = instantiatePages.Item1();
                    _flipPage.ChangeContent(page);
                    page.transform.localRotation = new Quaternion(0, 270, 0, 1);
                };

                endFlip = () =>
                {
                    Destroy(_leftPage.GetCurrentPage());
                    _leftPage.ChangeContent(_flipPage.GetCurrentPage());
                };

                _notebook.FlipPageLeft(midPointFlip, endFlip);
                return;
            }
            
            MoveBookmarks(_shouldBeOnLeftSide, nextPage);
            currentPage = _leftPage.GetCurrentPage();
            currentPage.transform.localRotation = new Quaternion(0, 0, 0, 1);
            _flipPage.ChangeContent(currentPage);
            _currentPageNumber = nextPage;
            instantiatePages = CheckContentToShow();
            _leftPage.ChangeContent(instantiatePages.Item1());

            midPointFlip = () =>
            {
                Destroy(currentPage);
                if (instantiatePages.Item2 == null)
                {
                    return;
                }
                
                GameObject page = instantiatePages.Item2();
                _flipPage.ChangeContent(page);
                page.transform.localRotation = new Quaternion(0, 0, 0, 1);
            };

            endFlip = () =>
            {
                Destroy(_rightPage.GetCurrentPage());
                _rightPage.ChangeContent(_flipPage.GetCurrentPage());
            };

            _notebook.FlipPageRight(midPointFlip, endFlip);
        }

        private void MoveBookmarks(Func<int, int, bool> sideChecker, int nextPage)
        {
            List<NotebookBookmark> notebookBookmarks = new List<NotebookBookmark>();

            for (int i = 0; i < _bookmarks.Length; i++)
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
            int internationalsIndex = _notebookIndices[NotebookContentType.INTERNATIONAL];
            int peopleIndex = _notebookIndices[NotebookContentType.PERSON];

            Func<GameObject> leftPage;
            Func<GameObject> rightPage;
            
            if (_currentPageNumber == mapIndex)
            {
                leftPage = PassContentToShow(_mapPagesPrefabs, MAP_RANGE_OF_PAGES, 0, _leftPage, true);
                rightPage = PassContentToShow(_mapPagesPrefabs, MAP_RANGE_OF_PAGES, 0, _rightPage, false);
                return (leftPage, rightPage);
            }
            
            if (_currentPageNumber < internationalsIndex)
            {
                leftPage = PassContentToShow(_countryPagesPrefabs, COUNTRY_RANGE_OF_PAGES,
                    _notebookIndices[NotebookContentType.MAP] + MAP_RANGE_OF_PAGES, _leftPage, true);
                
                rightPage = PassContentToShow(_countryPagesPrefabs, COUNTRY_RANGE_OF_PAGES,
                    _notebookIndices[NotebookContentType.MAP] + MAP_RANGE_OF_PAGES, _rightPage, false);
                
                return (leftPage, rightPage);
            }
            
            if (_currentPageNumber < peopleIndex)
            {
                leftPage = PassContentToShow(_internationalPagesPrefabs, INTERNATIONAL_RANGE_OF_PAGES,
                    internationalsIndex, _leftPage, true);
                
                rightPage = PassContentToShow(_internationalPagesPrefabs, INTERNATIONAL_RANGE_OF_PAGES,
                    internationalsIndex, _rightPage, false);
                
                return (leftPage, rightPage);
            }
            
            leftPage = PassContentToShow(_personPagesPrefabs, PERSON_RANGE_OF_PAGES,
                peopleIndex, _leftPage, true);
            
            rightPage = PassContentToShow(_personPagesPrefabs, PERSON_RANGE_OF_PAGES,
                peopleIndex, _rightPage, false);

            return (leftPage, rightPage);
        }

        private Func<GameObject> PassContentToShow(GameObject[] pagePrefabs, int rangeOfPages, 
            int index, NotebookPage notebookPage, bool isLeftPage)
        {
            int pageToFill = _currentPageNumber;
            
            if (!isLeftPage)
            {
                pageToFill++;
            }
            
            BaseNotebookPage pageContent = _notebookPages[pageToFill];

            if (pageContent == null)
            {
                notebookPage.ChangeContent(null);
                return null;
            }

            return () =>
            {
                GameObject page = Instantiate(pagePrefabs[(pageToFill - index) % rangeOfPages]);

                if (!_noteBookOpen && isLeftPage)
                {
                    page.transform.localRotation = new Quaternion(0, 180, 0, 1);
                }
            
                page.GetComponent<NotebookContentPage>().FillPage(pageContent);

                return page;
            };
        }

        private void SetIsNotebookOpen(bool notebookOpen)
        {
            _noteBookOpen = notebookOpen;
            
            OnOpenOrCloseNotebook();
        }

        private void OnOpenOrCloseNotebook()
        {
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
                GameObject page = _rightPage.GetCurrentPage();
                if (page != null)
                {
                    Destroy(page);
                }
                _rightPage.ChangeContent(pagesToInstantiate.Item2());    
            }

            return () =>
            {
                Destroy(_leftPage.GetCurrentPage());
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
            return () =>
            {
                Destroy(_leftPage.GetCurrentPage());
                GameObject page = Instantiate(_coverPrefab);
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
                }
                else
                {
                    _notebook.StartMoveDownCoroutine();
                }
            }
        }
    }
}