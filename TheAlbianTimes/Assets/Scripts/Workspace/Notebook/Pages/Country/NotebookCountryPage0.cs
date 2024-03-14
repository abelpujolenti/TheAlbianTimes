using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Workspace.Notebook.Pages.Country
{
    public class NotebookCountryPage0 : NotebookContentPage
    {
        [SerializeField] private TextMeshProUGUI _countryName;
        [SerializeField] private Image _flagImage;
        [SerializeField] private TextMeshProUGUI _description;

        public override void FillPage(BaseNotebookPage baseNotebookPage)
        {
            CountryContentPage0 page;
            
            try
            {
                page = (CountryContentPage0)baseNotebookPage;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            _countryName.text = page.countryName;
            //_flagImage.sprite =
            _description.text = page.description;
        }
    }
}
