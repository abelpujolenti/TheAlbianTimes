using System;
using Managers;
using UnityEngine;

namespace Editorial
{
    public class BiasContainer : MonoBehaviour
    {
        [SerializeField] private Bias[] _bias;

        private int _totalBiasActive;

        private void OnEnable()
        {
            EventsManager.OnChangeFrontNewsHeadline += ChangeSelectedBias;
            EventsManager.OnSettingNewBiasDescription += SetBias;
            EventsManager.OnChangeToNewBias += DeactivateBias;
        }

        private void OnDisable()
        {
            EventsManager.OnChangeFrontNewsHeadline -= ChangeSelectedBias;
            EventsManager.OnSettingNewBiasDescription -= SetBias;
            EventsManager.OnChangeToNewBias -= DeactivateBias;
        }

        private void Start()
        {
            EditorialManager.Instance.SetBiasContainerCanvas(gameObject);
            gameObject.SetActive(false);
        }

        private void SetBias(String[] shortBiasDescription)
        {
            _totalBiasActive = shortBiasDescription.Length;
            
            for (int i = 0; i < _totalBiasActive; i++)
            {
                _bias[i].gameObject.SetActive(true);
                _bias[i].SetText(shortBiasDescription[i]);
            }
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

        private void DeactivateBias()
        {
            for (int i = 0; i < _totalBiasActive; i++)
            {
                _bias[i].gameObject.SetActive(false);
            }
        }
    }
}
