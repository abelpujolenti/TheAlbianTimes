using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Utility;

namespace Editorial
{
    public class BiasButton : MovableRectTransform
    {
        private const String SEND_BIAS = "Send Bias";
        private const String CHANGE_BIAS = "Change Bias";
        
        [SerializeField] private TextMeshProUGUI _textMeshPro;

        private int _chosenBiasIndex;
        private int _selectedBiasIndex;

        private bool _readyToSend;

        private void OnEnable()
        {
            ActionsManager.OnChangeSelectedBiasIndex += ChangeButtonText;
            ActionsManager.OnChangeFrontNewsHeadline += ChangeChosenBiasIndex;
        }
        
        private void OnDisable()
        {
            ActionsManager.OnChangeSelectedBiasIndex -= ChangeButtonText;
            ActionsManager.OnChangeFrontNewsHeadline -= ChangeChosenBiasIndex;
        }

        new void Start()
        {
            base.Start();
            _textMeshPro.text = SEND_BIAS;
            _readyToSend = true;
        }

        protected override void PointerClick(BaseEventData data)
        {
            if (_readyToSend)
            {
                ActionsManager.OnSendNewsHeadline();
                return;
            }
            
            ActionsManager.OnChangeNewsHeadlineContent(_selectedBiasIndex);
        }

        private void ChangeButtonText(int newBiasIndex)
        {
            if (_chosenBiasIndex == newBiasIndex)
            {
                _textMeshPro.text = SEND_BIAS;
                _readyToSend = true;
                return;
            }

            _selectedBiasIndex = newBiasIndex;
            _textMeshPro.text = CHANGE_BIAS;
            _readyToSend = false;
        }

        private void ChangeChosenBiasIndex(int newChosenBiasIndex)
        {
            _chosenBiasIndex = newChosenBiasIndex;
            ChangeButtonText(newChosenBiasIndex);
        }
    }
}
