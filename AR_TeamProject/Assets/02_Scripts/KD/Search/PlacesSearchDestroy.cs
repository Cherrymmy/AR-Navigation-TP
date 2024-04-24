using System.Collections;
using UnityEngine;

namespace Search
{
    public class PlacesSearchDestroy : MonoBehaviour
    {
        public GameObject[] placesearch;

        private Coroutine updateCoroutine;

        // �ڽ��� ���� �Ǹ� ����Ǵ� �Լ�
        public void OnTransformChildrenChanged()
        {
            // �̹� ���� ���� �ڷ�ƾ�� �ִٸ� ����
            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
            }

            // �ڷ�ƾ ����
            updateCoroutine = StartCoroutine(UpdatePlacesearchCoroutine());
        }

        private IEnumerator UpdatePlacesearchCoroutine()
        {
            PrefabsDestroy();
            // �ڽ� ��ü�� ���� ����
            int children = transform.childCount;
            // �ӽ� �迭 �ʱ�ȭ
            GameObject[] tempPlacesearch = new GameObject[children];

            // �ڽ� ��ü�� �迭�� ����
            for (int i = 0; i < children; i++)
            {
                tempPlacesearch[i] = transform.GetChild(i).gameObject;

                // �� �ڽ� ��ü���� ������ ����
                if (i % 10 == 0)
                {
                    yield return null;
                }
            }

            // ��� �۾� �Ϸ� �� ���� �迭�� �Ҵ�
            placesearch = tempPlacesearch;
        }

        public void PrefabsDestroy()
        {
            // ��� �˻� ����� �ִ� ��� ���� ����� �����մϴ�.
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
