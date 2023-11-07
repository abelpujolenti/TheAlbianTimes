using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Editorial
{
    public class BiasButton : MovableRectTransform
    {
        private const String SEND_BIAS = "Send News Headline";
        private const String CHANGE_BIAS = "Change Bias";
        
        [SerializeField] private TextMeshProUGUI _textMeshPro;

        private int _chosenBiasIndex;
        private int _selectedBiasIndex;

        private bool _readyToSend;

        private void OnEnable()
        {
            ActionsManager.OnChangeSelectedBiasIndex += ChangeSelectedBias;
            ActionsManager.OnChangeFrontNewsHeadline += ChangeChosenBiasIndex;
        }
        
        private void OnDisable()
        {
            ActionsManager.OnChangeSelectedBiasIndex -= ChangeSelectedBias;
            ActionsManager.OnChangeFrontNewsHeadline -= ChangeChosenBiasIndex;
        }

        new void Start()
        {
            base.Start();
            
            ChangeButtonText();
        }

        protected override void PointerClick(BaseEventData data)
        {
            ActionsManager.OnChangeToNewBias();
            if (_readyToSend)
            {
                ActionsManager.OnSendNewsHeadline();
                return;
            }
            ActionsManager.OnChangeNewsHeadlineContent(_selectedBiasIndex);
        }

        private void ChangeButtonText()
        {
            if (_chosenBiasIndex == _selectedBiasIndex)
            {
                _textMeshPro.text = SEND_BIAS;
                _readyToSend = true;
                return;
            }
            _textMeshPro.text = CHANGE_BIAS;
            _readyToSend = false;
        }

        private void ChangeSelectedBias(int newSelectedBiasIndex)
        {
            _selectedBiasIndex = newSelectedBiasIndex;
            ChangeButtonText();
        }

        private void ChangeChosenBiasIndex(int newChosenBiasIndex)
        {
            _chosenBiasIndex = newChosenBiasIndex;
            ChangeButtonText();
        }
    }
}
