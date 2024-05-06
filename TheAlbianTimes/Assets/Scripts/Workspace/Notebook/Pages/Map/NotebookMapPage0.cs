using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Workspace.Notebook.Pages.Map
{
    public class NotebookMapPage0 : NotebookMapPage
    {
        private const string MAP_IMAGES_FOLDER = "Images/Map/";
        
        [SerializeField] private GameObject _hetia;
        [SerializeField] private GameObject _terkan;
        [SerializeField] private GameObject _dalme;

        [SerializeField] private Image _mapImage;
        
        private Action _hetiaClick;
        private Action _terkanClick;
        private Action _albiaClick;
        private Action _dalmeClick;
        private Action _madiaClick;

        private void Start()
        {
            int round = GameManager.Instance.GetRound();

            if (round < 3)
            {
                _hetia.SetActive(false);
                _terkan.SetActive(false);
                _dalme.SetActive(false);
                _mapImage.sprite = Resources.Load<Sprite>(MAP_IMAGES_FOLDER  + "Map0LeftSide");
                return;
            }

            if (round == 3)
            {
                _hetia.SetActive(false);
                _terkan.SetActive(false);
                _dalme.SetActive(false);
                _mapImage.sprite = Resources.Load<Sprite>(MAP_IMAGES_FOLDER  + "Map1LeftSide");
                return;
            }

            if (round == 4)
            {
                _dalme.SetActive(false);
                _mapImage.sprite = Resources.Load<Sprite>(MAP_IMAGES_FOLDER  + "Map2LeftSide");
                return;
            }

            if (round != 5)
            {
                _mapImage.sprite = Resources.Load<Sprite>(MAP_IMAGES_FOLDER  + "MapLeftSide");
                return;
            }
            
            _dalme.SetActive(false);
            _mapImage.sprite = Resources.Load<Sprite>(MAP_IMAGES_FOLDER  + "Map3LeftSideV2");
        }

        public override void FillPage(BaseNotebookPage baseNotebookPage)
        {
            MapPage0 page;
            
            try
            {
                page = (MapPage0)baseNotebookPage;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            _hetiaClick = page.hetiaClick;
            _terkanClick = page.terkanClick;
            _albiaClick = page.albiaClick;
            _dalmeClick = page.dalmeClick;
            _madiaClick = page.madiaClick;
        }

        public void ClickHetia() => _hetiaClick();
        public void ClickTerkan() => _terkanClick();
        public void ClickAlbia() => _albiaClick();
        public void ClickDalme() => _dalmeClick();
        public void ClickMadia() => _madiaClick();
        protected override void EraseClickers()
        {
            _hetiaClick = () => { };
            _terkanClick = () => { };
            _albiaClick = () => { };
            _dalmeClick = () => { };
            _madiaClick = () => { };
        }
    }
}
