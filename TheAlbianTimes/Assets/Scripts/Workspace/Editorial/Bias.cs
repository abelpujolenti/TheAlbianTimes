using System;
using System.Collections;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;
using Random = UnityEngine.Random;

namespace Workspace.Editorial
{
    public class Bias : InteractableRectTransform
    {
        private const String OPEN_MARKER_CAP_SOUND = "Open Marker Cap";
        private const String CLOSE_MARKER_CAP_SOUND = "Close Marker Cap";
        private const String MARK_SOUND = "Mark With Marker";

        [SerializeField] private TextMeshProUGUI _textMeshPro;

        [SerializeField] private NewsFolder _newsFolder;
        [SerializeField] private Image _image;
        [SerializeField] private Image _cap;

        private BiasContainer _biasContainer;

        private Coroutine _openCapPositionCoroutine;
        private Coroutine _openCapRotationCoroutine;
        private Coroutine _closeCapPositionCoroutine;
        private Coroutine _closeCapRotationCoroutine;
        private Coroutine _markAnimationCoroutine;

        private bool _markAnimationRunning;

        [Header("Cap Animation")]
        [SerializeField] private Vector3 openCapOffset = new Vector3(-0.8f, 0f, 0f);
        [SerializeField] private float openCapTime = 0.15f;
        [SerializeField] private float closeCapTime = 0.23f;
        [SerializeField] private Vector3 separateCapOffset = new Vector3(-2f, 1f, 0f);
        [SerializeField] private float separateCapRotation = -22f;
        [SerializeField] private float separateCapTime = 0.25f;
        [Header("Marker Animation")]
        [SerializeField] private Vector3 markAnimationStartPos = new Vector3(1.4f, .1f, 90f);
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
        [SerializeField] private float resetBiasWiggleIntensity = 25f;
        [SerializeField] private float resetBiasWiggleTime = .5f;

        private String _text;
        
        [SerializeField]private bool _selected;

        private int _siblingIndex;

        private Vector3 _capStartPosition;
        private Vector3 _markerStartPosition;

        protected override void Setup()
        {
            base.Setup();
            _capStartPosition = _cap.transform.position;
            _markerStartPosition = _image.transform.position;
        }

        void Start()
        {
            _textMeshPro.text = _text;

            _siblingIndex = transform.GetSiblingIndex();

            if (_siblingIndex != 0)
            {
                return;
            }

            _selected = true;
            _cap.transform.position = _capStartPosition + openCapOffset;
            CloseCap();
        }

        private void OnEnable()
        {
            _image.transform.position = _markerStartPosition;
            _image.transform.rotation = Quaternion.identity;
            _cap.transform.position = _capStartPosition;
            _cap.transform.rotation = Quaternion.identity;
        }

        private void OnDestroy()
        {
            EventsManager.OnChangeChosenBias -= UnselectBias;
        }

        protected override void PointerClick(BaseEventData data)
        {
            if (_selected)
            {
                return;
            }

            EventsManager.OnChangeChosenBiasIndex(_siblingIndex);
            EventsManager.OnChangeChosenBias();
            _selected = true;
            
            EventsManager.OnChangeChosenBias += UnselectBias;

            _newsFolder.GetFrontHeadline().ClearBiasMarks();
            
            MarkAnimation();
        }

        protected override void PointerEnter(BaseEventData data)
        {
            base.PointerEnter(data);
            if (!_markAnimationRunning && !_selected)
            {
                OpenCap();
            }
        }

        protected override void PointerExit(BaseEventData data)
        {
            base.PointerExit(data);
            if (!_markAnimationRunning)
            {
                CloseCap();
            }
        }

        private void OpenCap()
        {
            if (_openCapPositionCoroutine != null) StopCoroutine(_openCapPositionCoroutine);
            if (_openCapRotationCoroutine != null) StopCoroutine(_openCapRotationCoroutine);
            _openCapPositionCoroutine = StartCoroutine(TransformUtility.SetPositionCoroutine(_cap.transform, _cap.transform.position, _capStartPosition + openCapOffset, openCapTime));
            AudioManager.Instance.Play3DSound(OPEN_MARKER_CAP_SOUND, 10, 100, transform.position);
        }

        private void CloseCap()
        {
            if (_cap.transform.position == _capStartPosition) return;
            if (_closeCapPositionCoroutine != null) StopCoroutine(_closeCapPositionCoroutine);
            if (_closeCapRotationCoroutine != null) StopCoroutine(_closeCapRotationCoroutine);
            _closeCapPositionCoroutine = StartCoroutine(TransformUtility.SetPositionCoroutine(_cap.transform, _cap.transform.position, _capStartPosition, closeCapTime));
            _closeCapRotationCoroutine = StartCoroutine(TransformUtility.SetRotationCoroutine(_cap.transform, 0f, closeCapTime));
            StartCoroutine(DelaySoundCoroutine(closeCapTime / 2f, CLOSE_MARKER_CAP_SOUND));
        }

