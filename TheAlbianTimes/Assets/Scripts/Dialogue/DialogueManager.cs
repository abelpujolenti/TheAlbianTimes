using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace NoMonoBehavior
{
    public class DialogueData
    {
        public string[] piecesCoordinatesFromRootPiece;
    }

    public class DialogueObject
    {
        public string character;
        public string name; 
        public DialogueData[] dialogue;
    }
}


namespace Managers
{
    public class DialogueManager : MonoBehaviour
    {
        #region Stats
        [SerializeField] private Image character;
        [SerializeField] private RawImage background;

        private string[] words = new string[4]
        {
        "hey ho, hello, i'm here",
        "im a pen",
        "use me to mark positive stuff",
        "oh yeah and to make sure i speak correctly i really wanted to express my gratitude to Albia for letting me write this paper its been amazing"
        };

        private int currentWords = 0;

        DialogueSystem ds;
        NoMonoBehavior.TextArchitect architect;

        #endregion

        void Start()
        {
            ds = DialogueSystem.instance;
            architect = new NoMonoBehavior.TextArchitect(ds.container.dialogueText);
            architect.Build(words[currentWords]);

            LoadDialogueFromJson("test.json");
        }

        public void ContinueText()
        {
            currentWords++;
            architect.Build(words[currentWords]);
        }

        //private void SaveDialogueToJson<TContent>(TContent)

        private void LoadDialogueFromJson(string fileName)
        {
            string path = Application.streamingAssetsPath + "/Json/Dialogues/" + fileName;

            if (!File.Exists(path))
                return;

            string json = File.ReadAllText(path);

        }
    }
}