using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager Instance => _instance;

        private LayoutManager _layoutManager;

        private float counter = 5;

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

        /*private void Update()
        {
            counter -= Time.deltaTime;

            if (counter < 0f)
            {
                LayoutManager obj = Instantiate(_layoutManager);
                //obj.AddComponent<LayoutManager>();
                counter = 10;
            }
        }*/

        public void SetLayoutManager(LayoutManager layoutManager)
        {
            _layoutManager.CopyComponent(layoutManager);
        }
    }
}
