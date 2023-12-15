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
        [SerializeField] private Image _cap;
        private Animator _animator;

        private Coroutine _openCapPositionCoroutine;
        private Coroutine _openCapRotationCoroutine;
        private Coroutine _closeCapPositionCoroutine;
        private Coroutine _closeCapRotationCoroutine;

        [SerializeField] private float openCapOffset = 10f;

        private String _text;

        private Color _selectedColor;
        private Color _unselectedColor;
        
        private bool _selected;

        private int _siblingIndex;

        private Vector3 _capStartPosition;

        protected override void Setup()
        {
            base.Setup();
            SetupColor();
            _capStartPosition = _cap.transform.position;
        }

        void Start()
        {
            
            float red = MathUtil.Map(SELECTED_COLOR_RED_VALUE, 0, 255, 0, 1);
            float green = MathUtil.Map(SELECTED_COLOR_GREEN_VALUE, 0, 255, 0, 1);
            float blue = MathUtil.Map(SELECTED_COLOR_BLUE_VALUE, 0, 255, 0, 1);

            _textMeshPro.text = _text;

            _siblingIndex = transform.GetSiblingIndex();

            _animator = GetComponent<Animator>();

            if (_siblingIndex != 0)
            {
                return;
            }

            _selected = true;
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
            EventsManager.OnChangeSelectedBias += UnselectBias;

            SoundManager.Instance.ChangeBiasSound();
        }

        protected override void PointerEnter(BaseEventData data)
        {
            base.PointerEnter(data);
            OpenCap();
        }

        protected override void PointerExit(BaseEventData data)
        {
            base.PointerExit(data);
            CloseCap();
        }

        private void OpenCap()
        {
            if (_openCapPositionCoroutine != null) StopCoroutine(_openCapPositionCoroutine);
            _openCapPositionCoroutine = StartCoroutine(TransformUtility.SetPositionCoroutine(_cap.transform, _cap.transform.position.x, _capStartPosition.x - openCapOffset, 0.2f));
        
        }

        private void CloseCap()
        {
            if (_closeCapPositionCoroutine != null) StopCoroutine(_closeCapPositionCoroutine);
            _closeCapPositionCoroutine = StartCoroutine(TransformUtility.SetPositionCoroutine(_cap.transform, _cap.transform.position.x, _capStartPosition.x, 0.2f));
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
