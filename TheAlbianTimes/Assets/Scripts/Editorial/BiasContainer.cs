using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Editorial
{
    public class BiasContainer : MonoBehaviour
    {
        [SerializeField] private Bias[] _bias;

        [SerializeField] private TextMeshProUGUI _biasesdescriptionText;

        private String[] _biasesDescriptions;

        private int _totalBiasesToActive;

        private void OnEnable()
        {
            EventsManager.OnChangeFrontNewsHeadline += ChangeSelectedBias;
            EventsManager.OnChangeSelectedBiasIndex += ChangeBiasDescription;
            EventsManager.OnSettingNewBiases += SetBias;
        }

        private void OnDisable()
        {
            EventsManager.OnChangeFrontNewsHeadline -= ChangeSelectedBias;
            EventsManager.OnChangeSelectedBiasIndex -= ChangeBiasDescription;
            EventsManager.OnSettingNewBiases -= SetBias;
        }

        private void Start()
        {
            EditorialManager.Instance.SetBiasContainerCanvas(gameObject);
            gameObject.SetActive(false);
        }

        private void SetBias(String[] biasesNames, String[] biasesDescriptions)
        {
            _totalBiasesToActive = biasesDescriptions.Length;

            GameObject bias;

            for (int i = 0; i < _bias.Length; i++)
            {
                bias = _bias[i].gameObject;
                
                if (i < _totalBiasesToActive)
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

            if (_totalBiasesToActive == 0)
            {
                _biasesdescriptionText.transform.parent.gameObject.SetActive(false);
                return;
            }
            _biasesdescriptionText.transform.parent.gameObject.SetActive(true);

            _biasesDescriptions = new String[biasesDescriptions.Length];
            _biasesDescriptions = biasesDescriptions;
        }

        private void ChangeSelectedBias(int newSelectedBiasIndex)
        {
            if (EventsManager.OnChangeSelectedBias != null)
            {
                EventsManager.OnChangeSelectedBias();
            }
            _bias[newSelectedBiasIndex].SelectBias();
            EventsManager.OnChangeSelectedBiasIndex(newSelectedBiasIndex);
        }

        private void ChangeBiasDescription(int newSelectedBiasIndex)
        {
            _biasesdescriptionText.text = _biasesDescriptions[newSelectedBiasIndex];
        }
    }
}
