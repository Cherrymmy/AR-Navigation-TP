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

        public UnityEvent OnDataParsed;                         // 파싱된 데이터를 알리는 이벤트
        public UnityEvent OnDataUpdated;                        // 저장되면

        string _apiKey = "AIzaSyCsyqqXiR26jn_xlk5UTmDdKdKqLoHyw1U";
        private DetailModel _detailModel;

        private void Start()
        {
            _detailModel = FindObjectOfType<DetailModel>();
        }

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
                    Debug.Log(response);
                    ParseData(response);
                    OnDataParsed.Invoke();                              // 서치 완료 (PlaceSearchController)
                }
            }
        }
        #endregion

        #region search data 저장
        private void ParseData(string jsonData)
        {
            DataManager.Instance.PlacesData = JsonConvert.DeserializeObject<PlacesResponse>(jsonData);
            Debug.Log(DataManager.Instance.PlacesData.results);
        }

        public void SaveData(string name, string place_id)
        {
            Instance.AddPlaceIdData(name, place_id);
        }

        public void LoadDataDelete(string name)
        {
            Instance.RemovePlaceIdData(name);
        }

        #endregion

        /// <summary>
        /// 신규 검색용
        /// </summary>
        /// <param name="name"></param>
        public void OnClickDetailView(string name)
        {
            foreach (var place in DataManager.Instance.PlacesData.results)
            {
                if (place.name == name)
                {
                    _detailModel.Toss(name);
                    SaveData(name, place.place_id);
                    OnDataUpdated.Invoke();
                    break;
                }
            }
        }

        /// <summary>
        /// 이전 검색 기록용
        /// </summary>
        /// <param name="name"></param>
        public void OnClickReDetailView(string name)
        {
            // 저장 기록 넘기기
            foreach (var place in DataManager.Instance.jsonDatas.datas)
            {
                if (place.Name == name)
                {
                    _detailModel.ReToss(place.Name);
                    break;
                }
            }
        }

        public void OnClickListDestroy(string name)
        {
            LoadDataDelete(name);
        }
    }
}
