using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Workspace.Notebook.Pages.Person
{
    public class NotebookPersonPage : NotebookContentPage
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Image _photo;
        [SerializeField] private TextMeshProUGUI _bulletPoints;

        public override void FillPage(BaseNotebookPage baseNotebookPage)
        {
            PersonContentPage0 page;

            try
            {
                page = (PersonContentPage0)baseNotebookPage;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            _name.text = page.name;
            //_photo.sprite = Resources.Load<Sprite>("Images/Icons/" + _name.text.ToLower());

            List<string> descriptions = page.descriptions;

            int listSize = descriptions.Count;
            
            for (int i = 0; i < listSize; i++)
            {
                _bulletPoints.text += "\u2022 " + descriptions[i] + "\n";
            }
        }
    }
}
