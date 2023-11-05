using Managers.ScriptableObjects;
using UnityEngine;

namespace Managers
{
    public class EditorialManager : MonoBehaviour
    {

        private EditorialManager _instance;

        public EditorialManager Instance => _instance;

        [SerializeField] private EditorialManagerData _editorialManagerData;
    }
}
