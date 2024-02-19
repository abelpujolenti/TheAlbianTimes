using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Workspace.Notebook.Pages.Country
{
    public class NotebookCountryPage1 : NotebookContentPage
    {
        [SerializeField] private TextMeshProUGUI _countryName;
        [SerializeField] private TextMeshProUGUI _importantPeople;
        [SerializeField] private TextMeshProUGUI _organizations;

        public override void FillPage(BaseNotebookPage baseNotebookPage)
        {
            Debug.Log("page "+ 1);
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
            
            FillImportantPeople(page);
            
            FillOrganizations(page);
        }

        private void FillImportantPeople(CountryContentPage1 page)
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

        private void FillOrganizations(CountryContentPage1 page)
        {
            foreach (KeyValuePair<string, List<string>> organization in page.organizations)
            {
                _organizations.text += "\\u2022 " + organization.Key + "\n";

                foreach (string activity in organization.Value)
                {
                    _organizations.text += "\t- " + activity + "\n";
                }
            }
        }
    }
}
