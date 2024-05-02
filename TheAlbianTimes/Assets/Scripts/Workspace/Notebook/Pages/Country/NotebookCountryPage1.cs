using System;
using TMPro;
using UnityEngine;

namespace Workspace.Notebook.Pages.Country
{
    public class NotebookCountryPage1 : NotebookContentPage
    {
        [SerializeField] private TextMeshProUGUI _countryName;
        [SerializeField] private TextMeshProUGUI _history;

        public override void FillPage(BaseNotebookPage baseNotebookPage)
        {
            CountryContentPage1 page;
            
            try
            {
                page = (CountryContentPage1)baseNotebookPage;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            _countryName.text = page.countryName;
            _history.text = page.history;
        }
    }
}
