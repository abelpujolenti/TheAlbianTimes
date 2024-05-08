using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Workspace.Notebook.Pages.Map
{
    public class NotebookMapPage1 : NotebookMapPage
    {
        private const string MAP_IMAGES_FOLDER = "Images/Map/";
        
        [SerializeField] private GameObject _suoka;
        [SerializeField] private GameObject _zuania;
        [SerializeField] private GameObject _rekka;

        [SerializeField] private Image _mapImage;
        
        private Action _xayaClick;
        private Action _zuaniaClick;
        private Action _rekkaClick;
        private Action _suokaClick;

        private void Start()
        {
            int round = GameManager.Instance.GetRound();

            if (round < 3)
            {
                _countriesToFade = 1;
                _zuania.SetActive(false);
                _suoka.SetActive(false);
                _rekka.SetActive(false);
                _mapImage.sprite = Resources.Load<Sprite>(MAP_IMAGES_FOLDER  + "Map0RightSide");
                return;
            }

            if (round == 3)
            {
                _countriesToFade = 2;
                _suoka.SetActive(false);
                _rekka.SetActive(false);
                _mapImage.sprite = Resources.Load<Sprite>(MAP_IMAGES_FOLDER  + "Map1RightSide");
                return;
            }

            if (round == 4)
            {
                _countriesToFade = 2;
                _suoka.SetActive(false);
                _rekka.SetActive(false);
                _mapImage.sprite = Resources.Load<Sprite>(MAP_IMAGES_FOLDER  + "Map2RightSide");
                return;
            }

            if (round != 5)
            {
                _countriesToFade = 4;
                _mapImage.sprite = Resources.Load<Sprite>(MAP_IMAGES_FOLDER  + "MapRightSide");
                return;
            }

            
            _countriesToFade = 3;
            _suoka.SetActive(false);
            _mapImage.sprite = Resources.Load<Sprite>(MAP_IMAGES_FOLDER  + "Map3RightSide");
        }

        public override void FillPage(BaseNotebookPage baseNotebookPage)
        {
            MapPage1 page;
            
            try
            {
                page = (MapPage1)baseNotebookPage;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            _xayaClick = page.xayaClick;
            _suokaClick = page.suokaClick;
            _zuaniaClick = page.zuaniaClick;
            _rekkaClick = page.rekkaClick;
        }

        public void ClickXaya() => _xayaClick();
        public void ClickSuoka() => _suokaClick();
        public void ClickZuania() => _zuaniaClick();
        public void ClickRekka() => _rekkaClick();
        protected override void EraseClickers()
        {
            _xayaClick = () => { };
            _suokaClick = () => { };
            _zuaniaClick = () => { };
            _rekkaClick = () => { };
        }
    }
}