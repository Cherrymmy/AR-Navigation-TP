using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DetailMapRenderer : MonoBehaviour, IDirectionMapObserver
{
    // url 데이터
    [SerializeField] private string _apiKey;
    private string _url = string.Empty;
    public bool IsDestinationSet
    {
        get => _isDestinationSet;
        set => _isDestinationSet = value;
    }

    private bool _isDestinationSet;
    private float _desLat;
    private float _desLon;  
    private float _markerLat;
    private float _markerLon;
    private int _zoom = 18;
    private int _mapWidth;
    private int _mapHeight;

    // Data
    private GoogleMap _mapData;

    // Component
    private Rect _rect;


    private void OnEnable()
    {
        _mapData = GameObject.Find("GoogleMap").GetComponent<GoogleMap>();

        _mapData.ResisterDirectionMapObserver(this);
    }

    private void OnDisable()
    {
        _mapData.ResisterDirectionMapObserver(this);
    }

    void Start()
    {
        _rect = GetComponent<RawImage>().rectTransform.rect;
        _mapWidth = (int)Math.Round(_rect.width);
        _mapHeight = (int)Math.Round(_rect.height);

        StartCoroutine(GetGoogleStaticMap());
    }

    public void UpdateData(float lat, float lon, float deslat, float deslon, float draglat, float draglon, int zoom, Vector2 markerPos)
    {
        _desLat = deslat;
        _desLon = deslon;
        
        //if(_isDestinationSet)
        //{
            _markerLat = deslat;
            _markerLon = deslon;
        //}

        StartCoroutine(GetGoogleStaticMap());
    }

    IEnumerator GetGoogleStaticMap()
    {
        _rect = GetComponent<RawImage>().rectTransform.rect;
        _mapWidth = (int)Math.Round(_rect.width);
        _mapHeight = (int)Math.Round(_rect.height);

        _url = "https://maps.googleapis.com/maps/api/staticmap?center=" + _desLat + "," + _desLon +
                                                                      "&zoom=" + _zoom +
                                                                      "&size=" + _mapWidth + "x" + _mapHeight +
                                                                      "&scale=" + _mapData.MapResolution +
                                                                      "&markers=" + "color:" + GoogleMap.GoogleMapColor.purple + "|" + _markerLat + "," + _markerLon +
                                                                      "&maptype=" + _mapData.MapType +
                                                                      "&key=" + _apiKey;


        UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url);
        yield return www.SendWebRequest();

        // 실패했을 경우
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("www error" + www.error);
            Debug.LogError(_url);
        }
        else
        {
            GetComponent<RawImage>().texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }

        // 코루틴 종료
        yield break;
    }
}
