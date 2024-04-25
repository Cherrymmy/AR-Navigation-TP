using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;


///데이터를 로드, 저장, 수정하는 로직을 처리합니다.
namespace AR.Models
{
    public class PlacesModel : MonoBehaviour
    {
        public PlacesResponse PlacesData { get; private set; } // 파싱된 데이터 저장

        public DataManager datamanager;
        public UnityEvent<string> OnDataReceived;
        public UnityEvent OnDataParsed;  // 파싱된 데이터를 알리는 이벤트

        string apiKey = "AIzaSyCsyqqXiR26jn_xlk5UTmDdKdKqLoHyw1U";

        #region api 요청
        public void SearchPlaces(string query)
        {
            StartCoroutine(SearchPlacesCoroutine(query));
        }

        private IEnumerator SearchPlacesCoroutine(string query)
        {
            string fields = "name,place_id";
            string language = "ko";
            string url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={query}&language={language}&fields={fields}&key={apiKey}";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(webRequest.error);
                }
                else
                {
                    string response = webRequest.downloadHandler.text;
                    OnDataReceived.Invoke(response); // ??? 이거 데이터 수신 성공 이벤트 발생
                    DestoryData();
                    ParseData(response);
                }
            }
        }
        #endregion
        #region search data 저장
        private void ParseData(string jsonData)
        {
            PlacesData = JsonConvert.DeserializeObject<PlacesResponse>(jsonData);
            OnDataParsed.Invoke();  // 데이터 파싱 완료 이벤트 발생
        }

        private void DestoryData()
        {
            if (PlacesData != null && PlacesData.results != null)
            {
                PlacesData.results = new PlaceResult[0]; 
            }
        }

        public void SaveData(string name,string place_id)
        {
            datamanager.AddPlaceIdData(name, place_id);
        }

        private void LoadData(string name)
        {
            datamanager.RemovePlaceIdData(name);
        }
        #endregion
    }

    [System.Serializable]
    public class PlacesResponse
    {
        public PlaceResult[] results;
    }

    [System.Serializable]
    public class PlaceResult
    {
        public string name;
        public string place_id;
    }

    [System.Serializable]
    public class PlacesDatas
    {
        public List<PlaceResult> datas;
    }


}
