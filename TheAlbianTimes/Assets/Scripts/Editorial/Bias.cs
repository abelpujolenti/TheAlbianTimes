using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace Editorial
{
    public class Bias : InteractableRectTransform
    {
        private const int SELECTED_COLOR_RED_VALUE = 255;
        private const int SELECTED_COLOR_GREEN_VALUE = 102;
        private const int SELECTED_COLOR_BLUE_VALUE = 0;

        [SerializeField] private TextMeshProUGUI _textMeshPro;

        [SerializeField] private Image _image;
        
        private String _text;

        private Color _selectedColor;
        
        private bool _selected;

        private int _siblingIndex;

        void Start()
        {

            float red = MathUtil.Map(SELECTED_COLOR_RED_VALUE, 0, 255, 0, 1);
            float green = MathUtil.Map(SELECTED_COLOR_GREEN_VALUE, 0, 255, 0, 1);
            float blue = MathUtil.Map(SELECTED_COLOR_BLUE_VALUE, 0, 255, 0, 1);

            _selectedColor = new Color(red, green, blue);

            _textMeshPro.text = _text;

            _siblingIndex = transform.GetSiblingIndex();

            if (_siblingIndex != 0)
            {
                return;
            }

            _selected = true;
            _image.color = _selectedColor;
            EventsManager.OnChangeSelectedBias += UnselectBias;

        }
        
        protected override void PointerClick(BaseEventData data)
        {
            if (_selected)
            {
                return;
            }

            EventsManager.OnChangeSelectedBiasIndex(_siblingIndex);
            EventsManager.OnChangeSelectedBias();
            _selected = true;
            _image.color = _selectedColor;
            EventsManager.OnChangeSelectedBias += UnselectBias;
        }

        public void SelectBias()
        {
            BiasButtonStuff(true, _selectedColor);
        }

        private void UnselectBias()
        {
            BiasButtonStuff(false, Color.white);
        }

        private void BiasButtonStuff(bool isSelected, Color newColor)
        {
            _selected = isSelected;
            _image.color = newColor;
            if (isSelected)
            {
                EventsManager.OnChangeSelectedBias += UnselectBias;
                return;
            }
            EventsManager.OnChangeSelectedBias -= UnselectBias;
        }

        public void SetText(String newText)
        {
            _text = newText;
            _textMeshPro.text = _text;
        }
    }
}
