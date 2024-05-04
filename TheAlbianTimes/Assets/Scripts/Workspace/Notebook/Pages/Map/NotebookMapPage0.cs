using System;
using UnityEngine;

namespace Workspace.Notebook.Pages.Map
{
    public class NotebookMapPage0 : NotebookMapPage
    {
        private Action _hetiaClick;
        private Action _terkanClick;
        private Action _albiaClick;
        private Action _dalmeClick;
        private Action _madiaClick;

        public override void FillPage(BaseNotebookPage baseNotebookPage)
        {
            MapPage0 page;
            
            try
            {
                page = (MapPage0)baseNotebookPage;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            _hetiaClick = page.hetiaClick;
            _terkanClick = page.terkanClick;
            _albiaClick = page.albiaClick;
            _dalmeClick = page.dalmeClick;
            _madiaClick = page.madiaClick;
        }

        public void ClickHetia() => _hetiaClick();
        public void ClickTerkan() => _terkanClick();
        public void ClickAlbia() => _albiaClick();
        public void ClickDalme() => _dalmeClick();
        public void ClickMadia() => _madiaClick();
    }
}
