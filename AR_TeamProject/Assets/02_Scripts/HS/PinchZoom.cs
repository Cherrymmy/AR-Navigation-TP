using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zoom in & Zoom out on Map
/// </summary>
public class PinchZoom : MonoBehaviour
{
    private bool _isPinching;
    private int _minZoom = 8;
    private int _maxZoom = 20;
    private float _zoomSpeed = 0.005f;

    private Vector2 _prevTouchAPos;
    private Vector2 _prevTouchBPos;
    private GoogleMap _googleMap;

    void Start()
    {
        _googleMap = GetComponent<GoogleMap>();
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            if (!_isPinching)
            {
                _prevTouchAPos = Input.GetTouch(0).position;
                _prevTouchBPos = Input.GetTouch(1).position;

                _isPinching = true;
            }

            Vector2 curTouchAPos = Input.GetTouch(0).position;
            Vector2 curTouchBPos = Input.GetTouch(1).position;

            float prevTouchDeltaMag = (_prevTouchAPos - _prevTouchBPos).magnitude;
            float touchDeltaMag = (curTouchAPos - curTouchBPos).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            float newZoom = _googleMap.ZoomLast - deltaMagnitudeDiff * _zoomSpeed;
            int newIntZoom = Mathf.RoundToInt(newZoom);
            _googleMap.Zoom = Math.Clamp(newIntZoom, _minZoom, _maxZoom);

            int zoomScale = _googleMap.Zoom - _googleMap.ZoomLast;

            _googleMap.DragSpeed /= Mathf.Pow(2f, zoomScale);

            _prevTouchAPos = curTouchAPos;
            _prevTouchBPos = curTouchBPos;
        }
        else
        {
            _isPinching = false;
        }
    }
}
