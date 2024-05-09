using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utility;
using Workspace.Layout.Pieces;
using Random = UnityEngine.Random;

namespace Workspace.Editorial
{
    public class BiasContainer : MonoBehaviour
    {
        private const String POST_IT_SOUND = "Post it";
        private const int MAX_POSTITS_PER_COLOR = 2;
        
        [SerializeField] private Bias[] _bias;

        [SerializeField] private RectTransform[] _biasesDescriptionRectTransform;
        [SerializeField] private Transform _biasDescriptionPostitPrefab;

        [SerializeField] private GameObject _biasContainer;

        [SerializeField] private NewsFolder _newsFolder;

        private Dictionary<int, Queue<Postit>> _postitsPools;

        private string[] _biasesDescriptions;

        private int _totalBiasesToActive;

        [SerializeField] private float postitInitialScale = 3f;
        [SerializeField] private float postitScaleAnimationTime = .2f;
        [SerializeField] private float postitAppearDelay = .5f;

        private void OnEnable()
        {
            EventsManager.OnChangeFrontNewsHeadline += ChangeChosenBias;
            EventsManager.OnSettingNewBiases += SetBias;

            FixPostitScale();
        }

        private void OnDisable()
        {
            EventsManager.OnChangeFrontNewsHeadline -= ChangeChosenBias;
            EventsManager.OnSettingNewBiases -= SetBias;
        }

        private void Start()
        {
            InitializePostits();
            
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
                    continue;
                }
                
                if (!bias.activeSelf)
                {
                    continue;
                }
                bias.SetActive(false);
            }

            _biasesDescriptions = biasesDescriptions;

            for (int i = 0; i < _postitsPools.Count; i++)
            {
                Postit previousPostit = _postitsPools[i].Dequeue(); 
                
                if (i < _biasesDescriptions.Length)
                {
                    StartCoroutine(SpawnPostitCoroutine(i, postitAppearDelay, previousPostit));
                    continue;
                }
                
                previousPostit.StartThrowAway();
                _postitsPools[i].Enqueue(previousPostit);
            }
        }

        private void ChangeChosenBias(int newChosenBiasIndex)
        {
            if (EventsManager.OnChangeChosenBias != null)
            {
                EventsManager.OnChangeChosenBias();
            }
            
            for (int i = 0; i < _bias.Length; i++)
            {
                if (i == newChosenBiasIndex) 
                {
                    _bias[i].SelectBias();
                    continue;
                }
                _bias[i].ResetBiasUnderline();
            }
        }

        private IEnumerator SpawnPostitCoroutine(int biasDescriptionIndex, float delay, Postit previousPostit)
        {
            previousPostit.StartThrowAway();

            Postit postit = _postitsPools[biasDescriptionIndex].Dequeue();
            _postitsPools[biasDescriptionIndex].Enqueue(postit);
            _postitsPools[biasDescriptionIndex].Enqueue(previousPostit);
            postit.SetText(_biasesDescriptions[biasDescriptionIndex]);
            
            yield return new WaitForFixedUpdate();
            yield return new WaitUntil(() => !_bias[biasDescriptionIndex].IsMarkAnimationRunning());
            yield return new WaitForSeconds(delay);

            Vector3 position = _biasesDescriptionRectTransform[biasDescriptionIndex].position;
            Quaternion rotation = Quaternion.Euler(0f, 0f, (int)(Random.Range(-8f, 8f) / 2f) * 2);
            
            GameObject postitGameObject = postit.gameObject;
            
            postitGameObject.SetActive(true);

            Transform postitTransform = postitGameObject.transform;

            postitTransform.position = position;
            postitTransform.rotation = rotation;

            AudioManager.Instance.Play3DSound(POST_IT_SOUND, 5, 100, position);
            yield return ScaleAnimationCoroutine(postitTransform, postitScaleAnimationTime, postitInitialScale, 1f);
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
            foreach (RectTransform rectTransform in _biasesDescriptionRectTransform)
            {
                for (int j = 0; j < rectTransform.childCount; j++)
                {
                    Transform postitTransform = rectTransform.GetChild(j);
                    postitTransform.localScale = new Vector3(1f, 1f, 1f);
                }
            }
        }

        private void InitializePostits()
        {
            Queue<Postit> greenPostitPool = new Queue<Postit>();
            Queue<Postit> bluePostitPool = new Queue<Postit>();
            Queue<Postit> redPostitPool = new Queue<Postit>();
            Queue<Postit> extraPostitPool = new Queue<Postit>();

            _postitsPools = new Dictionary<int, Queue<Postit>>
            {
                {0, greenPostitPool},
                {1, bluePostitPool},
                {2, redPostitPool},
                {3, extraPostitPool},
            };

            for (int i = 0; i < _postitsPools.Count; i++)
            {
                for (int j = 0; j < MAX_POSTITS_PER_COLOR; j++)
                {
                    Postit postit = Instantiate(_biasDescriptionPostitPrefab,_biasesDescriptionRectTransform[i]).GetComponent<Postit>();
                    
                    postit.gameObject.SetActive(false);

                    postit.gameObject.name = i + j.ToString();

                    postit.GetImage().color = ColorUtil.SetSaturation(PieceData.biasColors[i], 0.3f);
                    
                    _postitsPools[i].Enqueue(postit);
                }
            }
        }

        public void ThrowAllPostitsAway()
        {
            for (int i = 0; i < _postitsPools.Count; i++)
            {
                
            }
        }

        private void ThowPostitAway(Transform postitTransform)
        {
            
        }
    }
}
