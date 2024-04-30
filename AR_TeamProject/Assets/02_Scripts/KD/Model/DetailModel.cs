using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using static AR.DataManager;


namespace AR
{
    public class DetailModel : MonoBehaviour
    {
        string apiKey = "AIzaSyCsyqqXiR26jn_xlk5UTmDdKdKqLoHyw1U";
        public PlacesResponse jsonResponse;

        private PlacesSearchController _PSController;

        private void Start()
        {
            _PSController = FindAnyObjectByType<PlacesSearchController>();
            // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            _PSController.onDetailSearch.AddListener(Toss) ;
            // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        }

        public void Toss()
        {
            //Details();
        }
       
        private void Details(string name)
        {
            // search 값에 있으면 여기서 돌고
            foreach (var result in jsonResponse.results)
            {
                if (result.name == name)
                {
                    // 검색기록 저장하기
                    DataManager.Instance.AddPlaceIdData(name, result.place_id);
                    string fields = "name,photos,geometry,vicinity,editorial_summary";
                    string placeDetailsUrl = $"https://maps.googleapis.com/maps/api/place/details/json?placeid={result.place_id}&fields={fields}&key={apiKey}&language=ko";
                    // Place Detail 경도 위도 받기
                    StartCoroutine(PlaceDetails(placeDetailsUrl));
                    break;
                }
            }
            // 검색 기록에 있으면 여기 실행함 (DataMager)
            foreach (var data in Instance.jsonDatas.datas)
            {
                if (data.Name == name)
                {
                    string fields = "name,photos,geometry,vicinity,editorial_summary";
                    string placeDetailsUrl = $"https://maps.googleapis.com/maps/api/place/details/json?placeid={data.PlaceId}&fields={fields}&key={apiKey}&language=ko";
                    StartCoroutine(PlaceDetails(placeDetailsUrl));
                    return;
                }
            }
        }

        private IEnumerator PlaceDetails(string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("상세 정보를 가져오는 중 오류 발생: " + request.error);
                    // 사용자에게 알리거나 재시도를 고려할 수 있습니다.
                }
                else
                {
                    Debug.Log("응답: " + request.downloadHandler.text);
                    PlaceDetailsResponse details = JsonUtility.FromJson<PlaceDetailsResponse>(request.downloadHandler.text);

                    if (details.status == "OK")
                    {
                        Debug.Log("Status: " + details.status);
                        Debug.Log("Place Name: " + details.result.name);
                        Debug.Log("Location: Lat " + details.result.geometry.location.lat + ", Lng " + details.result.geometry.location.lng);
                        Debug.Log("Vicinity: " + details.result.vicinity);

                        foreach (Photo photo in details.result.photos)
                        {
                            Debug.Log("Photo Reference: " + photo.photo_reference);
                            Debug.Log("Photo Height: " + photo.height);
                            Debug.Log("Photo Width: " + photo.width);
                        }

                        // 필요한 경우 UI를 상세 정보로 업데이트할 수 있습니다.
                    }
                    else
                    {
                    }
                }
            }
        }


    }

    [System.Serializable]
    public class PlaceDetailsResponse
    {
        public Result result;
        public string status;
    }

    [System.Serializable]
    public class Result
    {
        public Geometry geometry;
        public string name;
        public Photo[] photos;
        public string vicinity;
    }

    [System.Serializable]
    public class Geometry
    {
        public Location location;
        public Viewport viewport;
    }

    [System.Serializable]
    public class Location
    {
        public float lat;
        public float lng;
    }

    [System.Serializable]
    public class Viewport
    {
        public Location northeast;
        public Location southwest;
    }

    [System.Serializable]
    public class Photo
    {
        public int height;
        public string photo_reference;
        public int width;
    }

}

