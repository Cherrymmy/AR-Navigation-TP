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
        // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

        void Start()
        {
            _psview = FindAnyObjectByType<PlaceSearchView>();
            _placesModel = FindAnyObjectByType<PlacesModel>();

            /* PlaceSearchView Event */                                                                                  
            // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            _psview.OnInputFieldSelect.AddListener(ReSearchCreate);                                                      // 인풋 필드 선택시
            _psview.OnInputFieldChange.AddListener(PlaceSearch);                                                         // 인풋 필드 값 변경시
            // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

            /* placeSearch 완료시 */
            // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            _placesModel.OnDataParsed.AddListener(SearchCreate);                                                         // 서치 완료 이벤트

            /* objectPool 생성 이벤트 */
            // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            ObjectPool.Instance.OnDetailView.AddListener(OnDetailView);
            ObjectPool.Instance.OnListDestroy.AddListener(OnListDestroy);


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
                name = place.Name;
                Debug.Log(name);
                ObjectPool.Instance.GetReSearchListElement(name);
            }
        }

        /// <summary>
        /// 검색기록 생성
        /// Controller -> objectPool ->Veiw
        /// </summary>
        private void SearchCreate()
        {
            //ReSearchClear();
            SearchClear();
            var list = _placesModel.PlacesData.results;
            foreach (var place in list)
            {
                name = place.name;
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


        private void OnDetailView()
        {
            
        }

        private void OnListDestroy()
        {

        }
    }
}
