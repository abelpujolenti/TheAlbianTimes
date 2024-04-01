using System.Collections;
using Managers;
using TMPro;
using UnityEngine;

namespace Dialogue
{
    public class TextArchitect
    {
        #region Stats
        private TextMeshProUGUI tmpro_ui;
        private TextMeshPro tmpro_world;
        public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

        public string currentText => tmpro.text;
        public string targetText { get; private set; } = "";
        public string preText { get; private set; } = "";
        //private int preTextLength = 0;

        public string fullTargetText => preText + targetText;
        public enum BuildMethod { INSTANT, TYPE_WRITER, FADE }
        public BuildMethod currentMethod = BuildMethod.TYPE_WRITER;

        public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

        public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
        private const float baseSpeed = 1f;
        private float speedMultiplier = 1f;

        public int charactersPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
        private int characterMultiplier = 1;

        public bool hurryUp = false;
        #endregion

        private const string KEY = "Key Typing ";
        private const float MIN_VOLUME = 0.8f;
        private const float MAX_VOLUME = 1;
        private const float MIN_PITCH = 0.8f;
        private const float MAX_PITCH = 1.2f;

        private string[] _keyTypeSoundStrings;

        //private AudioSource[] _audioSourceKeyTypings;

        public TextArchitect(TextMeshProUGUI ui, int totalKeyTypeSoundStrings)
        {
            tmpro_ui = ui;
            _keyTypeSoundStrings = new string[totalKeyTypeSoundStrings];
            SetKeyTypingSoundsStrings();
        }

        //?????????
        public TextArchitect(TextMeshPro world, int totalKeyTypeSoundStrings)
        {
            tmpro_world = world;
        }
        //?????????

        public Coroutine Build(string text)
        {
            preText = "";
            targetText = text;

            Stop();

            buildProcess = tmpro.StartCoroutine(Building());
            return buildProcess;
        }

        public Coroutine Append(string text)
        {
            preText = tmpro.text;
            targetText = text;

            Stop();

            buildProcess = tmpro.StartCoroutine(Building());
            return buildProcess;
        }

        private Coroutine buildProcess = null;
        public bool isBuilding => buildProcess != null;

        private void Stop()
        {
            if (!isBuilding)
                return;

            tmpro.StopCoroutine(buildProcess);
            buildProcess = null;
        }

        private IEnumerator Building()
        {
            Prepare();

            switch (currentMethod)
            {
                case BuildMethod.TYPE_WRITER:
                    yield return Build_Typewriter();
                    break;
                case BuildMethod.FADE:
                    yield return Build_Fade();
                    break;
                default:
                    break;
            }

            OnComplete();
        }

        private IEnumerator Build_Typewriter()
        {
            int counter = 0;
            while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
            {
                tmpro.maxVisibleCharacters += hurryUp ? charactersPerCycle * 15 : charactersPerCycle;
                char currChar = tmpro.text[Mathf.Min(tmpro.text.Length - 1, tmpro.maxVisibleCharacters - 1)];
                if ((counter % 3 == 0 && currChar != '\u200b') || currChar == '.')
                {
                    PlayRandomAudioSource();
                }
                counter++;
                
                yield return new WaitForSeconds(0.015f / speed);
            }            
        }

        private IEnumerator Build_Fade()
        {
            yield return null;
        }

        void Prepare()
        {
            switch (currentMethod)
            {
                case BuildMethod.INSTANT:
                    Prepare_Instant();
                    break;
                case BuildMethod.TYPE_WRITER:
                    Prepare_Typewriter();
                    break;
                case BuildMethod.FADE:
                    Prepare_Fade();
                    break;
                default:
                    break;
            }
        }

        void Prepare_Instant()
        {
            tmpro.color = tmpro.color;
            tmpro.text = fullTargetText;
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }

        void Prepare_Typewriter()
        {
            tmpro.color = tmpro.color;
            tmpro.maxVisibleCharacters = 0;
            tmpro.text = preText;

            if (preText != "")
            {
                tmpro.ForceMeshUpdate();
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
            }

            tmpro.text += targetText;
            tmpro.ForceMeshUpdate();
        }

        void Prepare_Fade()
        {

        }

        void OnComplete()
        {
            buildProcess = null;
        }

        private void SetKeyTypingSoundsStrings()
        {
            for (int i = 0; i < _keyTypeSoundStrings.Length; i++)
            {
                _keyTypeSoundStrings[i] = KEY + i;
            }
        }

        void PlayRandomAudioSource() 
        {
            SoundManager.Instance.Play2DRandomSound(_keyTypeSoundStrings, 
                MIN_VOLUME, MAX_VOLUME, MIN_PITCH, MAX_PITCH);
        }
    }
}