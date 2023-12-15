using System;
using System.Collections;
using Managers;
using TMPro;
using Unity.Mathematics;
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
        private Coroutine _markAnimationCoroutine;

        [Header("Cap Animation")]
        [SerializeField] private Vector3 openCapOffset = new Vector3(-0.8f, 0f, 0f);
        [SerializeField] private float openCapTime = 0.15f;
        [SerializeField] private float closeCapTime = 0.23f;
        [SerializeField] private Vector3 separateCapOffset = new Vector3(-2f, 1f, 0f);
        [SerializeField] private float separateCapRotation = -22f;
        [SerializeField] private float separateCapTime = 0.25f;
        [Header("Marker Animation")]
        [SerializeField] private Vector3 markAnimationStart = new Vector3(.4f, .5f, 90f);
        [SerializeField] private float markAnimationWidth = 1f;
        [SerializeField] private float markAnimationHeight = 2f;
        [SerializeField] private float markAnimationPassMinHeight = .15f;
        [SerializeField] private float markAnimationPassMaxHeight = .2f;
        [SerializeField] private float markAnimationStartDelay = .1f;
        [SerializeField] private float markAnimationStartTime = .5f;
        [SerializeField] private float markAnimationPassTime = .3f;
        [SerializeField] private float markAnimationPassNewlineTime = .5f;
        [SerializeField] private float markAnimationPassLingerTime = .3f;
        [SerializeField] private float markAnimationReturnTime = .3f;

        private String _text;

        private Color _selectedColor;
        private Color _unselectedColor;
        
        private bool _selected;

        private int _siblingIndex;

        private Vector3 _capStartPosition;
        private Vector3 _markerStartPosition;

        protected override void Setup()
        {
            base.Setup();
            SetupColor();
            _capStartPosition = _cap.transform.position;
            _markerStartPosition = _image.transform.position;
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
            MarkAnimation();
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
            if (_openCapRotationCoroutine != null) StopCoroutine(_openCapRotationCoroutine);
            _openCapPositionCoroutine = StartCoroutine(TransformUtility.SetPositionCoroutine(_cap.transform, _cap.transform.position, _capStartPosition + openCapOffset, openCapTime));
        
        }

        private void CloseCap()
        {
            if (_cap.transform.position == _capStartPosition) return;
            if (_closeCapPositionCoroutine != null) StopCoroutine(_closeCapPositionCoroutine);
            if (_closeCapRotationCoroutine != null) StopCoroutine(_closeCapRotationCoroutine);
            _closeCapPositionCoroutine = StartCoroutine(TransformUtility.SetPositionCoroutine(_cap.transform, _cap.transform.position, _capStartPosition, closeCapTime));
            _closeCapRotationCoroutine = StartCoroutine(TransformUtility.SetRotationCoroutine(_cap.transform, 0f, closeCapTime));
        }

        private void SeparateCap()
        {
            if (_openCapPositionCoroutine != null) StopCoroutine(_openCapPositionCoroutine);
            if (_openCapRotationCoroutine != null) StopCoroutine(_openCapRotationCoroutine);
            _openCapPositionCoroutine = StartCoroutine(TransformUtility.SetPositionCoroutine(_cap.transform, _cap.transform.position, _capStartPosition + separateCapOffset, closeCapTime));
            _openCapRotationCoroutine = StartCoroutine(TransformUtility.SetRotationCoroutine(_cap.transform, separateCapRotation, separateCapTime));
        }

        private void MarkAnimation()
        {
            SeparateCap();

            if (_markAnimationCoroutine != null) StopCoroutine(_markAnimationCoroutine);
            _markAnimationCoroutine = StartCoroutine(MarkAnimationCoroutine());
        }

        private IEnumerator MarkAnimationCoroutine()
        {
            yield return new WaitForSeconds(markAnimationStartDelay);
            yield return TransformUtility.SetPositionCoroutine(_image.transform, _image.transform.position, markAnimationStart, markAnimationStartTime);

            float y = 0f;
            while (y <= markAnimationHeight)
            {
                float inc = UnityEngine.Random.Range(markAnimationPassMinHeight, markAnimationPassMaxHeight);
                float inc1 = inc * .1f;
                float inc2 = inc - inc1;
                y += inc;

                Vector3 passMovement = new Vector3(markAnimationWidth, -inc1, 0f);
                yield return TransformUtility.SetPositionCoroutine(_image.transform, _image.transform.position, _image.transform.position + passMovement, markAnimationPassTime);

                yield return new WaitForSeconds(markAnimationPassLingerTime);

                if (y <= markAnimationHeight)
                {
                    passMovement = new Vector3(-markAnimationWidth, -inc2, 0f);
                    yield return TransformUtility.SetPositionCoroutine(_image.transform, _image.transform.position, _image.transform.position + passMovement, markAnimationPassNewlineTime);
                }
            }
            
            yield return TransformUtility.SetPositionCoroutine(_image.transform, _image.transform.position, _markerStartPosition, markAnimationReturnTime);
        }

        public void SelectBias()
        {
            BiasButtonStuff(true, _selectedColor);
        }

        private void UnselectBias()
        {
            BiasButtonStuff(false, _unselectedColor);
            CloseCap();
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
