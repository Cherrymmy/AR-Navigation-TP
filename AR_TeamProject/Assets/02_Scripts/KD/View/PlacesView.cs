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

        private void Awake()
        {
            _viewModel = FindAnyObjectByType<PlacesViewModel>();
            _input_Search = GetComponentInChildren<TMP_InputField>();
        }

        void Start()
        {
            _input_Search.onValueChanged.AddListener((value) => OnSearchInputChanged(value));
            //input_Search.onSelect.AddListener();
            _viewModel.onPlacesProcessed.AddListener(DisplayPlaces);
        }

        private void OnSearchInputChanged(string searchTerm)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                _viewModel.SearchPlaces(searchTerm);
            }
        }

        

        void DisplayPlaces()
        {
            // viewModel에서 파싱된 데이터를 사용하여 UI 업데이트
            // 예: 결과를 리스트뷰에 표시
            _viewModel.UpdateUi();
        }
    }
}
