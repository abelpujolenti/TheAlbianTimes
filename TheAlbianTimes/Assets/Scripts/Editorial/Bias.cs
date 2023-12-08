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
        private Color _unselectedColor;
        
        private bool _selected;

        private int _siblingIndex;

        protected override void Setup()
        {
            base.Setup();
            SetupColor();
        }

        void Start()
        {
            
            float red = MathUtil.Map(SELECTED_COLOR_RED_VALUE, 0, 255, 0, 1);
            float green = MathUtil.Map(SELECTED_COLOR_GREEN_VALUE, 0, 255, 0, 1);
            float blue = MathUtil.Map(SELECTED_COLOR_BLUE_VALUE, 0, 255, 0, 1);

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

        private void OnDestroy()
        {
            //This is cringe but event wasnt getting unsubscribed
            EventsManager.OnChangeSelectedBias -= UnselectBias;
        }

        public void SetupColor()
        {
            _unselectedColor = _image.color;
            Color hsv = new Color();
            Color.RGBToHSV(_image.color, out hsv.r, out hsv.g, out hsv.b);
            hsv.r = ((hsv.r * 255f + 128f) % 255) / 255f;
            hsv.b *= .8f;
            _selectedColor = Color.HSVToRGB(hsv.r, hsv.g, hsv.b);
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

            SoundManager.Instance.ChangeBiasSound();
        }

        public void SelectBias()
        {
            BiasButtonStuff(true, _selectedColor);
        }

        private void UnselectBias()
        {
            BiasButtonStuff(false, _unselectedColor);
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
