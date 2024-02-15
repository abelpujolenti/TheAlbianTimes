using System;
using UnityEngine;

namespace Managers
{
    public class NotebookManager : MonoBehaviour
    {
        private static NotebookManager _instance;

        public static NotebookManager Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
