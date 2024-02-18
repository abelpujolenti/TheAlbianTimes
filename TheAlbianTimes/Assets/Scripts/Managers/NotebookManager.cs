using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Workspace.Notebook;

namespace Managers
{
    public class NotebookManager : MonoBehaviour
    {
        private static NotebookManager _instance;
        public static NotebookManager Instance => _instance;
        
        private const string PATH_COUNTRIES_CONTENT_CONTAINER = "/Json/Notebook/CountriesContainer.json";
        private const string PATH_INTERNATIONALS_CONTENT_CONTAINER = "/Json/Notebook/InternationalsContainer.json";
        private const string PATH_PEOPLE_CONTENT_CONTAINER = "/Json/Notebook/PeopleContainer.json";

        private const int COUNTRY_RANGE_OF_PAGES = 4;
        private const int INTERNATIONAL_RANGE_OF_PAGES = 1;
        private const int PERSON_RANGE_OF_PAGES = 1;
        private const int MAP_RANGE_OF_PAGES = 2;

        [SerializeField] private GameObject[] _countryPagesPrefabs;
        [SerializeField] private GameObject[] _internationalPagesPrefabs;
        [SerializeField] private GameObject[] _personPagesPrefabs;

        [SerializeField] private NotebookPage _leftPage;
        [SerializeField] private NotebookPage _rightPage;

        private Notebook _notebook;

        private Dictionary<int, BaseNotebookPage> _notebookPages = new Dictionary<int, BaseNotebookPage>();
        private Dictionary<NotebookContentType, int> _notebookIndices = new Dictionary<NotebookContentType, int>();

        private CountryContent[] _countriesContent;
        
        private InternationalContent[] _internationalsContent;
        
        private PersonContent[] _peopleContent;

        private int _currentPage;
        private int _totalPages;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                LoadNotebookContents();
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

        private void LoadNotebookContents()
        {
            _countriesContent =
                LoadNotebookContentsFromJson<CountriesContainer, CountryContent>(PATH_COUNTRIES_CONTENT_CONTAINER);
            _internationalsContent =
                LoadNotebookContentsFromJson<InternationalsContainer, InternationalContent>(PATH_INTERNATIONALS_CONTENT_CONTAINER);
            _peopleContent =
                LoadNotebookContentsFromJson<PeopleContainer, PersonContent>(PATH_PEOPLE_CONTENT_CONTAINER);
            
            SaveCountriesContentsInDictionary(_countriesContent);
            SaveInternationalsContentsInDictionary(_internationalsContent);
            SavePeopleContentsInDictionary(_peopleContent);
        }

        private void SaveCountriesContentsInDictionary(CountryContent[] contents)
        {
            _notebookIndices.Add(NotebookContentType.COUNTRY, _notebookPages.Count);
            
            foreach (CountryContent content in contents)
            {
                CountryContentPage0 page0 = new CountryContentPage0
                {
                    name = content.name,
                    description = content.description,
                    flagImagePath = content.flagImagePath,
                    history = content.history
                };
                
                CountryContentPage1 page1 = new CountryContentPage1
                {
                    importantPeople = content.importantPeople,
                    organizations = content.organizations
                };

                CountryContentPage2 page2 = new CountryContentPage2
                {
                    ongoingConflicts = content.ongoingConflicts
                };

                CountryContentPage3 page3 = new CountryContentPage3
                {
                    reputationHistory = content.reputationHistory
                };
                
                _notebookPages.Add(_notebookPages.Count, page0);
                _notebookPages.Add(_notebookPages.Count, page1);
                _notebookPages.Add(_notebookPages.Count, page2);
                _notebookPages.Add(_notebookPages.Count, page3);
            }
            
            FillLastPageIfUneven();
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
            
            _notebookIndices.Add(NotebookContentType.MAP, _notebookPages.Count);
        }

        private void FillLastPageIfUneven()
        {
            if (_notebookPages.Count % 2 == 0)
            {
                return;
            }
            
            _notebookPages.Add(_notebookPages.Count, null);
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

            return JsonUtility.FromJson<TContainer>(json);
        }

        private string LoadFromJson(string path)
        {
            return !File.Exists(Application.streamingAssetsPath + path) ? null : FileManager.LoadJsonFile(path);
        }

        public int AssignPageToBookmark(NotebookContentType notebookContentType)
        {
            return _notebookIndices[notebookContentType];
        }

        public void NextPage()
        {
            _currentPage += 2;
            _notebook.FlipPage();
            if (_currentPage >= _notebookIndices[NotebookContentType.MAP])
            {
             
                return;
            }
            ChangeShownContentNotebook();
        }

        public void PreviousPage()
        {
            _currentPage -= 2;
            _notebook.FlipPage();
            if (_currentPage >= _notebookIndices[NotebookContentType.MAP])
            {
             
                return;
            }
            ChangeShownContentNotebook();
        }

        private void ChangeShownContentNotebook()
        {
            CheckContentToShow(_currentPage);
            CheckContentToShow(_currentPage + 1);
        }

        private void CheckContentToShow(int pageToFill)
        {
            int internationalsIndex = _notebookIndices[NotebookContentType.INTERNATIONAL];
            int peopleIndex = _notebookIndices[NotebookContentType.PERSON];
            
            if (pageToFill < internationalsIndex)
            {
                PassContentToShow(_countryPagesPrefabs, COUNTRY_RANGE_OF_PAGES,
                    _notebookIndices[NotebookContentType.COUNTRY], pageToFill, NotebookPageType.COUNTRY_PAGE_0);
                return;
            }
            
            if (pageToFill < peopleIndex)
            {
                PassContentToShow(_internationalPagesPrefabs, INTERNATIONAL_RANGE_OF_PAGES,
                    _notebookIndices[NotebookContentType.INTERNATIONAL], pageToFill, NotebookPageType.INTERNATIONAL_PAGE_0);
            }
            
            PassContentToShow(_personPagesPrefabs, PERSON_RANGE_OF_PAGES,
                _notebookIndices[NotebookContentType.PERSON], pageToFill, NotebookPageType.PERSON_PAGE_0);
        }

        private void PassContentToShow(GameObject[] pagePrefabs, int rangeOfPages, int index, int pageToFill,
            NotebookPageType firstIndex)
        {
            BaseNotebookPage page = _notebookPages[pageToFill];

            if (page == null)
            {
                return;
            }

            int pageType = (index - pageToFill) % rangeOfPages;
            
            GameObject pagePrefab = pagePrefabs[pageType];

            if (pageToFill == _currentPage)
            {
                _leftPage.ChangeContent(pagePrefab, page, firstIndex + pageType);
                return;
            }
            
            _rightPage.ChangeContent(pagePrefab, page, firstIndex + pageType);
        }
    }
}