using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AR
{
    public class DataManager : MonoBehaviour
    {
        /// <summary>
        /// 지난 검색 목록 저장
        /// </summary>
        [System.Serializable]
        public class PlaceIdData
        {
            public string Name;
            public string PlaceId;
        }

        [System.Serializable]
        public class PlacesDatas
        {
            public List<PlaceIdData> datas;
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

        public PlacesDatas jsonDatas { get; private set; }


        public static DataManager Instance { get; private set; }
        // JSON 데이터를 담는 속성입니다.


        private void Awake()
        {
            // 인스턴스가 null이면 현재 객체를 인스턴스로 지정하고, 아니면 중복 인스턴스를 제거합니다.
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정합니다.
            }
            else
            {
                Destroy(gameObject); // 중복 인스턴스는 제거합니다.
            }
        }

        private void Start()
        {
            LoadPlacesDatas();
        }

        /// <summary>
        /// 제네릭 메서드로, 어떤 타입의 데이터든 JSON으로 변환하여 파일에 저장합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="jsonPath"></param>
        public void SaveData<T>(T data, string jsonPath)
        {
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(jsonPath, jsonData); // 파일 시스템에 JSON 문자열을 기록합니다.
            Debug.Log(jsonData);
        }

        /// <summary>
        /// 제네릭 메서드로, 지정된 경로에서 JSON 파일을 읽고 T 타입의 데이터로 변환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        public T LoadData<T>(string jsonPath)
        {
            if (File.Exists(jsonPath)) // 파일이 존재하는지 확인합니다.
            {
                string jsonData = File.ReadAllText(jsonPath); // 파일의 내용을 읽습니다.
                return JsonUtility.FromJson<T>(jsonData); // 읽은 데이터를 JSON에서 T 타입으로 변환합니다.
            }
            return default(T); // 파일이 없으면 T 타입의 기본값을 반환합니다.
        }

        /// <summary>
        /// 이름과 place_id를 받아 PlacesDatas에 추가하는 메서드
        /// 중복을 피하기 위해 이미 존재하는지 확인합니다.
        /// </summary>
        /// <param name="name"> 검색기록 </param>
        /// <param name="placeId"> 장소 id값 </param>
        public void AddPlaceIdData(string name, string placeId)
        {
            string jsonPath = Path.Combine(Application.persistentDataPath, "places_datas.json");
            PlacesDatas placesDatas = LoadData<PlacesDatas>(jsonPath);

            // placesDatas가 null이 아니고 datas 리스트도 null이 아닌지 확인합니다.
            if (placesDatas == null || placesDatas.datas == null)
            {
                placesDatas = new PlacesDatas { datas = new List<PlaceIdData>() };
            }

            // 중복된 이름이 있는지 검사합니다.
            if (!placesDatas.datas.Exists(p => p.Name == name))
            {
                // 중복되지 않은 경우 새 데이터를 리스트에 추가합니다.
                placesDatas.datas.Add(new PlaceIdData { Name = name, PlaceId = placeId });
                SaveData(placesDatas, jsonPath); // 변경된 데이터를 저장합니다.
            }

            Debug.Log(jsonPath);
        }

        /// <summary>
        /// 데이터 삭제 메서드
        /// </summary>
        /// <param name="name"></param>
        public void RemovePlaceIdData(string name)
        {
            string jsonPath = Path.Combine(Application.persistentDataPath, "places_datas.json");
            LoadPlacesDatas(); // 데이터를 먼저 로드

            PlaceIdData itemToRemove = jsonDatas.datas.Find(x => x.Name == name);
            if (itemToRemove != null)
            {
                jsonDatas.datas.Remove(itemToRemove); // 데이터 삭제
                SaveData(jsonDatas, jsonPath); // JSON 파일 업데이트
            }
        }

        /// <summary>
        /// 애플리케이션 데이터 폴더에서 "places_datas.json" 파일을 로드합니다.
        /// 파일이 없으면 빈 PlacesDatas를 생성합니다.
        /// </summary>
        /// 
        public void LoadPlacesDatas()
        {
            Debug.Log("이전기록 생성");
            string jsonPath = Path.Combine(Application.persistentDataPath, "places_datas.json");
            if (File.Exists(jsonPath))
            {
                string jsonData = File.ReadAllText(jsonPath);
                jsonDatas = JsonUtility.FromJson<PlacesDatas>(jsonData) ?? new PlacesDatas { datas = new List<PlaceIdData>() };
            }
            else
            {
                jsonDatas = new PlacesDatas { datas = new List<PlaceIdData>() };
            }
        }
    }

}
