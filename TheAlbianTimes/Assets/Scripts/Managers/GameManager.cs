using Managers;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Editorial;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;
        public GameState gameState;
        private SaveManager saveManager;

        private LayoutManager _layoutManager;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            gameState = new GameState();
            saveManager = new SaveManager();
            if (saveManager.SaveFileExists())
            {
                saveManager.LoadFromJson();
            }
            LoadCountries();
            LoadPlayerData();
        }

        public void SetLayoutManager(LayoutManager layoutManager)
        {
            _layoutManager.CopyComponent(layoutManager);
        }

        public void SendNewsHeadlineToEditorialManager(GameObject newsHeadline)
        {
            EditorialManager.Instance.SendNewsHeadlineToNewsFolderCanvas(newsHeadline);
        }

        public void SendNewsHeadlineToLayoutManager(NewsHeadline newsHeadline)
        {

        }

        private void GenerateCountryEvents()
        {
            foreach (Country country in gameState.countries)
            {
                CountryEventManager.Instance.AddEventToQueue(country.GenerateEvent());
            }
        }

        private void LoadCountries()
        {
            Country[] countryObjects = transform.Find("Countries").GetComponentsInChildren<Country>();
            gameState.countries = new Country[(int)Country.Id.AMOUNT];
            foreach (Country country in countryObjects)
            {
                int index = (int)country.data.countryId;
                if (saveManager.SaveFileExists() && saveManager.save.countryData[index] != null)
                {
                    country.data = saveManager.save.countryData[index];
                }
                gameState.countries[index] = country;
            }
        }

        private void LoadPlayerData()
        {
            if (saveManager.SaveFileExists())
            {
                gameState.playerData = saveManager.save.playerData;
            }
            else
            {
                gameState.playerData = new GameStatePlayerData();
            }
        }
    }
}

    

   