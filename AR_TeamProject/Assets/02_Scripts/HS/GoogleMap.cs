using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GoogleMap : MonoBehaviour, ISubject
{
    private float _latLast = -33.85660f;
    private float _lonLast = 151.21500f;

    public enum resolution { low = 0, high = 2 } // 해상도
    public resolution MapResolution { get => _mapResolution; }
    private resolution _mapResolution = resolution.low;
    private resolution _mapResolutionLast = resolution.low;

    public enum type { roadmap, satellite, hybrid, terrain } // 맵 타입
    public type MapType { get => _maptype; }
    private type _maptype = type.roadmap;
    private type _mapTypeLast = type.roadmap;

    public int Zoom 
    { 
        get => _zoom; 
        set => _zoom = value; 
    }

    private int _zoom = 14;

    public int ZoomLast { get => _zoomLast; }
    private int _zoomLast = 14;

    public bool UpdateMap
    {
        get => _updateMap;
        set => _updateMap = value;
    }
    private bool _updateMap = true;

    private GameObject GPSManager;

    // GPS
    private bool _isGPSWorked;
    public float GpsLat { get => _gpsLat; }
    public float GpsLon { get => _gpsLon; }

    private float _gpsLat;
    private float _gpsLon;

    // Zoom & Dragging
    public bool IsGPSOn
    {
        get => _isGPSOn;
        set => _isGPSOn = value;
    }

    public bool IsGPSButtonClick
    {
        get => _isGPSButtonClick;
        set => _isGPSButtonClick = value;
    }

    private bool _isGPSOn = true;
    private bool _isPinching;
    public bool IsDraging
    {
        get => _isDraging;
    }

    private bool _isDraging;
    private bool _isGPSButtonClick;
    
    private float _zoomSpeed = 0.005f;
    private int _minZoom = 8;
    private int _maxZoom = 20;
    public float DragSpeed 
    { 
        get => _dragSpeed; 
        set => _dragSpeed = value; 
    }

    [SerializeField] private float _dragSpeed = 0.000085f;

    private Vector2 _prevTouch1Pos;
    private Vector2 _prevTouch2Pos;
    private Vector2 _dragStartPos;

    // Marker
    private Vector2 _markerPos;

    public Vector2 markerPosition
    {
        get => _marker.rectTransform.anchoredPosition;
        set => _marker.rectTransform.anchoredPosition = value;
    }

    public Vector2 markerInitPosition
    {
        get => _markerInitPos;
    }

    private Image _marker;
    private Vector2 _markerInitPos;

    // Drag
    public float DragInitGPSLat
    {
        get => _dragInitGPSLat;
        set => _dragInitGPSLat = value;
    }

    public float DragInitGPSLon
    {
        get => _dragInitGPSLon;
        set => _dragInitGPSLon = value;
    }

    private float _dragInitGPSLat;
    private float _dragInitGPSLon;

    public bool IsDragZoomDisable
    {
        get => _isDragZoomDisable;
        set => _isDragZoomDisable = value;
    }

    private bool _isDragZoomDisable;

    // Destination
    public float DestinationLat
    {
        get => _destinationLat;
        set => _destinationLat = value;
    }

    public float DestinationLon
    {
        get => _destinationLon;
        set => _destinationLon = value;
    }

    private float _destinationLat;
    private float _destinationLon;

    // Canvas
    private Canvas _staticMapCanvas;

    // Observer Pattern
    private List<IStaticMapObserver> _staticMapObserver = new List<IStaticMapObserver>();
    private List<IDirectionMapObserver> _directionMapObserver = new List<IDirectionMapObserver>();

    public enum GoogleMapColor
    {
        black,
        brown,
        green,
        purple,
        yellow,
        blue,
        gray,
        orange,
        red,
        white
    }   

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        GPSManager = GameObject.Find("GPSManager");
        _staticMapCanvas = GameObject.Find("Canvas - StaticMap").GetComponent<Canvas>();
        _markerInitPos = _staticMapCanvas.GetComponentInChildren<StaticMapRenderer>().markerInitPosition;
        StartCoroutine(C_UpdateData());
    }

    private void FixedUpdate()
    {
        if (GPSManager != null && GPSManager.GetComponent<GPS>().receiveGPS)
        {
            if (_isGPSOn == true)
            {
                _gpsLat = GPSManager.GetComponent<GPS>().latitude;
                _gpsLon = GPSManager.GetComponent<GPS>().longitude;

                // gps가 초기화 되기 전에 막기 위한 변수 -> gpsLat, gpsLon이 0, 0 이면 true를 반환
                _isGPSWorked = (Mathf.Approximately(_gpsLat, 0f) && Mathf.Approximately(_gpsLon, 0f));
            }
        }
    }

    private void Update()
    {
        if (_updateMap && (Mathf.Approximately(_latLast, _gpsLat) || Mathf.Approximately(_lonLast, _gpsLon) ||
                        _mapResolutionLast != _mapResolution || _mapTypeLast != _maptype || !_isGPSWorked))
        {
            // zoom in & out
            ZoomInAndOut();

            Draging();
        }
    }

    private void ZoomInAndOut()
    {
        // zoom in & out
        if (Input.touchCount == 2 && !_isDragZoomDisable)
        {
            if (!_isPinching)
            {
                _prevTouch1Pos = Input.GetTouch(0).position;
                _prevTouch2Pos = Input.GetTouch(1).position;

                _isPinching = true;
            }

            Vector2 curTouch1Pos = Input.GetTouch(0).position;
            Vector2 curTouch2Pos = Input.GetTouch(1).position;

            float prevTouchDeltaMag = (_prevTouch1Pos - _prevTouch2Pos).magnitude;
            float touchDeltaMag = (curTouch1Pos - curTouch2Pos).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            float newZoom = _zoomLast - deltaMagnitudeDiff * _zoomSpeed;
            int newIntZoom = Mathf.RoundToInt(newZoom);

            _zoom = Math.Clamp(newIntZoom, _minZoom, _maxZoom);

            int zoomScale = _zoom - _zoomLast;

            // Drag Speed 보정
            // 줌이 커지면 Drag Speed 값이 2배로 작아지고 줌이 작아지면 Drag Speed 값이 2배로 커짐
            //_dragSpeed /= Mathf.Pow(2f, zoomScale);

            _prevTouch1Pos = curTouch1Pos;
            _prevTouch2Pos = curTouch2Pos;
        }
        else
        {
            _isPinching = false;
        }
    }

    private void Draging()
    {
        // 입력한 터치값이 1개 일 때
        if (Input.touchCount == 1 && !_isGPSButtonClick && !_isDragZoomDisable)
        {
            if (!_isDraging)
            {
                _dragStartPos = Input.GetTouch(0).position;

                _isDraging = true;
                _isGPSOn = false;
            }

            Debug.Log("Zoom : " + _zoom);
            Debug.Log("DragSpeed : " + _dragSpeed);

            Vector2 curTouchPos = Input.GetTouch(0).position;

            float horiziontalTouchDelta = (_dragStartPos.x - curTouchPos.x);
            float verticalTouchDelta = (_dragStartPos.y - curTouchPos.y);

            switch (_zoom)
            {
                case 14:
                    _dragSpeed = 0.000085f;
                    break;
                case 15:
                    _dragSpeed = 0.000085f / 2f;
                    break;
                case 16:
                    _dragSpeed = 0.000085f / 4f;
                    break;
                case 17:
                    _dragSpeed = 0.000085f / 8f;
                    break;
                case 18:
                    _dragSpeed = 0.000085f / 16f;
                    break;
                case 19:
                    _dragSpeed = 0.000085f / 32f;
                    break;
                case 20:
                    _dragSpeed = 0.000085f / 64f;
                    break;
                default:
                    break;
            }

            // 좌우로 swipe 했다면
            if (Mathf.Abs(horiziontalTouchDelta * _dragSpeed) > 0f)
            {
                Vector2 newpos = new Vector2(-horiziontalTouchDelta, 0);
                _markerPos.x = newpos.x;
                _gpsLon = _gpsLon + horiziontalTouchDelta * _dragSpeed;
                _dragInitGPSLon = _gpsLon;
            }

            // 위아래로 swipe 했다면
            if (Mathf.Abs(verticalTouchDelta * _dragSpeed) > 0f)
            {
                Vector2 newpos = new Vector2(0, -verticalTouchDelta);
                _markerPos.y = newpos.y;
                _gpsLat = _gpsLat + verticalTouchDelta * _dragSpeed;
                _dragInitGPSLat = _gpsLat;
            }

            _dragStartPos = curTouchPos;
        }
        // 터치값이 1개가 아닐 때
        else
        {
            _isDraging = false;
        }
    }

    IEnumerator C_UpdateData()
    {
        while(true)
        {
            // 목적지가 있을 때만 필요한 데이터들을 갱신하라고 지시
            if (!(Mathf.Approximately(_destinationLat, 0f) && Mathf.Approximately(_destinationLon, 0f)))
            {
                UpdateDirectionMap();
            }

            UpdateStaticMap();

            _latLast = _gpsLat;
            _lonLast = _gpsLon;
            _zoomLast = _zoom;
            _updateMap = false;

            yield return new WaitForSeconds(5f);
            //yield return new WaitForSeconds(0.016f);
        }

    }

    #region 옵저버패턴
    /// <summary>
    /// staticMap/DirectionMap Resister
    /// </summary>
    /// <param name="observer"></param>
    public void ResisterStaticMapObserver(IStaticMapObserver observer)
    {
        _staticMapObserver.Add(observer);
    }

    public void ResisterDirectionMapObserver(IDirectionMapObserver observer)
    {
        _directionMapObserver.Add(observer);
    }

    public void RemoveStaticMapObserver(IStaticMapObserver observer)
    {
        _staticMapObserver.Remove(observer);
        //_directionMapObserver

}

public void RemoveDirectionMapObserver(IDirectionMapObserver observer)
    {
        _directionMapObserver.Remove(observer);
    }

    public void NotifyStaticMapObservers()
    {
        foreach(IStaticMapObserver observer in _staticMapObserver)
        {
            observer.UpdateData(_gpsLat, _gpsLon, _zoom, _markerPos);
        }
    }

    public void NotifyDirectionMapObservers()
    {
        //Debug.Log("_directionMapObserver " + _directionMapObserver.Count);
        foreach(IDirectionMapObserver observer in _directionMapObserver)
        {
            observer.UpdateData(_gpsLat, _gpsLon, _destinationLat, _destinationLon, _dragInitGPSLat, _dragInitGPSLon, _zoom, _markerPos);
        }
    }

    private void UpdateStaticMap()
    {
        // Update StaticMap Data
        NotifyStaticMapObservers();
    }

    private void UpdateDirectionMap()
    {
        // Update DirectionMap Data
        NotifyDirectionMapObservers();
    }
    #endregion
}
