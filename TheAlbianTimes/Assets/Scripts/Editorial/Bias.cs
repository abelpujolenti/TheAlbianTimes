using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace Editorial
{
    public class Bias : MovableRectTransform
    {
        private const int SELECTED_COLOR_RED_VALUE = 255;
        private const int SELECTED_COLOR_GREEN_VALUE = 102;
        private const int SELECTED_COLOR_BLUE_VALUE = 0;

        [SerializeField] private TextMeshProUGUI _textMeshPro;
        
        [SerializeField] private String _text;

        [SerializeField] private Image _image;

        private Color _selectedColor;
        
        private bool _selected;

        private int _siblingIndex;

        new void Start()
        {
            base.Start();

            float red = MathUtil.Map(SELECTED_COLOR_RED_VALUE, 0, 255, 0, 1);
            float green = MathUtil.Map(SELECTED_COLOR_GREEN_VALUE, 0, 255, 0, 1);
            float blue = MathUtil.Map(SELECTED_COLOR_BLUE_VALUE, 0, 255, 0, 1);

            _selectedColor = new Color(red, green, blue);

            _textMeshPro.text = _text;

            _siblingIndex = transform.GetSiblingIndex();
            
            //DYNAMICALLY

            if (_siblingIndex != 0)
            {
                return;
            }

            _selected = true;
            _image.color = _selectedColor;
            ActionsManager.OnChangeSelectedBias += UnselectBias;

        }
        
        protected override void PointerClick(BaseEventData data)
        {
            if (_selected)
            {
                return;
            }

            ActionsManager.OnChangeSelectedBiasIndex(_siblingIndex);
            ActionsManager.OnChangeSelectedBias();
            _selected = true;
            _image.color = _selectedColor;
            ActionsManager.OnChangeSelectedBias += UnselectBias;
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
                ActionsManager.OnChangeSelectedBias += UnselectBias;
                return;
            }
            ActionsManager.OnChangeSelectedBias -= UnselectBias;
        }

        public void SetText(String newText)
        {
            _text = newText;
            _textMeshPro.text = _text;
        }
    }
}
