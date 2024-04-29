using System.Collections;
using TMPro;
using UnityEngine;

namespace AR
{
    public class PlacesSearch : MonoBehaviour
    {
        private TMP_InputField inputField;
        private PlacesSearchDestroy destroy;
        private Create create;


        private void Start()
        {
            destroy = FindObjectOfType<PlacesSearchDestroy>();
            create = FindObjectOfType<Create>();
            inputField = GetComponentInChildren<TMP_InputField>();

            // InputField의 값이 변경될 때마다 OnValueChange를 호출
            inputField.onValueChanged.AddListener(OnValueChange);
            // delegate 사용 하는 이유 여러동작 사용, 조건적 실행, 람다식 표현
            inputField.onSelect.AddListener(delegate { HandleSelect(); });
        }

        public void OnValueChange(string searchText)
        {
            // destroy가 null이 아닌지 먼저 확인
            if (destroy.placesearch != null)
            {
                // 입력된 텍스트가 없을 때 모든 검색 결과를 파괴합니다.
                if (string.IsNullOrEmpty(searchText))
                {
                    destroy.PrefabsDestroy();  // 기존 검색 결과를 파괴
                    LoadDataAndCreateButtons();
                }
                // 최소 2글자 이상 입력되었을 때 검색을 시작합니다.
                else if (searchText.Length >= 2)
                {
                    StartCoroutine(SearchAPI.Instance.SearchPlacesCoroutine(searchText));
                }
            }
            else
            {
                // destroy가 null이면 여기서 오류 메시지를 로깅하거나 적절한 조치를 취할 수 있습니다.
                Debug.LogWarning("Destroy component is not assigned or missing.");
            }
        }

        private void LoadDataAndCreateButtons()
        {
            DataManager.Instance.LoadPlacesDatas();  // 데이터 로드
            if (DataManager.Instance.jsonDatas != null && DataManager.Instance.jsonDatas.datas != null)
            {
                foreach (var data in DataManager.Instance.jsonDatas.datas)
                {
                    create.CreateButton(data.Name, 1);  // 각 데이터 항목에 대한 버튼 생성
                }
            }
            else
            {
                Debug.LogWarning("No data available in jsonDatas.");
            }
        }


        private void HandleSelect()
        {
            if (destroy != null)
            {
                destroy.PrefabsDestroy();  // 기존 검색 결과를 파괴
                LoadDataAndCreateButtons();  // jsonDatas에서 데이터 로드 및 버튼 생성
            }
        }
    }
}

