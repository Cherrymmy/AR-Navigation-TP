using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// _menuScreens의 배열순서 맞추기
    /// 스테틱맵0, 검색1, 상세2, 내비3, AR내비4
    /// </summary>
    public enum MenuType
    {
        StaticMap_Canvas,
        Search_Canvas,
        Detail_Canvas,
        NaviMap_Canvas,
        ARMap_Canvas
    }

    public static UIManager Instance { get; private set; }

    [SerializeField]
    private IUimenu[] _menuScreens;

    public IUimenu[] MenuScreens
    {
        get { return _menuScreens; }
        set { _menuScreens = value; }
    }

    public MenuType StartMenu;
    public GameObject lodingPopUpSceen;

    private bool _loadingSet = true;

    /// <summary>
    /// 로딩창 온 오프 
    /// </summary>
    public bool LoadingSet
    {
        set { _loadingSet = value; }
    }

    // main canvas 위치
    private Transform _transform;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 싱글톤 인스턴스를 다른 씬으로 넘어가도 파괴되지 않게 합니다.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _transform = gameObject.transform;

        // 초기 시작 켄버스 선택
        _menuScreens[(int)StartMenu].transform.position = _transform.position;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentMenu"> 현재 메뉴 </param>
    /// <param name="targetMenu"> 바뀔 메뉴 </param>
    public void Switch(MenuType currentMenu, MenuType targetMenu)
    {
        _loadingSet = true;
        int currentIndex = (int)currentMenu;
        int targetIndex = (int)targetMenu;
        StartCoroutine(LodingPopUp(currentIndex, targetIndex));
    }

    private void SwapMenuScreens(int indexA, int indexB)
    {
        // 인덱스가 배열 범위 내에 있는지 확인합니다.
        if (indexA < 0 || indexA >= _menuScreens.Length || indexB < 0 || indexB >= _menuScreens.Length)
        {
            Debug.LogError("인덱스가 배열 범위를 벗어났습니다.");
            return;
        }

        // 두 오브젝트의 현재 위치를 저장합니다.
        Vector3 positionA = _menuScreens[indexA].transform.position;
        Vector3 positionB = _menuScreens[indexB].transform.position;

        // 위치를 교환합니다.
        _menuScreens[indexA].transform.position = positionB;
        _menuScreens[indexB].transform.position = positionA;
    }

    /// <summary>
    /// 로딩 팝업
    /// </summary>
    /// <returns></returns>
    private IEnumerator LodingPopUp(int currentIndex, int targetIndex)
    {
        lodingPopUpSceen?.SetActive(true);
        yield return new WaitUntil(() => _loadingSet == false);
        lodingPopUpSceen?.SetActive(false);
        SwapMenuScreens(currentIndex, targetIndex);
    }

    
}
