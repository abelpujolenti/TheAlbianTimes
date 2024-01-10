using System;
using System.Collections;
using Managers;
using NoMonoBehavior;
using UnityEngine;

namespace Workspace.Layout
{
    public class NewsHeadlinePiece : MonoBehaviour
    {
        private const String GRAB_PIECE_SOUND = "Grab Piece";
        private const String DROP_PIECE_IN_BOX_SOUND = "Drop Piece In Box";
        private const String SNAP_PIECE_SOUND = "Snap Piece";
        
        private const float TRANSPARENCY_VALUE = 0.9f;
        private const float FULL_OPACITY = 1;
        private const float SPEED_MOVEMENT = 15;
        private const float TIME_TO_SLIDE = 2f;

        [SerializeField] private NewsType _newsType;

        [SerializeField] private NewsHeadlineSubPiece[] _newsHeadlineSubPieces;

        private NewsHeadlinePiecesContainer _newsHeadlinePiecesContainer;

        private Cell[] _snappedCells;

        private Vector2[] _subPiecesPositionsRelativeToRoot;

        private Vector2 _containerMinCoordinates;
        private Vector2 _containerMaxCoordinates;
        private Vector2 _initialPosition;
        
        private Coroutine _setRotationCoroutine;

        private bool _rotate;
        private bool _transferDrag;

        private AudioSource _audioSourceGrabPiece;
        private AudioSource _audioSourceDropPieceInBox;
        private AudioSource _audioSourceSnapPiece;

        private void Start()
        {
            _audioSourceGrabPiece = gameObject.AddComponent<AudioSource>();
            _audioSourceDropPieceInBox = gameObject.AddComponent<AudioSource>();
            _audioSourceSnapPiece = gameObject.AddComponent<AudioSource>();
            (AudioSource, String)[] tuples =
            {
                (_audioSourceGrabPiece, GRAB_PIECE_SOUND),
                (_audioSourceDropPieceInBox, DROP_PIECE_IN_BOX_SOUND),
                (_audioSourceSnapPiece, SNAP_PIECE_SOUND)
            };
            SoundManager.Instance.SetMultipleAudioSourcesComponents(tuples);
            _newsHeadlinePiecesContainer = LayoutManager.Instance.GetNewsHeadlinePiecesContainer();
            _subPiecesPositionsRelativeToRoot = new Vector2[_newsHeadlineSubPieces.Length];
            gameObject.SetActive(false);
        }

        public void BeginDrag()
        {
            transform.SetAsLastSibling();

            _audioSourceGrabPiece.Play();

            EventsManager.OnCheckDistanceToMouse += DistanceToPosition;

            foreach (NewsHeadlineSubPiece newsHeadlineSubPiece in _newsHeadlineSubPieces)
            {
                newsHeadlineSubPiece.Fade(TRANSPARENCY_VALUE);
            }

            if (_snappedCells == null)
            {
                return;
            }

            foreach (Cell cell in _snappedCells)
            {
                cell.SetFree(true);
            }
        }

        public void EndDrag(NewsHeadlineSubPiece draggedSubPiece, Vector2 mousePosition)
        {
            EventsManager.OnCheckDistanceToMouse -= DistanceToPosition;
            
            foreach (NewsHeadlineSubPiece newsHeadlineSubPiece in _newsHeadlineSubPieces)
            {
                newsHeadlineSubPiece.Fade(FULL_OPACITY);
            }

            if (!gameObject.activeSelf)
            {
                return;
            }

            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            _snappedCells = EventsManager.OnPreparingCells(draggedSubPiece, mousePosition, _newsHeadlineSubPieces);

            if (_snappedCells == null)
            {
                _rotate = !_newsHeadlinePiecesContainer.IsValidPiecePosition(this);

                if (_rotate)
                {
                    EventsManager.OnPressPanicButtonForPieces += GoToContainer;
                }
                else
                {
                    EventsManager.OnPressPanicButtonForPieces -= GoToContainer;
                }

                _audioSourceDropPieceInBox.Play();
                return;
            }
            SlideToRotation(0f, 0.1f);
            transform.position = EventsManager.OnSuccessfulSnap(_snappedCells);
            _rotate = false;
            
            _audioSourceSnapPiece.Play();
        }

        private void GoToContainer()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            EventsManager.OnPressPanicButtonForPieces -= GoToContainer;
            _newsHeadlinePiecesContainer.PositionPieceOnRandomPosition(this);
        }

        public void SlideFromOriginToDestination()
        {
            SlideToRotation(0f, 0.1f);
            StartCoroutine(Slide(transform.position, _initialPosition));
        }

        private IEnumerator Slide(Vector2 origin, Vector2 destination)
        {
            float timer = 0;

            while (timer < TIME_TO_SLIDE)
            {
                timer = MoveToDestination(origin, destination, timer);

                yield return null;
            }
        }

        private float MoveToDestination(Vector2 origin, Vector2 destination, float timer)
        {
            timer += Time.deltaTime * SPEED_MOVEMENT;
            transform.position = Vector2.Lerp(origin, destination, timer / TIME_TO_SLIDE);
            return timer;
        }


        private void SlideToRotation(float rotation, float t)
        {
            if (_setRotationCoroutine != null)
            {
                StopCoroutine(_setRotationCoroutine);
            }
            _setRotationCoroutine = StartCoroutine(SetRotationCoroutine(rotation, t));
        }
        private IEnumerator SetRotationCoroutine(float zRotation, float t)
        {
            float elapsedT = 0f;
            Vector3 startRotation = transform.rotation.eulerAngles;
            while (elapsedT <= t)
            {
                float z = Mathf.LerpAngle(startRotation.z, zRotation, elapsedT / t);
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, z));
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, zRotation));
        }

        public Vector2[] GetSubPiecesPositionsRelativeToRoot()
        {
            return _subPiecesPositionsRelativeToRoot;
        }

        public NewsHeadlineSubPiece[] GetNewsHeadlinesSubPieces()
        {
            return _newsHeadlineSubPieces;
        }

        public void SetInitialPosition(Vector2 newInitialPosition)
        {
            _initialPosition = newInitialPosition;
        }
        
        public void SetTransferDrag(bool transferDrag)
        {
            _transferDrag = transferDrag;
        }

        public bool GetTransferDrag()
        {
            return _transferDrag;
        }

        public bool CanRotate()
        {
            return _rotate;
        }

        private Vector2 DistanceToPosition(Vector2 position)
        {
            return (Vector2)transform.position - position;
        }

        public void SetSubPieces(NewsHeadlineSubPiece[] subPieces)
        {
            _newsHeadlineSubPieces = subPieces;
        }
    }
}
