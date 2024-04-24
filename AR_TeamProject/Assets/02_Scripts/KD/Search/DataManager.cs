using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Search
{
    public class DataManager : MonoBehaviour, IJsonDataManager
    {
        /// <summary>
        /// ���� �˻� ��� ����
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
        public PlacesDatas jsonDatas { get; private set; }


        public static DataManager Instance { get; private set; }
        // JSON �����͸� ��� �Ӽ��Դϴ�.


        private void Awake()
        {
            // �ν��Ͻ��� null�̸� ���� ��ü�� �ν��Ͻ��� �����ϰ�, �ƴϸ� �ߺ� �ν��Ͻ��� �����մϴ�.
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� �����մϴ�.
            }
            else
            {
                Destroy(gameObject); // �ߺ� �ν��Ͻ��� �����մϴ�.
            }
        }

        /// <summary>
        /// ���׸� �޼����, � Ÿ���� �����͵� JSON���� ��ȯ�Ͽ� ���Ͽ� �����մϴ�.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="jsonPath"></param>
        public void SaveData<T>(T data, string jsonPath)
        {
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(jsonPath, jsonData); // ���� �ý��ۿ� JSON ���ڿ��� ����մϴ�.
            Debug.Log(jsonData);
        }

        /// <summary>
        /// ���׸� �޼����, ������ ��ο��� JSON ������ �а� T Ÿ���� �����ͷ� ��ȯ�մϴ�.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        public T LoadData<T>(string jsonPath)
        {
            if (File.Exists(jsonPath)) // ������ �����ϴ��� Ȯ���մϴ�.
            {
                string jsonData = File.ReadAllText(jsonPath); // ������ ������ �н��ϴ�.
                return JsonUtility.FromJson<T>(jsonData); // ���� �����͸� JSON���� T Ÿ������ ��ȯ�մϴ�.
            }
            return default(T); // ������ ������ T Ÿ���� �⺻���� ��ȯ�մϴ�.
        }

        /// <summary>
        /// �̸��� place_id�� �޾� PlacesDatas�� �߰��ϴ� �޼���
        /// �ߺ��� ���ϱ� ���� �̹� �����ϴ��� Ȯ���մϴ�.
        /// </summary>
        /// <param name="name"> �˻���� </param>
        /// <param name="placeId"> ��� id�� </param>
        public void AddPlaceIdData(string name, string placeId)
        {
            string jsonPath = Path.Combine(Application.persistentDataPath, "places_datas.json");
            PlacesDatas placesDatas = LoadData<PlacesDatas>(jsonPath);

            // placesDatas�� null�� �ƴϰ� datas ����Ʈ�� null�� �ƴ��� Ȯ���մϴ�.
            if (placesDatas == null || placesDatas.datas == null)
            {
                placesDatas = new PlacesDatas { datas = new List<PlaceIdData>() };
            }

            // �ߺ��� �̸��� �ִ��� �˻��մϴ�.
            if (!placesDatas.datas.Exists(p => p.Name == name))
            {
                // �ߺ����� ���� ��� �� �����͸� ����Ʈ�� �߰��մϴ�.
                placesDatas.datas.Add(new PlaceIdData { Name = name, PlaceId = placeId });
                SaveData(placesDatas, jsonPath); // ����� �����͸� �����մϴ�.
            }
        }

        /// <summary>
        /// ������ ���� �޼���
        /// </summary>
        /// <param name="name"></param>
        public void RemovePlaceIdData(string name)
        {
            string jsonPath = Path.Combine(Application.persistentDataPath, "places_datas.json");
            LoadPlacesDatas(); // �����͸� ���� �ε�

            PlaceIdData itemToRemove = jsonDatas.datas.Find(x => x.Name == name);
            if (itemToRemove != null)
            {
                jsonDatas.datas.Remove(itemToRemove); // ������ ����
                SaveData(jsonDatas, jsonPath); // JSON ���� ������Ʈ
            }
        }

        /// <summary>
        /// ���ø����̼� ������ �������� "places_datas.json" ������ �ε��մϴ�.
        /// ������ ������ �� PlacesDatas�� �����մϴ�.
        /// </summary>
        /// 
        public void LoadPlacesDatas()
        {
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
