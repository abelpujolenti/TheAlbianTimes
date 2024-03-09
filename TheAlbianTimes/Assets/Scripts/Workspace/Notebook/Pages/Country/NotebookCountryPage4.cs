using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Workspace.Notebook.Pages.Country
{
    public class NotebookCountryPage4 : NotebookContentPage
    {
        [SerializeField] private TextMeshProUGUI _countryName;
        [SerializeField] private TextMeshProUGUI _ongoingConflicts;

        public override void FillPage(BaseNotebookPage baseNotebookPage)
        {
            CountryContentPage4 page;

            try
            {
                page = (CountryContentPage4)baseNotebookPage;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            _countryName.text = page.countryName;
            
            FillOngoingConflicts(page);
        }

        private void FillOngoingConflicts(CountryContentPage4 page)
        {
            foreach (string conflict in page.ongoingConflicts)
            {
                _ongoingConflicts.text += "\u2022" + conflict + "\n";
            }
        }
    }
}
