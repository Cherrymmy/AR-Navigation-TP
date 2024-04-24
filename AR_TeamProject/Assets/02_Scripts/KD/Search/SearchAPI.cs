using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// API��û
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
        /// places api ���ؼ� ���� �޾ƿ���
        /// </summary>
        /// <param name="query"> �˻� text </param>
        /// <returns></returns>
        public IEnumerator SearchPlacesCoroutine(string query)
        {
            string fields = "name,place_id";
            string language = "ko";
            string url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={query}&language={language}&fields={fields}&key={apiKey}";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // API ��û�� �����ϴ�.
                yield return webRequest.SendWebRequest();

                // ��Ʈ��ũ ���� �Ǵ� HTTP ���� ó��
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.LogError(webRequest.error);
                }
                else
                {
                    // ������ JSON���� ��ȯ
                    string response = webRequest.downloadHandler.text;
                    jsonResponse = JsonUtility.FromJson<PlacesResponse>(response);

                    // �˻� ����� �������� UI ��ư ����
                    foreach (var result in jsonResponse.results)
                    {
                        createInstance.CreateButton(result.name, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Place Details �� �޾ƿ���
        /// </summary>
        /// <param name="url"> details �޾ƿ��� �ּ� </param>
        /// <returns></returns>
        public IEnumerator FetchPlaceDetails(string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("�� ������ �������� �� ���� �߻�: " + request.error);
                    // ����ڿ��� �˸��ų� ��õ��� ����� �� �ֽ��ϴ�.
                }
                else
                {
                    Debug.Log("����: " + request.downloadHandler.text);
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

                        // �ʿ��� ��� UI�� �� ������ ������Ʈ�� �� �ֽ��ϴ�.
                    }
                    else
                    {
                    }
                }
            }
        }


        /// <summary>
        /// �̸� Ȯ���� Place Details�� place_id �ѱ��
        /// </summary>
        /// <param name="name"> �����̸� </param>
        public void Toss(string name)
        {
            // search ���� ������ ���⼭ ����
            foreach (var result in jsonResponse.results)
            {
                if (result.name == name)
                {
                    // �˻���� �����ϱ�
                    DataManager.Instance.AddPlaceIdData(name, result.place_id);
                    string fields = "name,photos,geometry,vicinity,editorial_summary";
                    string placeDetailsUrl = $"https://maps.googleapis.com/maps/api/place/details/json?placeid={result.place_id}&fields={fields}&key={apiKey}&language=ko";
                    // Place Detail �浵 ���� �ޱ�
                    StartCoroutine(FetchPlaceDetails(placeDetailsUrl));
                    break;
                }
            }
            // �˻� ��Ͽ� ������ ���� ������
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
