using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoldPlace : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

    private Vector3[] _corners = new Vector3[4];
    private Vector2 _containerMinCoordinates;
    private Vector2 _containerMaxCoordinates;
    private float _initialContainerMinCoordinateY;
    private float _initialContainerMaxCoordinateY;

    private void Start()
    {
        _rectTransform.GetWorldCorners(_corners);
        SetContainerLimiters();
    }

    private void SetContainerLimiters()
    {
        _containerMinCoordinates.x = _corners[0].x;
        _containerMinCoordinates.y = _corners[1].y;
        _containerMaxCoordinates.x = _corners[2].x;
        _containerMaxCoordinates.y = _corners[3].y;
        _initialContainerMinCoordinateY = _containerMinCoordinates.y;
        _initialContainerMaxCoordinateY = _containerMaxCoordinates.y;
    }

    private bool IsCoordinateInsideBounds(Vector2 coordinate)
    {
        return coordinate.x < _containerMaxCoordinates.x && coordinate.x > _containerMinCoordinates.x &&
               coordinate.y > _containerMaxCoordinates.y && coordinate.y < _containerMinCoordinates.y;
    }
}
