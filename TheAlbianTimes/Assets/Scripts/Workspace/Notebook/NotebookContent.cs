using System;
using System.Collections.Generic;

namespace Workspace.Notebook
{
    public enum NotebookContentType
    {
        MAP,
        PERSON,
        COUNTRY
    }

    public abstract class BaseNotebookContainer
    {
        protected BaseNotebookContainer() {}
        public abstract BaseNotebookContent[] GetContent();
    }

    [Serializable]
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
    
    [Serializable]
    public class CountryContent : BaseNotebookContent
    {
        public string countryName;
        public string description;
        public string history;
        public Dictionary <string, List<string>> importantPeople;
        public Dictionary <string, List<string>> organizations;
        public List<string> ongoingConflicts;
    }

    public class MapPage0 : BaseNotebookPage
    {
        public Action hetiaClick;
        public Action albiaClick;
        public Action terkanClick;
        public Action dalmeClick;
        public Action madiaClick;
    }

    public class MapPage1 : BaseNotebookPage
    {
        public Action xayaClick;
        public Action suokaClick;
        public Action zuaniaClick;
        public Action rekkaClick;
    }

    public class CountryContentPage0 : BaseNotebookPage
    {
        public string countryName;
        public string description;
    }

    public class CountryContentPage1 : BaseNotebookPage
    {
        public string countryName;
        public string history;
    }

    public class CountryContentPage2 : BaseNotebookPage
    {
        public string countryName;
        public Dictionary <string, List<string>> importantPeople;
    }

    public class CountryContentPage3 : BaseNotebookPage
    {
        public string countryName;
        public Dictionary <string, List<string>> organizations;
    }

    public class CountryContentPage4 : BaseNotebookPage
    {
        public string countryName;
        public List<string> ongoingConflicts;
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
        public List <string> descriptions;
    }

    public class PersonContentPage0 : BaseNotebookPage
    {
        public string name;
        public List <string> descriptions;
    }
}
