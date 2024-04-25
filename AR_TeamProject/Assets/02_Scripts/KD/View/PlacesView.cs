using UnityEngine;
using TMPro;
using AR.ViewModels;
using UnityEngine.UI;


///뷰모델이 제공하는 데이터를 사용자에게 보여주고, 사용자의 입력을 뷰모델로 전달합니다. 뷰는 직접적으로 데이터를 처리하거나 비즈니스 로직을 포함하지 않습니다.
namespace AR.Views
{
    public class PlacesView : MonoBehaviour
    {
        private PlacesViewModel _viewModel;
        private TMP_InputField _input_Search;
        public GameObject searchListTransform;                                                      // 리스트 프리펩 생성 위치
        public Button[] searchListPrefabs;                                                          // 생성될 프리펩 종류 0.검색 기록 1.이전기록


        private void Awake()
        {
            _viewModel = FindAnyObjectByType<PlacesViewModel>();
            _input_Search = GetComponentInChildren<TMP_InputField>();
        }

        void Start()
        {
            _input_Search.onValueChanged.AddListener((value) => OnSearchInputChanged(value));       // input 값에 따른 검색
            //_input_Search.onSelect.AddListener(OnSearchInputSelect);                              // 문제있음
            _viewModel.onPlacesProcessed.AddListener(DisplayPlaces);                                // UI 업데이트
            _viewModel.OnDeleteButton.AddListener(DestroyPlaceObjects);                             // 리스트 프리펩 사제
            _viewModel.OnCreateButtonRequested.AddListener(CreateButton);                           // places 검색 기록 생성
            _viewModel.OnLodeCreateButtonRequested.AddListener(LoadDataAndCreateButton);            // 이전검색 기록 생성
        }

        // 인풋 값이 변경이 되면 Place Search 
        private void OnSearchInputChanged(string searchTerm)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                _viewModel.SearchPlaces(searchTerm);
            }
            
        }

        // 인풋 필드 활성화 되면 실행 
        private void OnSearchInputSelect()
        {
            _viewModel.LodeCreatButton();
        }

        // 검색 리스트 생성
        public void CreateButton(string name)
        {
            Button newButton = Instantiate(searchListPrefabs[0], searchListTransform.transform);
            TMP_Text tmpText = newButton.GetComponentInChildren<TMP_Text>();
            newButton.onClick.AddListener(() => _viewModel.OnClick(name));

            if (tmpText != null)
            {
                tmpText.text = name;
            }
        }

        // 이전 검색 기록 생성 
        public void LoadDataAndCreateButton(string name)
        {
            Button newButton = Instantiate(searchListPrefabs[0], searchListTransform.transform);
            TMP_Text tmpText = newButton.GetComponentInChildren<TMP_Text>();
            Button exitButton = newButton.GetComponentInChildren<Button>();

            newButton.onClick.AddListener(() => _viewModel.OnClick(name));
            exitButton.onClick.AddListener(() => _viewModel.OnDelete(name));

            if (tmpText != null)
            {
                tmpText.text = name;
            }
        }

        // 다지우기 
        public void DestroyPlaceObjects()
        {
            GameObject goMain = GameObject.Find("SearchList");

            for (int i = 0; i < goMain.transform.childCount; i++)
            {
                Destroy(goMain.transform.GetChild(i).gameObject);
            }
        }

        private void OnsearchInputSelect()
        {
            //_viewModel.LodeDataCreateObject();
        }

        // 리스트 업데이트
        void DisplayPlaces()
        {
            // viewModel에서 파싱된 데이터를 사용하여 UI 업데이트
            _viewModel.UpdateUi();
        }


    }
}
