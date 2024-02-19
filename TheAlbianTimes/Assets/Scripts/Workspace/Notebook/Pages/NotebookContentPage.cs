using UnityEngine;

namespace Workspace.Notebook.Pages
{
    public abstract class NotebookContentPage : MonoBehaviour
    {
        public abstract void FillPage(BaseNotebookPage baseNotebookPage);
    }
}
