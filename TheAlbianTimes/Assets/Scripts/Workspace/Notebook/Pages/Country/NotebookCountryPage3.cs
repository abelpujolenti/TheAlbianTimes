using System;
using TMPro;
using UnityEngine;

namespace Workspace.Notebook.Pages.Country
{
    public class NotebookCountryPage3 : NotebookContentPage
    {
        [SerializeField] private TextMeshProUGUI _countryName;

        public override void FillPage(BaseNotebookPage baseNotebookPage)
        {
            CountryContentPage3 page;

            try
            {
                page = (CountryContentPage3)baseNotebookPage;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            _countryName.text = page.countryName;
        }
    }
}
