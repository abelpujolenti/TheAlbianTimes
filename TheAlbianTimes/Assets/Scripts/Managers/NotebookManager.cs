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

        [SerializeField]private int _currentPage;
        [SerializeField]private int _totalPages;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this; 
                //TEST();
                
                LoadNotebookContents();
                ChangeShownContentNotebook();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void TEST()
        {
            CountryContent countryContent = new CountryContent
            {
                countryName = "A",
                flagImagePath = "A",
                description = "A",
                history = "A",
                importantPeople = new Dictionary<string, List<string>>
                {
                    { "Abel", new List<string>
                        {
                            "Putero", "Programador"
                        } 
                    }  
                },
                organizations = new Dictionary<string, List<string>>
                {
                    { "MiCasa" , new List<string>
                        {
                            "Tiene ratas con camuflaje", "Jeje son tacticas"
                        }
                    }  
                },
                ongoingConflicts = new List<string>
                {
                    "La rata toca los huevos", "Los niños quieren amotinarse"
                }
            };
            
            CountriesContainer container = new CountriesContainer
            {
                countriesContent = new [] {countryContent}
            };

            string json = JsonConvert.SerializeObject(container);
            string path = Application.streamingAssetsPath + PATH_COUNTRIES_CONTENT_CONTAINER;
            
            File.WriteAllText(path, json);
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
            
            ReservePagesForMap();
            SaveCountriesContentsInDictionary(_countriesContent);
            SaveInternationalsContentsInDictionary(_internationalsContent);
            SavePeopleContentsInDictionary(_peopleContent);

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
            _notebookIndices.Add(NotebookContentType.COUNTRY, _notebookPages.Count);
            
            foreach (CountryContent content in contents)
            {
                CountryContentPage0 page0 = new CountryContentPage0
                {
                    countryName = content.countryName,
                    description = content.description,
                    flagImagePath = content.flagImagePath,
                    history = content.history
                };
                
                CountryContentPage1 page1 = new CountryContentPage1
                {
                    countryName = content.countryName,
                    importantPeople = content.importantPeople,
                    organizations = content.organizations
                };

                CountryContentPage2 page2 = new CountryContentPage2
                {
                    countryName = content.countryName,
                    ongoingConflicts = content.ongoingConflicts
                };

                CountryContentPage3 page3 = new CountryContentPage3
                {
                    countryName = content.countryName,
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

            return JsonConvert.DeserializeObject<TContainer>(json);
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
            int auxCurrentPage = _currentPage;
            auxCurrentPage += 2;
            
            if (auxCurrentPage >= _totalPages)
            {
                return;
            }
            
            MoveToPage(auxCurrentPage);
        }

        public void PreviousPage()
        {
            int auxCurrentPage = _currentPage;
            auxCurrentPage -= 2;
            
            if (auxCurrentPage < 0)
            {
                return;
            }
            
            MoveToPage(auxCurrentPage);
        }

        public void MoveToPage(int page)
        {
            FlipPage(page);
            _currentPage = page;
            ChangeShownContentNotebook();
        }

        private void FlipPage(int nextPage)
        {
            if (_currentPage < nextPage)
            {
                //Right
                _notebook.FlipPage();
            }
            else if (_currentPage > nextPage)
            {
                //Left
                _notebook.FlipPage();
            }
        }

        private void ChangeShownContentNotebook()
        {
            CheckContentToShow(_currentPage);
        }

        private void CheckContentToShow(int pageToFill)
        {
            int internationalsIndex = _notebookIndices[NotebookContentType.INTERNATIONAL];
            int peopleIndex = _notebookIndices[NotebookContentType.PERSON];
            
            if (pageToFill < internationalsIndex)
            {
                PassContentToShow(_countryPagesPrefabs, COUNTRY_RANGE_OF_PAGES,
                    _notebookIndices[NotebookContentType.COUNTRY], pageToFill, _leftPage);
                PassContentToShow(_countryPagesPrefabs, COUNTRY_RANGE_OF_PAGES,
                    _notebookIndices[NotebookContentType.COUNTRY], pageToFill + 1, _rightPage);
                return;
            }
            
            if (pageToFill < peopleIndex)
            {
                PassContentToShow(_internationalPagesPrefabs, INTERNATIONAL_RANGE_OF_PAGES,
                    internationalsIndex, pageToFill, _leftPage);
                PassContentToShow(_internationalPagesPrefabs, INTERNATIONAL_RANGE_OF_PAGES,
                    internationalsIndex, pageToFill + 1, _rightPage);
                return;
            }
            
            PassContentToShow(_personPagesPrefabs, PERSON_RANGE_OF_PAGES,
                peopleIndex, pageToFill, _leftPage);
            PassContentToShow(_personPagesPrefabs, PERSON_RANGE_OF_PAGES,
                peopleIndex, pageToFill + 1, _rightPage);
        }

        private void PassContentToShow(GameObject[] pagePrefabs, int rangeOfPages, 
            int index, int pageToFill, NotebookPage notebookPage)
        {
            BaseNotebookPage pageContent = _notebookPages[pageToFill];

            if (pageContent == null)
            {
                notebookPage.ChangeContent(null);
                return;
            }
            
            GameObject page = Instantiate(pagePrefabs[(pageToFill - index) % rangeOfPages]);
            
            page.GetComponent<NotebookContentPage>().FillPage(pageContent);

            notebookPage.ChangeContent(page);
        }
    }
}