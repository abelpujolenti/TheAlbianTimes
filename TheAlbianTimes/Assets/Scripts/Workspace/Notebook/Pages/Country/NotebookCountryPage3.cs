using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Workspace.Notebook.Pages.Country
{
    public class NotebookCountryPage3 : NotebookContentPage
    {
        [SerializeField] private TextMeshProUGUI _countryName;
        [SerializeField] private TextMeshProUGUI _organizations;

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
            
            FillOrganizations(page);
        }

        private void FillOrganizations(CountryContentPage3 page)
        {
            foreach (KeyValuePair<string, List<string>> organization in page.organizations)
            {
                _organizations.text += "\u2022 " + organization.Key + "\n";

                foreach (string activity in organization.Value)
                {
                    _organizations.text += "\t- " + activity + "\n";
                }
            }
        }
    }
}
