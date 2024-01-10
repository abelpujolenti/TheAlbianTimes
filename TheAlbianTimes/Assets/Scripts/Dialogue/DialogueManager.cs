using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NoMonoBehavior;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private Image character;
        [SerializeField] private RawImage background;
        [SerializeField] private TextMeshProUGUI speakerText;
        [SerializeField] private GameObject dialogueOptionButtonsRoot;
        [SerializeField] private GameObject continueButton;

        DialogueSystem ds;
        TextArchitect architect;
        DialogueOptionButton[] dialogueOptionButtons;

        DialogueData selectedDialogue;

        SortedList<float, DialogueData> dialogue;

        Coroutine displayNextLineCoroutine;
        Coroutine displayTextCoroutine;

        private int currentLine = -1;
        private List<int> chosenDialogueOptions = new List<int>();
        private int chosenOptionLinesRemaining = 0;

        void Start()
        {
            ds = DialogueSystem.instance;
            architect = new TextArchitect(ds.container.dialogueText);
            dialogueOptionButtons = dialogueOptionButtonsRoot.GetComponentsInChildren<DialogueOptionButton>();

            LoadDialogue();

            DisplayNextLine();
        }

        private void LoadDialogue()
        {
            string path = "Dialogue";
            dialogue = new SortedList<float, DialogueData>(new DuplicateKeyComparer<float>());

            FileManager.LoadAllJsonFiles(path, LoadDialogueFromFile);
            if (dialogue.Count == 0) return;

            selectedDialogue = dialogue.Last().Value;
        }

        private void LoadDialogueFromFile(string json)
        {
            DialogueData dialogueData = JsonUtility.FromJson<DialogueData>(json);

            int round = GameManager.Instance.GetRound();

            if (!dialogueData.ConditionsFulfilled(round)) return;
            float priority = dialogueData.priority;

            if (priority > 0)
            {
                dialogue.Add(priority, dialogueData);
            }
        }

        private void DisplayNextLine()
        {
            currentLine++;

            if (currentLine >= selectedDialogue.lines.Length)
            {
                GameManager.Instance.sceneLoader.SetScene("WorkspaceScene");
                return;
            }

            continueButton.SetActive(false);
            HideOptions();

            displayNextLineCoroutine = StartCoroutine(DisplayNextLineCoroutine());
        }

        private IEnumerator DisplayNextLineCoroutine()
        {
            int currentPartId = chosenOptionLinesRemaining > 0 ? chosenDialogueOptions.Last() : 0;
            currentPartId = currentPartId >= selectedDialogue.lines[currentLine].parts.Length ? currentPartId = selectedDialogue.lines[currentLine].parts.Length - 1 : currentPartId;
            chosenOptionLinesRemaining = Mathf.Max(0, chosenOptionLinesRemaining - 1);

            string text = selectedDialogue.lines[currentLine].parts[currentPartId].text;
            string speaker = selectedDialogue.lines[currentLine].parts[currentPartId].speaker;

            SetSpeakerText(speaker);

            yield return displayTextCoroutine = architect.Build(text);

            //after text has finished typing
            if (selectedDialogue.lines[currentLine].options.Length > 0)
            {
                DisplayOptions();
            }
            else
            {
                continueButton.SetActive(true);
            }
        }

        private void SetSpeakerText(string text)
        {
            speakerText.text = text;
        }

        private void DisplayOptions()
        {
            for (int i = 0; i < selectedDialogue.lines[currentLine].options.Length && i < dialogueOptionButtons.Length; i++)
            {
                dialogueOptionButtons[i].gameObject.SetActive(true);
                dialogueOptionButtons[i].Setup(selectedDialogue.lines[currentLine].options[i]);
            }
        }

        private void HideOptions()
        {
            foreach(DialogueOptionButton button in dialogueOptionButtons)
            {
                button.gameObject.SetActive(false);
            }
        }

        //buttons
        public void SelectOption(int optionId, int buttonId)
        {
            chosenDialogueOptions.Add(optionId);
            chosenOptionLinesRemaining = selectedDialogue.lines[currentLine].options[buttonId].followupLines;
            DisplayNextLine();
        }
        
        public void NextButtonOnClick()
        {
            DisplayNextLine();
        }

        //loader
        private void LoadDialogueFromJson(string fileName)
        {
            string path = Application.streamingAssetsPath + "/Json/Dialogues/" + fileName;

            if (!File.Exists(path))
                return;

            string json = File.ReadAllText(path);

            selectedDialogue = JsonUtility.FromJson<DialogueData>(json);
        }
    }
}