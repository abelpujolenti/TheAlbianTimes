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
            ActionsManager.OnChangeFrontNewsHeadline += ChangeBias;
            ActionsManager.OnSettingNewBiasDescription += SetBias;
            ActionsManager.OnChangeToNewBias += DeactivateBias;
        }

        private void OnDisable()
        {
            ActionsManager.OnChangeFrontNewsHeadline -= ChangeBias;
            ActionsManager.OnSettingNewBiasDescription -= SetBias;
            ActionsManager.OnChangeToNewBias -= DeactivateBias;
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

        private void ChangeBias(int newSelectedBiasIndex)
        {
            if (ActionsManager.OnChangeSelectedBias != null)
            {
                ActionsManager.OnChangeSelectedBias();
            }
            _bias[newSelectedBiasIndex].SelectBias();
            ActionsManager.OnChangeSelectedBiasIndex(newSelectedBiasIndex);
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
