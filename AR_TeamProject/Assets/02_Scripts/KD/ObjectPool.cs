using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace AR
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get { return _instance; } }
        [Header("오브젝트 풀")]
        [Tooltip("생성될 UI프리팹")]
        public GameObject[] uiPrefab; // UI 프리팹
        [Tooltip("생성될 곳")]
        public Transform uiParent; // UI가 부착될 캔버스
        [Tooltip("처음에 만들 갯수")]
        public int poolSize = 15;

        private List<GameObject> _searchListPool = new List<GameObject>();
        public List<GameObject> _reSearchListPool = new List<GameObject>();
        private static ObjectPool _instance;


        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject reSearchList = Instantiate(uiPrefab[1], uiParent.transform);
                reSearchList.SetActive(false);
                _reSearchListPool.Add(reSearchList);
                Debug.Log(_reSearchListPool.Count);

            }

            for (int i = 0; i < poolSize; i++)
            {
                GameObject searchList = Instantiate(uiPrefab[0], uiParent.transform);
                searchList.SetActive(false);
                _searchListPool.Add(searchList);
            }
        }

        /// <summary>
        /// 검색 기록
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject GetSearchListElement(string name)
        {
            foreach (GameObject obj in _searchListPool)
            {
                if (!obj.activeInHierarchy)
                {
                    TMP_Text listname = obj.GetComponentInChildren<TMP_Text>();
                    listname.text = name;
                    obj.SetActive(true);
                    return obj;
                }
            }
            return null; // All items are in use
        }
        /// <summary>
        /// 이전 검색 기록
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject GetReSearchListElement(string name)
        {
            foreach (GameObject obj in _reSearchListPool)
            {
                if (!obj.activeInHierarchy)
                {
                    TMP_Text listname = obj.GetComponentInChildren<TMP_Text>();
                    listname.text = name;
                    obj.SetActive(true);
                    return obj;
                }
            }
            return null; // All items are in use
        }

        public void ReturnSearchListElement(GameObject uiElement)
        {
            uiElement.SetActive(false);
        }

        public void ReturnReSearchListElement(GameObject uiElement)
        {
            uiElement.SetActive(false);
        }

        public void ClearSearchResults()
        {
            foreach (var uiElement in _searchListPool)
            {
                if (uiElement.activeInHierarchy)
                {
                    ReturnSearchListElement(uiElement);
                }
            }
        }

        public void ClearReSearchResults()
        {
            foreach (var uiElement in _reSearchListPool)
            {
                if (uiElement.activeInHierarchy)
                {
                    ReturnReSearchListElement(uiElement);
                }
            }
        }
    }
}
