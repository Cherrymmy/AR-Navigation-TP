using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GoogleMap : MonoBehaviour
{
    public string apiKey;       // 구글맵 api key
    public float lat = 0.0f;    // 위도
    public float lon = 0.0f;    // 경도

    public enum resolution { low = 0, high = 2 }    // 해상도
    public resolution mapResolution = resolution.low;

    public enum type { roadmap, satellite, hybrid, terrain }    // 맵 타입
    public type maptype = type.roadmap;

    private string url = string.Empty;
    private int mapWidth = 640;
    private int mapHeight = 640;
    //private bool mapIsLoading = false;
    private Rect rect;
    private string apiKeyLast;

    private float latLast = -33.85660f;
    private float lonLast = 151.21500f;

    public int Zoom { get => _zoom; set => _zoom = value; }
    private int _zoom = 14;
    public int ZoomLast { get => _zoomLast; }
    private int _zoomLast = 14;
    private resolution mapResolutionLast = resolution.low;
    private type mapTypeLast = type.roadmap;
    private bool updateMap = true;

    private GameObject GPSManager;

    // GPS
    private bool _isGPSWorked;
    private float _gpsLat;
    private float _gpsLon;

    // marker GPS
    private float _markerLat;
    private float _markerLon;

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

    // PolyLine
    public string directionsApiKey; // Directions API용 API 키
    private float _dragInitGPSLat;
    private float _dragInitGPSLon;

    // Destination
    private float _destinationLat;
    private float _destinationLon;
    //private string _destinationPos;

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

    private void Start()
    {
        GPSManager = GameObject.Find("GPSManager");
        _marker = transform.Find("Image - Marker").GetComponent<Image>();
        _markerInitPos = _marker.rectTransform.anchoredPosition;

        StartCoroutine(GetGoogleMap());
        rect = gameObject.GetComponent<RawImage>().rectTransform.rect;
        mapWidth = (int)Math.Round(rect.width);
        mapHeight = (int)Math.Round(rect.height);
    }

    private void FixedUpdate()
    {
        if (GPSManager != null && GPSManager.GetComponent<GPS>().receiveGPS)
        {
            if (_isGPSOn == true)
            {
                _gpsLat = GPSManager.GetComponent<GPS>().latitude;
                _gpsLon = GPSManager.GetComponent<GPS>().longitude;

                _markerLat = _gpsLat;
                _markerLon = _gpsLon;

                // gps가 초기화 되기 전에 막기 위한 변수
                // gpsLat, gpsLon이 0, 0 이면 true를 반환
                _isGPSWorked = (Mathf.Approximately(_gpsLat, 0f) && Mathf.Approximately(_gpsLon, 0f));
            }
        }
    }

    private void Update()
    {
        if (updateMap && (apiKeyLast != apiKey || Mathf.Approximately(latLast, /*lat*/_gpsLat) || Mathf.Approximately(lonLast, /*lon*/_gpsLon) ||
                        /*zoomLast != zoom ||*/ mapResolutionLast != mapResolution || mapTypeLast != maptype || !_isGPSWorked))
        {
            // zoom in & out
            ZoomInAndOut();

            Draging();

            //Debug.Log("Update map start");
            rect = gameObject.GetComponent<RawImage>().rectTransform.rect;
            mapWidth = (int)Math.Round(rect.width);
            mapHeight = (int)Math.Round(rect.height);

            // 목적지가 설정되어 있지 않다면 static map 그리기
            if (Mathf.Approximately(_destinationLat, 0f) && Mathf.Approximately(_destinationLon, 0f))
                StartCoroutine(GetGoogleMap());
            // 목적지가 설정되어 있다면 경로 그리기
            else
                StartCoroutine(GetDirections());

            updateMap = false;
        }
    }

    private IEnumerator GetGoogleMap(string path = "")
    {
        url = "https://maps.googleapis.com/maps/api/staticmap?center=" + /*lat*/_gpsLat + "," + /*lon */_gpsLon +
                                                                                "&zoom=" + _zoom +
                                                                                "&size=" + mapWidth +
                                                                                "x" + mapHeight +
                                                                                "&scale=" + mapResolution +
                                                                                "&markers=" + "color:" + GoogleMapColor.purple + "|" + _markerLat + "," + _markerLon +
                                                                                "&maptype=" + maptype +
                                                                                "&key=" + apiKey +
                                                                                path; // 도보 + 대중교통 경로 추가


        //mapIsLoading = true;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        // 실패했을 경우
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("www error" + www.error);
            Debug.LogError(url);
        }
        else
        {
            //mapIsLoading = false;
            gameObject.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            apiKeyLast = apiKey;
            //latLast = lat;
            //lonLast = lon;
            latLast = _gpsLat;
            lonLast = _gpsLon;
            _zoomLast = _zoom;
            mapResolutionLast = mapResolution;
            mapTypeLast = maptype;
            updateMap = true;
        }

        // 코루틴 종료
        yield break;
    }

    private IEnumerator GetDirections()
    {
        string directionsUrl = string.Empty;

        // 드래그를 시작하면 목적지가 검색된 시점의 gps를 기준으로 폴리라인을 그림
        if (!_isGPSOn)
        {
            directionsUrl = "https://maps.googleapis.com/maps/api/directions/json?origin=" + _dragInitGPSLat + "," + _dragInitGPSLon +
                                    "&destination=" + _destinationLat + "," + _destinationLon +
                                    "&region=KR" +
                                    "&mode=transit" +
                                    "&key=" + directionsApiKey;
        }
        // 드래그를 하지 않는다면 gps 위치에 따라서 폴리라인을 그림 
        else
        {
            directionsUrl = "https://maps.googleapis.com/maps/api/directions/json?origin=" + _gpsLat + "," + _gpsLon +
                                    "&destination=" + _destinationLat + "," + _destinationLon +
                                    "&region=KR" + 
                                    "&mode=transit" +
                                    "&key=" + directionsApiKey;
        }

        UnityWebRequest www = UnityWebRequest.Get(directionsUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Directions API Error: " + www.error);
        }
        else
        {
            //Debug.Log(www.downloadHandler.text); // 확인용

            JObject json = JObject.Parse(www.downloadHandler.text);
            JArray routes = (JArray)json["routes"];


            if (routes.Count > 0)
            {
                //string polyline = (string)routes[0]["overview_polyline"]["points"];

                JArray legs = (JArray)routes[0]["legs"];
                JArray steps = (JArray)legs[0]["steps"];

                List<string> walkPolyline = new List<string>();
                List<string> transitPolyline = new List<string>();

                for (int i = 0; i < steps.Count; i++)
                {
                    string modes = (string)steps[i]["travel_mode"];
                    if (modes.Equals("WALKING"))
                    {
                        walkPolyline.Add((string)steps[i]["polyline"]["points"]);
                    }
                    if (modes.Equals("TRANSIT"))
                    {
                        transitPolyline.Add((string)steps[i]["polyline"]["points"]);
                    }
                }

                string[] walkPath = walkPolyline.ToArray();
                string[] transitPath = transitPolyline.ToArray();

                string walkPaths = string.Empty;
                for (int i = 0; i < walkPath.Length; i++)
                {
                    walkPaths += "&path=color:0x8080807F|weight:5|enc:" + walkPath[i];
                }

                string transitPaths = string.Empty;
                for (int i = 0; i < transitPath.Length; i++)
                {
                    transitPaths += "&path=color:0x0000ff80|weight:5|enc:" + transitPath[i];
                }

                string path = walkPaths + transitPaths;

                //string path = "&path=enc:" + polyline;
                yield return GetGoogleMap(path); // 경로가 포함된 지도 이미지를 가져오기 위해 수정된 GetGoogleMap 코루틴 호출
            }
            else
            {
                Debug.LogError("No routes found. Draw Static Map");
                yield return GetGoogleMap();
            }
        }
    }

    private void ZoomInAndOut()
    {
        // zoom in & out
        if (Input.touchCount == 2)
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

            _dragSpeed /= Mathf.Pow(2f, zoomScale);

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
        if (Input.touchCount == 1 && !_isGPSButtonClick)
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

            // 좌우로 swipe 했다면
            if (Mathf.Abs(horiziontalTouchDelta * _dragSpeed) > 0f)
            {
                Vector2 newpos = new Vector2(-horiziontalTouchDelta, 0);
                _marker.rectTransform.anchoredPosition += newpos;
                _gpsLon = _gpsLon + horiziontalTouchDelta * _dragSpeed;
            }

            // 위아래로 swipe 했다면
            if (Mathf.Abs(verticalTouchDelta * _dragSpeed) > 0f)
            {
                Vector2 newpos = new Vector2(0, -verticalTouchDelta);
                _marker.rectTransform.anchoredPosition += newpos;
                _gpsLat = _gpsLat + verticalTouchDelta * _dragSpeed;
            }

            _dragStartPos = curTouchPos;
        }
        // 터치값이 1개가 아닐 때
        else
        {
            _isDraging = false;
        }
    }

    public void OnSetOnDestination()
    {
        // 목적지 야당역 설정
        _destinationLat = 37.71275f;
        _destinationLon = 126.7615f;

        _dragInitGPSLat = _gpsLat;
        _dragInitGPSLon = _gpsLon;

        // 마커 위치를 목적지에 설정
        //_markerLat = 37.71275f;
        //_markerLon = 126.7615f;
    }

    public void OnSetOffDestination()
    {
        // 목적지 초기화
        _destinationLat = 0f;
        _destinationLon = 0f;

        _dragInitGPSLat = 0f;
        _dragInitGPSLon = 0f;
    }

    public void OnSearchButton()
    {
        //_destinationPos = "&markers=" + "color:" + GoogleMapColor.purple + "|" + _markerLat + "," + _markerLon;
    }
}
