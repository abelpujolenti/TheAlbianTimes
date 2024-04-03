using UnityEngine;

namespace Dialogue
{
    public class DialogueSystem : MonoBehaviour
    {
        public DialogueContainer container = new DialogueContainer();

        public static DialogueSystem instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                DestroyImmediate(gameObject);

        }
    }
}
