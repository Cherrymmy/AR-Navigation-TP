using AR.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static AR.ObjectPool;

namespace AR
{
    public class PlacesSearchController : MonoBehaviour
    {
        public PlaceSearchView psview;
        public PlacesModel placesModel;


        void Start()
        {
        // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            psview = FindAnyObjectByType<PlaceSearchView>();
            placesModel = FindAnyObjectByType<PlacesModel>();
        // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        //  PlaceSearchView Event
            psview.OnInputFieldSelect.AddListener(ReSearchCreate);                                                      // 인풋 필드 선택시
            psview.OnInputFieldChange.AddListener(PlaceSearch);                                                         // 인풋 필드 값 변경시
        // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            placesModel.OnDataParsed.AddListener(SearchCreate);                                                         // 서치 완료 이벤트
        }

        /// <summary>
        /// 인풋 값 받아서 서치 돌리기 
        /// </summary>
        private void PlaceSearch()
        {
            placesModel.SearchPlaces(psview.searchText.text);
        }

        /// <summary>
        /// 이전 검색기록 생성
        /// </summary>
        private void ReSearchCreate()
        {

            ReSearchClear();
            var placesDatas = DataManager.Instance.jsonDatas.datas;
            foreach (var place in placesDatas)
            {
                name = place.Name;
                Debug.Log(name);
                Instance.GetReSearchListElement(name);
            }
        }

        /// <summary>
        /// 검색기록 생성
        /// </summary>
        private void SearchCreate()
        {
            //ReSearchClear();
            SearchClear();
            var list = placesModel.PlacesData.results;
            foreach (var place in list)
            {
                name = place.name;
                Instance.GetSearchListElement(name);
            }
        }

        private void ReSearchClear()
        {
            Instance.ClearReSearchResults();

        }

        private void SearchClear()
        {
            Instance.ClearSearchResults();
        }
    }
}
