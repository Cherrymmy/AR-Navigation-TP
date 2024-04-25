using AR.Models;
using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


///모델에서 제공한 데이터를 가공하여 뷰가 사용할 수 있는 형태로 만듭니다. 또한, 사용자의 입력을 처리하고 그에 따라 모델을 업데이트합니다.
namespace AR.ViewModels
{
    public class PlacesViewModel : MonoBehaviour
    {
        public PlacesModel placesModel;
        public UnityEvent onPlacesProcessed; // 데이터 로드 완료 이벤트
        public GameObject searchListTransform;
        public Button searchList;


        private void Awake()
        {
            placesModel = FindAnyObjectByType<PlacesModel>();
        }
        void Start()
        {
            placesModel.OnDataParsed.AddListener(HandleDataParsed);
            //onPlacesProcessed.AddListener();
        }

        private void HandleDataParsed()
        {
            
            // 모델에서 파싱된 데이터를 사용하여 무엇인가를 할 수 있습니다.
            onPlacesProcessed.Invoke();
        }

        // 인풋필드 값으로 검색
        public void SearchPlaces(string query)
        {
            if (query.Length == 0)
            {

            }
            else if (query.Length >= 2)
            {
                placesModel.SearchPlaces(query);
                onPlacesProcessed.Invoke();
            }
        }

        public void CreateObject(string name, Button searchList, GameObject searchListTransform)
        {
            // 실제 필요한 객체 생성 로직
            Button newButton = Instantiate(searchList, searchListTransform.transform);

            // 인스턴스화된 버튼에서 TextMeshPro 컴포넌트를 찾습니다.
            TMP_Text tmpText = newButton.GetComponentInChildren<TMP_Text>();
            newButton.onClick.AddListener(() => OnClick(name));

            // 텍스트 컴포넌트가 있다면, 그 내용을 변경합니다.
            if (tmpText != null)
            {
                tmpText.text = name;
            }
        }

        private void OnClick(string name)
        {
            //TMP_Text tmptext = gameObject.GetComponentInChildren<TMP_Text>();

            // 검색기록 저장 해주고 
            for(int i = 0; placesModel.PlacesData.results.Length > i; i++)
            {
                if(name == placesModel.PlacesData.results[i].name)
                {
                    placesModel.SaveData(name, placesModel.PlacesData.results[i].place_id);
                }
            }
            // 여기서 Details api로 검색
        }

        public void DestroyPlaceObjects()
        {
            GameObject goMain = GameObject.Find("SearchList");

            for (int i = 0; i < goMain.transform.childCount; i++)
            {
                Destroy(goMain.transform.GetChild(i).gameObject);
            }
        }

        #region UI 업데이트
        public void UpdateUi()
        {
            // 검색 결과 UI 요소를 클리어
            ClearSearchResultsUi();

            // 새로운 검색 결과로 UI 업데이트
            // PlacesData가 null이 아닌지, results가 null이 아닌지 체크
            if (placesModel.PlacesData != null && placesModel.PlacesData.results != null)
            {
                foreach (var place in placesModel.PlacesData.results)
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
            DestroyPlaceObjects();
        }

        private void CreatePlaceObjectInUi(string placeName)
        {
            // 검색 결과에 따라 UI 리스트에 새 요소를 추가
            CreateObject(placeName, searchList, searchListTransform);
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
