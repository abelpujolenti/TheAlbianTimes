using System;
using System.Collections;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Workspace.Layout.Pieces;
using Random = UnityEngine.Random;

namespace Workspace.Editorial
{
    public class BiasContainer : MonoBehaviour
    {
        private const String POST_IT_SOUND = "Post it";
        
        [SerializeField] private Bias[] _bias;

        [SerializeField] private GameObject _biasDescriptionRoot;
        [SerializeField] private Transform _biasDescriptionPostitPrefab;

        [SerializeField] private NewsFolder _newsFolder;
        
        [SerializeField] private Tray _tray;

        private string[] _biasesDescriptions;

        private int _totalBiasesToActive;

        private int maxPostits = 4;

        [SerializeField] private float postitInitialScale = 3f;
        [SerializeField] private float postitScaleAnimationTime = .2f;
        [SerializeField] private float postitAppearDelay = .5f;

        private void OnEnable()
        {
            EventsManager.OnChangeFrontNewsHeadline += ChangeSelectedBias;
            EventsManager.OnChangeSelectedBiasIndex += ChangeBiasDescription;
            EventsManager.OnSettingNewBiases += SetBias;

            FixPostitScale();
        }

        private void OnDisable()
        {
            EventsManager.OnChangeFrontNewsHeadline -= ChangeSelectedBias;
            EventsManager.OnChangeSelectedBiasIndex -= ChangeBiasDescription;
            EventsManager.OnSettingNewBiases -= SetBias;
        }

        private void Awake()
        {
            EditorialManager.Instance.SetBiasContainerCanvas(gameObject);
            gameObject.SetActive(false);
        }

        private void Start()
        {
            foreach (Bias bias in _bias)
            {
                bias.SetBiasContainer(this);
            }
        }

        private void SetBias(string[] biasesNames, string[] biasesDescriptions, int totalBiasesToActivate)
        {
            GameObject bias;

            for (int i = 0; i < _bias.Length; i++)
            {
                bias = _bias[i].gameObject;
                
                if (i < totalBiasesToActivate)
                {
                    _bias[i].SetText(biasesNames[i]);
                    if (bias.activeSelf)
                    {
                        continue;
                    }

                    bias.SetActive(true);
                }
                else
                {
                    if (bias.activeSelf)
                    {
                        bias.SetActive(false);
                    }
                }
            }

            _biasesDescriptions = new string[biasesDescriptions.Length];
            _biasesDescriptions = biasesDescriptions;
        }

        private void ChangeSelectedBias(int newSelectedBiasIndex)
        {
            if (EventsManager.OnChangeSelectedBias != null)
            {
                EventsManager.OnChangeSelectedBias();
            }
            for (int i = 0; i < _bias.Length; i++)
            {
                if (i == newSelectedBiasIndex) {
                    _bias[i].SelectBias();
                }
                else
                {
                    _bias[i].ResetBiasUnderline();
                }
            }
            ChangeBiasDescription(newSelectedBiasIndex);
        }

        private void ChangeBiasDescription(int newSelectedBiasIndex)
        {
            StartCoroutine(SpawnPostitCoroutine(newSelectedBiasIndex, postitAppearDelay));
             
            if (_newsFolder.GetFrontHeadline().GetChosenBiasIndex() != newSelectedBiasIndex)
            {            
                ExtendTray();
                return;
            }
            HideTray(null);
        }

        private void ExtendTray()
        {
            _tray.Extend();
        }

        private void HideTray(GameObject newsHeadline)
        {
            _tray.Hide(newsHeadline);
        }

        private IEnumerator SpawnPostitCoroutine(int newSelectedBiasIndex, float delay)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitUntil(() => !_bias[newSelectedBiasIndex].IsMarkAnimationRunning());
            yield return new WaitForSeconds(delay);

            if (!_bias[newSelectedBiasIndex].IsSelected()) yield break;

            Vector3 position = _biasDescriptionRoot.transform.position;
            Quaternion rotation = _biasDescriptionRoot.transform.childCount > 0 ? Quaternion.Euler(0f, 0f, (int)(Random.Range(-8f, 8f) / 2f) * 2) : Quaternion.identity;
            Image biasDescriptionPostit = Instantiate(_biasDescriptionPostitPrefab, position, rotation, _biasDescriptionRoot.transform).GetComponent<Image>();

            if (_biasDescriptionRoot.transform.childCount > maxPostits)
            {
                Destroy(_biasDescriptionRoot.transform.GetChild(0).gameObject);
            }

            ApplyPostitShading();

            biasDescriptionPostit.color = ColorUtil.SetSaturation(PieceData.biasColors[newSelectedBiasIndex], .3f);

            TextMeshProUGUI biasdescriptionText = biasDescriptionPostit.transform.Find("BiasDescriptionText").GetComponent<TextMeshProUGUI>();
            biasdescriptionText.text = _biasesDescriptions[newSelectedBiasIndex];

            SoundManager.Instance.Play3DSound(POST_IT_SOUND, 10, 100, gameObject.transform.position);
            yield return ScaleAnimationCoroutine(biasDescriptionPostit.transform, postitScaleAnimationTime, postitInitialScale, 1f);
        }

        private void ApplyPostitShading()
        {
            float v;
            for (int i = _biasDescriptionRoot.transform.childCount - 2; i >= 0; i--)
            {
                Image postitImage = _biasDescriptionRoot.transform.GetChild(i).GetComponent<Image>();

                v = .2f + (maxPostits - (_biasDescriptionRoot.transform.childCount - 2 - i)) * .12f;
                postitImage.color = ColorUtil.SetBrightness(postitImage.color, v);
            }
        }

        private IEnumerator ScaleAnimationCoroutine(Transform transformToChange, float t, float start, float end)
        {
            float elapsedT = 0f;
            while (elapsedT <= t)
            {
                float s = Mathf.Lerp(start, end, Mathf.Pow(elapsedT / t, 2.5f));
                transformToChange.localScale = new Vector3(s, s, s);
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            transformToChange.localScale = new Vector3(end, end, end);
        }

        private void FixPostitScale()
        {
            for (int i = 0; i < _biasDescriptionRoot.transform.childCount; i++)
            {
                Transform t = _biasDescriptionRoot.transform.GetChild(i);
                t.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
}
