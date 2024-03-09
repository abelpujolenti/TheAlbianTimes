using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Workspace.Notebook.Pages.Country
{
    public class NotebookCountryPage2 : NotebookContentPage
    {
        [SerializeField] private TextMeshProUGUI _countryName;
        [SerializeField] private TextMeshProUGUI _importantPeople;

        public override void FillPage(BaseNotebookPage baseNotebookPage)
        {
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
            
            FillImportantPeople(page);
        }

        private void FillImportantPeople(CountryContentPage2 page)
        {
            foreach (KeyValuePair<string, List<string>> importantPerson in page.importantPeople)
            {
                _importantPeople.text += "\u2022 " + importantPerson.Key + "\n";

                foreach (string activity in importantPerson.Value)
                {
                    _importantPeople.text += "\t- " + activity + "\n";
                }
            }
        }

    }
}
