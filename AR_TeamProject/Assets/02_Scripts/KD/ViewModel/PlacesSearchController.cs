using AR.Models;
using UnityEngine;
using UnityEngine.Events;

namespace AR
{
    public class PlacesSearchController : MonoBehaviour
    {
        // DetailModel 이벤트용
        // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        public UnityEvent onDetailSearch;
        // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        private PlaceSearchView _psview;
        private PlacesModel _placesModel;
        private DetailModel _detailModel;
        // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

        void Start()
        {
            _psview = FindAnyObjectByType<PlaceSearchView>();
            _placesModel = FindAnyObjectByType<PlacesModel>();
            _detailModel = FindAnyObjectByType<DetailModel>();

            /* PlaceSearchView Event */
            // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            _psview.OnInputFieldSelect.AddListener(ReSearchCreate);                                                      // 인풋 필드 선택시
            _psview.OnInputFieldChange.AddListener(PlaceSearch);                                                         // 인풋 필드 값 변경시
            // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

            /* placeSearchModel Event */
            // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            _placesModel.OnDataParsed.AddListener(SearchCreate);                                                         // 서치 완료 이벤트
            _placesModel.OnDataUpdated.AddListener(ReSearchCreate);                                                      // 이전 기록 저장시



        }
        #region 요청 및 UI 생성
        /// <summary>
        /// 인풋 값 받아서 서치 돌리기 
        /// Controller -> model
        /// </summary>
        private void PlaceSearch()
        {
            _placesModel.SearchPlaces(_psview.searchText.text);
        }

        /// <summary>
        /// 이전 검색기록 생성
        /// Controller -> objectPool ->Veiw
        /// </summary>
        private void ReSearchCreate()
        {
            ReSearchClear();
            var placesDatas = DataManager.Instance.jsonDatas.datas;
            foreach (var place in placesDatas)
            {
                string name = place.Name;
                ObjectPool.Instance.GetReSearchListElement(name);
            }
        }

        /// <summary>
        /// 검색기록 생성
        /// Controller -> objectPool ->Veiw
        /// </summary>
        private void SearchCreate()
        {
            SearchClear();
            var list = DataManager.Instance.PlacesData.results;
            foreach (var place in list)
            {
                string name = place.name;
                ObjectPool.Instance.GetSearchListElement(name);
            }
        }
        #endregion
        #region 오브젝트 풀링 초기화
        private void ReSearchClear()
        {
            ObjectPool.Instance.ClearReSearchResults();
        }

        private void SearchClear()
        {
            ObjectPool.Instance.ClearSearchResults();
        }
        #endregion
    }
}
