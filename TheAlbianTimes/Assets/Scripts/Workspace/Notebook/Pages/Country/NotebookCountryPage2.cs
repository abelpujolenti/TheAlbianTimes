using System;
using TMPro;
using UnityEngine;

namespace Workspace.Notebook.Pages.Country
{
    public class NotebookCountryPage2 : NotebookContentPage
    {
        [SerializeField] private TextMeshProUGUI _countryName;
        [SerializeField] private TextMeshProUGUI _ongoingConflicts;

        public override void FillPage(BaseNotebookPage baseNotebookPage)
        {
            Debug.Log("page "+2);
            CountryContentPage2 page;

            try
            {
                page = (CountryContentPage2)baseNotebookPage;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            _countryName.text = page.countryName;
            
            FillOngoingConflicts(page);
        }

        private void FillOngoingConflicts(CountryContentPage2 page)
        {
            foreach (string conflict in page.ongoingConflicts)
            {
                _ongoingConflicts.text += "\u2022" + conflict + "\n";
            }
        }
    }
}
