using System;
using System.Collections.Generic;

namespace Workspace.Notebook
{
    public enum NotebookContentType
    {
        COUNTRY,
        INTERNATIONAL,
        PERSON,
        MAP
    }

    public enum NotebookPageType
    {
        COUNTRY_PAGE_0,
        COUNTRY_PAGE_1,
        COUNTRY_PAGE_2,
        COUNTRY_PAGE_3,
        INTERNATIONAL_PAGE_0,
        PERSON_PAGE_0,
    }

    public abstract class BaseNotebookContainer
    {
        protected BaseNotebookContainer() {}
        public abstract BaseNotebookContent[] GetContent();
    }

    public class CountriesContainer : BaseNotebookContainer
    {
        public CountryContent[] countriesContent = Array.Empty<CountryContent>();
        public override BaseNotebookContent[] GetContent()
        {
            return countriesContent;
        }
    }

    public class InternationalsContainer : BaseNotebookContainer
    {
        public InternationalContent[] internationalsContent = Array.Empty<InternationalContent>();
        public override BaseNotebookContent[] GetContent()
        {
            return internationalsContent;
        }
    }

    public class PeopleContainer : BaseNotebookContainer
    {
        public PersonContent[] peopleContent = Array.Empty<PersonContent>();
        public override BaseNotebookContent[] GetContent()
        {
            return peopleContent;
        }
    }

    public abstract class BaseNotebookContent{

        protected BaseNotebookContent() {}
    }

    public abstract class BaseNotebookPage
    {
        protected BaseNotebookPage() {}
    }
    
    public class CountryContent : BaseNotebookContent
    {
        public string name;
        public string flagImagePath;
        public string description;
        public string history;
        public Dictionary <string, List<string>> importantPeople;
        public Dictionary <string, List<string>> organizations;
        public string ongoingConflicts;
        public Dictionary <int, int> reputationHistory;
    }

    public class CountryContentPage0 : BaseNotebookPage
    {
        public string name;
        public string flagImagePath;
        public string description;
        public string history;
    }

    public class CountryContentPage1 : BaseNotebookPage
    {
        public Dictionary <string, List<string>> importantPeople;
        public Dictionary <string, List<string>> organizations;
    }

    public class CountryContentPage2 : BaseNotebookPage
    {
        public string ongoingConflicts;
    }

    public class CountryContentPage3 : BaseNotebookPage
    {
        public Dictionary <int, int> reputationHistory;
    }
    
    public class InternationalContent : BaseNotebookContent
    {
        public string name;
    }

    public class InternationalContentPage0 : BaseNotebookPage
    {
        public string name;
    }

    public class PersonContent : BaseNotebookContent
    {
        public string name;
        public string photoImagePath;
        public List <string> descriptions;
    }

    public class PersonContentPage0 : BaseNotebookPage
    {
        public string name;
        public string photoImagePath;
        public List <string> descriptions;
    }
}
