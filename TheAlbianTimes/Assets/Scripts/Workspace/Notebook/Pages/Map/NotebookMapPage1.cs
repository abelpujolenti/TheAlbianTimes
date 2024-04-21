using System;
using UnityEngine;

namespace Workspace.Notebook.Pages.Map
{
    public class NotebookMapPage1 : NotebookContentPage
    {
        private Action _xayaClick;
        private Action _suokaClick;
        private Action _zuaniaClick;
        private Action _rekkaClick;
        public override void FillPage(BaseNotebookPage baseNotebookPage)
        {
            MapPage1 page;
            
            try
            {
                page = (MapPage1)baseNotebookPage;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            _xayaClick = page.xayaClick;
            _suokaClick = page.suokaClick;
            _zuaniaClick = page.zuaniaClick;
            _rekkaClick = page.rekkaClick;
        }

        public void ClickXaya() => _xayaClick();
        public void ClickSuoka() => _suokaClick();
        public void ClickZuania() => _zuaniaClick();
        public void ClickRekka() => _rekkaClick();
    }
}