        private void SeparateCap()
        {
            if (_openCapPositionCoroutine != null) StopCoroutine(_openCapPositionCoroutine);
            if (_openCapRotationCoroutine != null) StopCoroutine(_openCapRotationCoroutine);
            _openCapPositionCoroutine = StartCoroutine(TransformUtility.SetPositionCoroutine(_cap.transform, _cap.transform.position, _capStartPosition + separateCapOffset, closeCapTime));
            _openCapRotationCoroutine = StartCoroutine(TransformUtility.SetRotationCoroutine(_cap.transform, separateCapRotation, separateCapTime));
            AudioManager.Instance.Play3DSound(OPEN_MARKER_CAP_SOUND, 10, 100, transform.position);
        }

        public void MarkAnimation()
        {
            SeparateCap();

            markAnimationHeight = 0.12f * (_newsFolder.GetFrontHeadline().transform.Find("Text").GetComponent<TextMeshProUGUI>().textInfo.lineCount - 1);

            StopMarkAnimation();
            _markAnimationCoroutine = StartCoroutine(MarkAnimationCoroutine());
        }

        public void StopMarkAnimation()
        {
            if (_markAnimationCoroutine == null) return;
            StopCoroutine(_markAnimationCoroutine);
            _markAnimationRunning = false;
        }

        private IEnumerator DelaySoundCoroutine(float t, string audioSourceName)
        {
            yield return new WaitForSeconds(t);
            AudioManager.Instance.Play3DSound(audioSourceName, 10, 100, transform.position);
        }

        private IEnumerator MarkAnimationCoroutine()
        {
            _markAnimationRunning = true;
            yield return new WaitForSeconds(markAnimationStartDelay);
            StartCoroutine(TransformUtility.SetRotationCoroutine(_image.transform, 0f, markAnimationStartTime));
            yield return TransformUtility.SetPositionCoroutine(_image.transform, _image.transform.position, markAnimationStartPos, markAnimationStartTime);

            float y = 0f;
            while (y <= markAnimationHeight)
            {
                float inc = Random.Range(markAnimationPassMinHeight, markAnimationPassMaxHeight);
                float inc1 = inc * .1f;
                float inc2 = inc - inc1;
                y += inc;

                Vector3 passMovement = new Vector3(markAnimationWidth, -inc1, 0f);
                _newsFolder.GetFrontHeadline().SpawnBiasMark(_siblingIndex, _image.transform.position);

                AudioManager.Instance.Play3DSound(MARK_SOUND, 10, 100, transform.position);

                yield return TransformUtility.SetPositionCoroutine(_image.transform, _image.transform.position, _image.transform.position + passMovement, markAnimationPassTime);

                yield return new WaitForSeconds(markAnimationPassLingerTime);

                if (y <= markAnimationHeight)
                {
                    passMovement = new Vector3(-markAnimationWidth, -inc2, 0f);
                    yield return TransformUtility.SetPositionCoroutine(_image.transform, _image.transform.position, _image.transform.position + passMovement, markAnimationPassNewlineTime);
                }
            }

            yield return ReturnMarkerCoroutine();

            EventsManager.OnChangeNewsHeadlineContent();

            _markAnimationRunning = false;
        }

        private IEnumerator ReturnMarkerCoroutine()
        {
            yield return TransformUtility.SetPositionCoroutine(_image.transform, _image.transform.position, _markerStartPosition, markAnimationReturnTime);
            CloseCap();
        }

        public void Wiggle()
        {
            StartCoroutine(WiggleCoroutine(resetBiasWiggleIntensity, resetBiasWiggleTime));
        }

        private IEnumerator WiggleCoroutine(float intensity, float t)
        {
            _markAnimationRunning = true;

            float elapsedT = 0f;
            while (elapsedT <= t)
            {
                float p = (1 - Mathf.Pow(elapsedT / t, 2f)) * t * 100f;
                float angle = Mathf.Sin(p * t) * intensity * (1f - (elapsedT / t));
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            transform.rotation = Quaternion.identity;
            CloseCap();

            _markAnimationRunning = false;
        }

        public void SelectBias()
        {
            BiasButtonStuff(true);
            //_textMeshPro.fontStyle = FontStyles.Underline;
        }

        private void UnselectBias()
        {
            BiasButtonStuff(false);
            StopMarkAnimation();
            StartCoroutine(ReturnMarkerCoroutine());
        }

        public void ResetBiasUnderline()
        {
            _textMeshPro.fontStyle = FontStyles.Normal;
        }

        private void BiasButtonStuff(bool isSelected)
        {
            _selected = isSelected;
            if (isSelected)
            {
                EventsManager.OnChangeChosenBias += UnselectBias;
                return;
            }
            EventsManager.OnChangeChosenBias -= UnselectBias;
        }

        public void SetText(String newText)
        {
            _text = newText;
            _textMeshPro.text = _text;
        }

        public bool IsMarkAnimationRunning()
        {
            return _markAnimationRunning;
        }

        public bool IsSelected()
        {
            return _selected;
        }

        public void SetBiasContainer(BiasContainer biasContainer)
        {
            _biasContainer = biasContainer;
        }
    }
}