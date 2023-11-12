using System;
using Managers;
using TMPro;
using UnityEngine;

namespace Editorial
{
    public class BiasContainer : MonoBehaviour
    {
        [SerializeField] private Bias[] _bias;

        [SerializeField] private TextMeshProUGUI _biasesdescriptionText;

        private String[] _biasesDescriptions;

        private int _totalBiasActive;

        private void OnEnable()
        {
            EventsManager.OnChangeFrontNewsHeadline += ChangeSelectedBias;
            EventsManager.OnSettingNewBiases += SetBias;
            EventsManager.OnChangeToNewBias += DeactivateBias;
            EventsManager.OnChangeSelectedBiasIndex += ChangeBiasDescription;
        }

        private void OnDisable()
        {
            EventsManager.OnChangeFrontNewsHeadline -= ChangeSelectedBias;
            EventsManager.OnSettingNewBiases -= SetBias;
            EventsManager.OnChangeToNewBias -= DeactivateBias;
            EventsManager.OnChangeSelectedBiasIndex -= ChangeBiasDescription;
        }

        private void Start()
        {
            EditorialManager.Instance.SetBiasContainerCanvas(gameObject);
            gameObject.SetActive(false);
        }

        private void SetBias(String[] biasesNames, String[] biasesDescriptions)
        {
            _totalBiasActive = biasesDescriptions.Length;
            
            for (int i = 0; i < _totalBiasActive; i++)
            {
                _bias[i].gameObject.SetActive(true);
                _bias[i].SetText(biasesNames[i]);
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

        private void DeactivateBias()
        {
            for (int i = 0; i < _totalBiasActive; i++)
            {
                _bias[i].gameObject.SetActive(false);
            }
            _biasesdescriptionText.transform.parent.gameObject.SetActive(false);
        }
    }
}
