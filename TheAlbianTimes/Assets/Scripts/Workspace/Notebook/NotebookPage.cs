using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Workspace.Notebook
{
    public class NotebookPage : InteractableRectTransform
    {
        [SerializeField] private bool _righPage;

        private NotebookManager _notebookManager;

        private void Start()
        {
            _notebookManager = NotebookManager.Instance;
        }

        protected override void PointerClick(BaseEventData data)
        {
            if (_righPage)
            {
                _notebookManager.NextPage();
                
                return;
            }
            _notebookManager.PreviousPage();
        }

        public void ChangeContent(GameObject pagePrefab, BaseNotebookPage baseNotebookContent,
            NotebookPageType notebookPageType)
        {
            GameObject pageGameObject = Instantiate(pagePrefab, transform);
        }

        private void FillCountryPage0(CountryContentPage0 countryContentPage0)
        {
            
        }
        
        private void FillCountryPage1(CountryContentPage1 countryContentPage1)
        {
            
        }
        
        private void FillCountryPage2(CountryContentPage2 countryContentPage1)
        {
            
        }
        
        private void FillCountryPage3(CountryContentPage3 countryContentPage1)
        {
            
        }
        
        private void FillInternationalPage0(InternationalContentPage0 internationalContentPage0)
        {
            
        }
        
        private void FillPersonPage0(PersonContentPage0 personContentPage0)
        {
            
        }
    }
}
