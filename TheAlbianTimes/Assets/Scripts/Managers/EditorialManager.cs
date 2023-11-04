using Managers.ScriptableObjects;
using UnityEngine;

namespace Managers
{
    public class EditorialManager : MonoBehaviour
    {

        private EditorialManager _instance;

        public EditorialManager Instance => _instance;

        [SerializeField] private EditorialManagerData _editorialManagerData;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
