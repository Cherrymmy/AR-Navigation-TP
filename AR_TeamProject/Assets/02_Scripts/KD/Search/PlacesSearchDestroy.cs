using System.Collections;
using UnityEngine;

namespace Search
{
    public class PlacesSearchDestroy : MonoBehaviour
    {
        public GameObject[] placesearch;

        private Coroutine updateCoroutine;

        // 자식이 변경 되면 실행되는 함수
        public void OnTransformChildrenChanged()
        {
            // 이미 실행 중인 코루틴이 있다면 중지
            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
            }

            // 코루틴 시작
            updateCoroutine = StartCoroutine(UpdatePlacesearchCoroutine());
        }

        private IEnumerator UpdatePlacesearchCoroutine()
        {
            PrefabsDestroy();
            // 자식 객체의 수를 얻음
            int children = transform.childCount;
            // 임시 배열 초기화
            GameObject[] tempPlacesearch = new GameObject[children];

            // 자식 객체를 배열에 저장
            for (int i = 0; i < children; i++)
            {
                tempPlacesearch[i] = transform.GetChild(i).gameObject;

                // 매 자식 객체마다 프레임 지연
                if (i % 10 == 0)
                {
                    yield return null;
                }
            }

            // 모든 작업 완료 후 메인 배열에 할당
            placesearch = tempPlacesearch;
        }

        public void PrefabsDestroy()
        {
            // 장소 검색 결과가 있는 경우 기존 결과를 삭제합니다.
            if (placesearch != null && placesearch.Length > 0)
            {
                for (int i = 0; i < placesearch.Length; i++)
                {
                    Destroy(placesearch[i]);
                }
            }
        }
    }

}
