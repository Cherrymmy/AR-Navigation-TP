using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get { return _instance; } }
    public GameObject[] uiPrefab; // UI 프리팹
    public Transform uiParent; // UI가 부착될 캔버스
    public int poolSize = 15;

    private Queue<GameObject> _searchListPool = new Queue<GameObject>();
    private Queue<GameObject> _reSearchListPool = new Queue<GameObject>();
    private static ObjectPool _instance;


    private void Awake()
    {
        if(_instance == null)
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
            GameObject searchList = Instantiate(uiPrefab[0], uiParent.transform);
            searchList.SetActive(false);
            _searchListPool.Enqueue(searchList);
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject reSearchList = Instantiate(uiPrefab[1], uiParent.transform);
            reSearchList.SetActive(false);
            _reSearchListPool.Enqueue(reSearchList);

        }
    }

    /// <summary>
    /// 검색 기록
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetSearchListElement(string name)
    {
        if (_searchListPool.Count > 0)
        {
            GameObject uiObject = _searchListPool.Dequeue();
            TMP_Text listname = uiObject.GetComponentInChildren<TMP_Text>();
            listname.text = name;
            uiObject.SetActive(true);
            return uiObject;
        }
        return null; // 풀이 비어있으면 null 반환
    }
    /// <summary>
    /// 이전 검색 기록
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetReSearchListElement(string name)
    {
        if (_reSearchListPool.Count > 0)
        {
            GameObject uiObject = _reSearchListPool.Dequeue();
            TMP_Text listname = uiObject.GetComponentInChildren<TMP_Text>();
            listname.text = name;
            uiObject.SetActive(true);
            return uiObject;
        }
        return null; // 풀이 비어있으면 null 반환
    }

    public void ReturnSearchListElement(GameObject uiObject)
    {
        uiObject.SetActive(false);
        _searchListPool.Enqueue(uiObject);
    }

    public void ReturnReSearchListElement(GameObject uiObject)
    {
        uiObject.SetActive(false);
        _reSearchListPool.Enqueue(uiObject);
    }
}
