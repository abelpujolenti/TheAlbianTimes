using System;
using System.Collections.Generic;

namespace Workspace.Notebook
{
    public enum NotebookContentType
    {
        MAP,
        INTERNATIONAL,
        PERSON
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
        public string flagImagePath;
        public string description;
        public string history;
        public Dictionary <string, List<string>> importantPeople;
        public Dictionary <string, List<string>> organizations;
        public List<string> ongoingConflicts;
        public Dictionary <int, int> reputationHistory;
    }

    public class CountryContentPage0 : BaseNotebookPage
    {
        public string countryName;
        public string flagImagePath;
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
