
namespace Workspace.Notebook.Content
{
    public abstract class NotebookContent{

        protected NotebookContent() {}
    }

    public class CountryContent : NotebookContent
    {
        public string countryName;
        public string countryDescription;
        public string countryRecord;
    }

    public class EditorialContent : NotebookContent
    {
        
    }

    public class MapContent : NotebookContent
    {
        
    }
}
