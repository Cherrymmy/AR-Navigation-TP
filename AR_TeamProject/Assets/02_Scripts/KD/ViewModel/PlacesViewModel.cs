using AR.Models;
using System;
using System.ComponentModel;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


///모델에서 제공한 데이터를 가공하여 뷰가 사용할 수 있는 형태로 만듭니다. 또한, 사용자의 입력을 처리하고 그에 따라 모델을 업데이트합니다.
namespace AR.ViewModels
{
    public class PlacesViewModel : MonoBehaviour
    {
        private PlacesModel _placesModel;

        public UnityEvent onPlacesProcessed;                            // view/ 데이터 로드 완료 이벤트
        public UnityEvent<string> OnCreateButtonRequested;              // view/ 검색기록 생성
        public UnityEvent<string> OnLodeCreateButtonRequested;          // view/ 이전 기록 생성 
        public UnityEvent OnDeleteButton;                               // view/ 리스트 삭제


        public void RequestCreateButton(string name)
        {
            OnCreateButtonRequested?.Invoke(name);
        }

        public void LodeCreatButton()
        {
            Debug.Log("이전 검색기록");
            Debug.Log(DataManager.Instance.jsonDatas.datas);
            foreach (var data in DataManager.Instance.jsonDatas.datas)
            {
                Debug.Log("도냐?");
                string name = data.Name; 
                OnLodeCreateButtonRequested.Invoke(name);
            }
        }

        private void Awake()
        {
            _placesModel = FindAnyObjectByType<PlacesModel>();
        }
        void Start()
        {
            _placesModel.OnDataParsed.AddListener(HandleDataParsed);
        }

        private void HandleDataParsed()
        {
            // 모델에서 파싱된 데이터를 사용하여 무엇인가를 할 수 있습니다.
            UpdateUi();
        }

        // 인풋필드 값으로 검색
        public void SearchPlaces(string query)
        {
            if (query.Length >= 2)
            {
                _placesModel.SearchPlaces(query);
                UpdateUi();
            }
        }

        // Search 프리펩 클릭 이벤트 
        public void OnClick(string name)
        {
            // 검색기록 저장 해주고 
            for(int i = 0; _placesModel.PlacesData.results.Length > i; i++)
            {
                if(name == _placesModel.PlacesData.results[i].name)
                {
                    _placesModel.SaveData(name, _placesModel.PlacesData.results[i].place_id);
                }
            }
            // 여기서 Details api로 검색
        }

        // 이전 검색기록 삭제 
        public void OnDelete(string name)
        {
            _placesModel.LoadDataDelete(name);
            Destroy(gameObject); // 프리팹 제거
        }
        

        #region UI 업데이트
        public void UpdateUi()
        {
            // 검색 결과 UI 요소를 클리어
            ClearSearchResultsUi();

            if (_placesModel.PlacesData != null && _placesModel.PlacesData.results != null)
            {
                foreach (var place in _placesModel.PlacesData.results)
                {
                    CreatePlaceObjectInUi(place.name);
                }
            }
            // 로딩 인디케이터 비활성화
            SetLoadingIndicator(false);

            // 검색 필드 초기화 또는 업데이트
            ResetSearchField();
        }

        private void ClearSearchResultsUi()
        {
            OnDeleteButton.Invoke();
        }

        private void CreatePlaceObjectInUi(string placeName)
        {
            // 검색 결과에 따라 UI 리스트에 새 요소를 추가
            RequestCreateButton(placeName);
        }

        private void SetLoadingIndicator(bool isActive)
        {
            // 로딩 인디케이터 활성화 또는 비활성화
        }

        private void ResetSearchField()
        {
            
        }
        #endregion
    }
}
