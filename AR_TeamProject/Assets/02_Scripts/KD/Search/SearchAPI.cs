using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// API요청
/// </summary>
/// 
namespace Search
{
    public class SearchAPI : MonoBehaviour, IJsonSearchService
    {
        public static SearchAPI Instance { get { return instance; } }
        public PlacesResponse jsonResponse;


        string apiKey = "AIzaSyCsyqqXiR26jn_xlk5UTmDdKdKqLoHyw1U";
        private static SearchAPI instance;
        private Create createInstance;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
            Debug.Log(Application.persistentDataPath);
        }

        private void Start()
        {
            createInstance = FindObjectOfType<Create>();

            if (DataManager.Instance != null)
            {
                DataManager.Instance.LoadPlacesDatas();
            }
            else
            {
                Debug.LogWarning("DataManager instance is not ready.");
            }
        }

        /// <summary>
        /// places api 통해서 정보 받아오기
        /// </summary>
        /// <param name="query"> 검색 text </param>
        /// <returns></returns>
        public IEnumerator SearchPlacesCoroutine(string query)
        {
            string fields = "name,place_id";
            string language = "ko";
            string url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={query}&language={language}&fields={fields}&key={apiKey}";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // API 요청을 보냅니다.
                yield return webRequest.SendWebRequest();

                // 네트워크 에러 또는 HTTP 에러 처리
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.LogError(webRequest.error);
                }
                else
                {
                    // 응답을 JSON으로 변환
                    string response = webRequest.downloadHandler.text;
                    jsonResponse = JsonUtility.FromJson<PlacesResponse>(response);

                    // 검색 결과를 바탕으로 UI 버튼 생성
                    foreach (var result in jsonResponse.results)
                    {
                        createInstance.CreateButton(result.name, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Place Details 값 받아오기
        /// </summary>
        /// <param name="url"> details 받아오는 주소 </param>
        /// <returns></returns>
        public IEnumerator FetchPlaceDetails(string url)
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


        /// <summary>
        /// 이름 확인후 Place Details에 place_id 넘기기
        /// </summary>
        /// <param name="name"> 가게이름 </param>
        public void Toss(string name)
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
                    StartCoroutine(FetchPlaceDetails(placeDetailsUrl));
                    break;
                }
            }
            // 검색 기록에 있으면 여기 실행함
            foreach (var data in DataManager.Instance.jsonDatas.datas)
            {
                if (data.Name == name)
                {
                    string fields = "name,photos,geometry,vicinity,editorial_summary";
                    string placeDetailsUrl = $"https://maps.googleapis.com/maps/api/place/details/json?placeid={data.PlaceId}&fields={fields}&key={apiKey}&language=ko";
                    StartCoroutine(FetchPlaceDetails(placeDetailsUrl));
                    return;
                }
            }
        }

        #region Place Details Json Data
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
        #endregion
        #region Place Text Search Json Data
        [System.Serializable]
        public class PlaceResult
        {
            public string name;
            public string place_id;
        }

        [System.Serializable]
        public class PlacesResponse
        {
            public PlaceResult[] results;
        }
        #endregion
    }
}
