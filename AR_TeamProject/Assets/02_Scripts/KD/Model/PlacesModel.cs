using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.Events;
using static AR.DataManager;


///데이터를 로드, 저장, 수정하는 로직을 처리합니다.
namespace AR.Models
{
    public class PlacesModel : MonoBehaviour
    {
        public PlacesResponse PlacesData { get; private set ; } // 파싱된 데이터 저장
        public PlacesDatas jsonDatas { get; private set; }

        public UnityEvent OnDataParsed;                         // 파싱된 데이터를 알리는 이벤트
        string _apiKey = "AIzaSyCsyqqXiR26jn_xlk5UTmDdKdKqLoHyw1U";
        

        #region api 요청
        public void SearchPlaces(string query)
        {
            StartCoroutine(SearchPlacesCoroutine(query));
        }

        private IEnumerator SearchPlacesCoroutine(string query)
        {
            string fields = "name,place_id";
            string language = "ko";
            string url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={query}&language={language}&fields={fields}&key={_apiKey}";

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
                    ParseData(response);
                    OnDataParsed.Invoke();                              // 서치 완료 (PlaceSearchController)
                }
            }
        }
        #endregion

        #region search data 저장
        private void ParseData(string jsonData)
        {
            PlacesData = JsonConvert.DeserializeObject<PlacesResponse>(jsonData);
        }

        public void SaveData(string name,string place_id)
        {
            Instance.AddPlaceIdData(name, place_id);
        }

        public void LoadDataDelete(string name)
        {
            Instance.RemovePlaceIdData(name);
        }
        #endregion
    }
}
