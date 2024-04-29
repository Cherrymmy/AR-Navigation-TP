using AR.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static AR.DataManager;

namespace AR
{
    public class PlacesSearchController : MonoBehaviour
    {
        public PlaceSearchView psview;
        public PlacesModel PlacesModel;


        void Start()
        {
            psview = FindAnyObjectByType<PlaceSearchView>();
            psview.OnReSearchCreate.AddListener(ReSearchCreate);
            //psview.OnSearchCreate.AddListener();
        }

        private void ReSearchCreate()
        {
            List<PlaceIdData> placesDatas = DataManager.Instance.jsonDatas.datas; // 타입과 프로퍼티 접근 수정
            foreach (var place in placesDatas)
            {
                name = place.Name;
                ObjectPool.Instance.GetReSearchListElement(name); 
            }
        }

        private void SearchCreate()
        {

        }

        void Update()
        {

        }
    }

}
