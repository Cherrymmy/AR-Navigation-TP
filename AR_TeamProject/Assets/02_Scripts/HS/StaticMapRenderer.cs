using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StaticMapRenderer : MonoBehaviour, IStaticMapObserver
{
    // url 데이터
    [SerializeField] private string _apiKey;
    private string _url = string.Empty;
    private float _gpsLat;
    private float _gpsLon;
    private int _zoom;
    private int _mapWidth;
    private int _mapHeight;

    // Data
    private GoogleMap _mapData;

    // Component
    private Rect _rect;


    public void UpdateData(float lat, float lon, int zoom)
    {
        _gpsLat = lat;
        _gpsLon = lon;
        _zoom = zoom;
        //Debug.Log("UpdateData");
        StartCoroutine(GetGoogleStaticMap());
    }

    private void OnEnable() 
    {
        _mapData = GameObject.Find("GoogleMap").GetComponent<GoogleMap>();

        _mapData.ResisterStaticMapObserver(this);
    }

    private void OnDisable()
    {
        _mapData.RemoveStaticMapObserver(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        _rect = GetComponent<RawImage>().rectTransform.rect;
        _mapWidth = (int)Math.Round(_rect.width);
        _mapHeight = (int)Math.Round(_rect.height);

        StartCoroutine(GetGoogleStaticMap());
    }

    IEnumerator GetGoogleStaticMap()
    {
        _rect = GetComponent<RawImage>().rectTransform.rect;
        _mapWidth = (int)Math.Round(_rect.width);
        _mapHeight = (int)Math.Round(_rect.height);

        _url = "https://maps.googleapis.com/maps/api/staticmap?center=" + _gpsLat + "," + _gpsLon +
                                                                               "&zoom=" + _zoom +
                                                                               "&size=" + _mapWidth + "x" + _mapHeight +
                                                                               "&scale=" + _mapData.MapResolution +
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
