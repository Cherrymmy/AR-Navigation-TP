using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MiniMapRenderer : MonoBehaviour, IDirectionMapObserver
{
    // url 데이터
    [SerializeField] private string _apiKey;
    private string _url = string.Empty;
    private float _gpsLat;
    private float _gpsLon;
    private float _destinationLat;
    private float _destinationLon;
    private int _zoom = 14;
    private int _mapWidth;
    private int _mapHeight;

    // Data
    private GoogleMap _mapData;

    // Component
    private Rect _rect;


    public void UpdateData(float gpslat, float gpslon, float deslat, float deslon, float draglat, float draglon, int zoom)
    {
        _gpsLat = gpslat;
        _gpsLon = gpslon;
        _destinationLat = deslat;
        _destinationLon = deslon;
        Debug.Log("destination lat : " + _destinationLat + "destination lon : " + _destinationLon);

        StartCoroutine(GetDirectionsMap());
    }


    private void OnEnable() 
    {
        _mapData = GameObject.Find("GoogleMap").GetComponent<GoogleMap>();

        _mapData.ResisterDirectionMapObserver(this);
    }

    private void OnDisable()
    {
        _mapData.RemoveDirectionMapObserver(this);
    }

    void Start()
    {
        _rect = GetComponent<RawImage>().rectTransform.rect;
        _mapWidth = (int)Math.Round(_rect.width);
        _mapHeight = (int)Math.Round(_rect.height);

        StartCoroutine(GetGoogleStaticMap());
    }

    IEnumerator GetGoogleStaticMap(string path = "")
    {
        _rect = GetComponent<RawImage>().rectTransform.rect;
        _mapWidth = (int)Math.Round(_rect.width);
        _mapHeight = (int)Math.Round(_rect.height);

        _url = "https://maps.googleapis.com/maps/api/staticmap?center=" + _gpsLat + "," + _gpsLon +
                                                                               "&zoom=" + _zoom +
                                                                               "&size=" + _mapWidth + "x" + _mapHeight +
                                                                               "&scale=" + _mapData.MapResolution +
                                                                               "&maptype=" + _mapData.MapType +
                                                                               "&key=" + _apiKey +
                                                                               path;


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

    private IEnumerator GetDirectionsMap()
    {
        string directionsUrl = string.Empty;

        directionsUrl = "https://maps.googleapis.com/maps/api/directions/json?origin=" + _gpsLat + "," + _gpsLon +
                                                                                         "&destination=" + _destinationLat + "," + _destinationLon +
                                                                                         "&region=KR" +
                                                                                         "&mode=transit" +
                                                                                         "&key=" + _apiKey;

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
                yield return GetGoogleStaticMap(path); // 경로가 포함된 지도 이미지를 가져오기 위해 수정된 GetGoogleMap 코루틴 호출
            }
            else
            {
                Debug.LogError("No routes found. Draw Static Map");
                yield return GetGoogleStaticMap();
            }
        }
    }

}
