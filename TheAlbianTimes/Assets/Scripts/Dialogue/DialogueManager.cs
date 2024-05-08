using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private const int TOTAL_KEY_TYPES_AUDIOS = 7;

        private const float TIME_TO_SHOW_CHARACTER = 3.5f;
        
        private const string CHARACTERS_SPRITE_PATH = "Images/Characters/";
        private const string CLICK_BUTTON_SOUND = "Click Button";

        [SerializeField] private GameObject root;
        [SerializeField] private Image _character;
        [SerializeField] private RawImage background;
        [SerializeField] private TextMeshProUGUI speakerText;
        [SerializeField] private GameObject dialogueOptionButtonsRoot;
        [SerializeField] private GameObject continueText;

        DialogueSystem _dialogueSystem;
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
            _dialogueSystem = DialogueSystem.instance;
            architect = new TextArchitect(_dialogueSystem.container.dialogueText, TOTAL_KEY_TYPES_AUDIOS);

            architect.speed = 0.5f;
            dialogueOptionButtons = dialogueOptionButtonsRoot.GetComponentsInChildren<DialogueOptionButton>();

            if (!LoadDialogue())
            {
                GameManager.Instance.AddToRound();
                GameManager.Instance.LoadScene(ScenesName.WORKSPACE_SCENE);
                return;
            }

            continueText.SetActive(false);
            HideOptions();

            StartCoroutine(StartDialogueCoroutine());
        }

        private void OnGUI()
        {
            Event e = Event.current;
            if (!e.isMouse && !e.isKey || !(e.type == EventType.KeyDown) && !(e.type == EventType.MouseDown)) return;
            if (architect.isBuilding)
            {
                architect.hurryUp = true;
            }
            else if (continueText.activeSelf)
            {
                DisplayNextLine();
            }
        }

        private IEnumerator StartDialogueCoroutine()
        {
            yield return new WaitForSeconds(2.7f);
            DisplayNextLine();
        }

        private bool LoadDialogue()
        {
            string path = "Dialogues";
            dialogue = new SortedList<float, DialogueData>(new DuplicateKeyComparer<float>());

            FileManager.LoadAllJsonFiles(path, LoadDialogueFromFile);
            if (dialogue.Count == 0)
            {
                return false;
            }

            selectedDialogue = dialogue.Last().Value;

            string speakerName = selectedDialogue.lines[0].parts[0].speaker; 
            
            SetSpeakerText(speakerName);
            
            StartCoroutine(ShowCharacter(TIME_TO_SHOW_CHARACTER, speakerName));
            return true;
        }

        private IEnumerator ShowCharacter(float timeToShow, string speakerName)
        {
            float time = 0;

            _character.sprite = Resources.Load<Sprite>(CHARACTERS_SPRITE_PATH + speakerName);

            if (_character.sprite == null)
            {
                yield break;
            }

            Color color = _character.color;

            while (time < timeToShow)
            {
                time += Time.deltaTime;

                color.a = Mathf.Lerp(0, 1, time / timeToShow);

                _character.color = color;
                
                yield return null;
            }

            color.a = 1;

            _character.color = color;
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
            AudioManager.Instance.Play2DSound(CLICK_BUTTON_SOUND);

            currentLine++;

            architect.hurryUp = false;

            if (currentLine >= selectedDialogue.lines.Length)
            {
                GameManager.Instance.AddToRound();
                GameManager.Instance.LoadScene(ScenesName.WORKSPACE_SCENE);
                return;
            }
            continueText.SetActive(false);
            HideOptions();

            displayNextLineCoroutine = StartCoroutine(DisplayNextLineCoroutine());
        }

        private IEnumerator DisplayNextLineCoroutine()
        {
            int currentPartId = chosenOptionLinesRemaining > 0 ? chosenDialogueOptions.Last() : 0;
            currentPartId = currentPartId >= selectedDialogue.lines[currentLine].parts.Length ? selectedDialogue.lines[currentLine].parts.Length - 1 : currentPartId;
            chosenOptionLinesRemaining = Mathf.Max(0, chosenOptionLinesRemaining - 1);

            string text = ProcessDialogue(selectedDialogue.lines[currentLine].parts[currentPartId].text);
            string speaker = selectedDialogue.lines[currentLine].parts[currentPartId].speaker;

            SetSpeakerText(speaker);

            yield return displayTextCoroutine = architect.Build(text);

            //after text has finished typing
            if (selectedDialogue.lines[currentLine].options != null && selectedDialogue.lines[currentLine].options.Length > 0)
            {
                DisplayOptions();
                yield break;
            }
            StartCoroutine(ShowContinueTextCoroutine(0.3f));
        }

        private IEnumerator ShowContinueTextCoroutine(float t)
        {
            yield return new WaitForSeconds(t);
            continueText.SetActive(true);
        }

        private string ProcessDialogue(string text)
        {
            string ret = text;
            ret = Regex.Replace(ret, "¬", "\u200b\u200b\u200b\u200b\u200b\u200b\u200b");
            ret = Regex.Replace(ret, "\\. ", ". \u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b");
            ret = Regex.Replace(ret, "! ", "! \u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b");
            ret = Regex.Replace(ret, "\\? ", "? \u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b");
            ret = Regex.Replace(ret, ", ", ", \u200b\u200b\u200b\u200b\u200b\u200b\u200b");
            ret = Regex.Replace(ret, "\\.{3}", "\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b.\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b.\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b\u200b.");
            return ret;
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
                float duration = architect.hurryUp ? .08f : .3f;
                float delay = duration * 0.8f * i;
                dialogueOptionButtons[i].Setup(selectedDialogue.lines[currentLine].options[i], duration, delay);
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