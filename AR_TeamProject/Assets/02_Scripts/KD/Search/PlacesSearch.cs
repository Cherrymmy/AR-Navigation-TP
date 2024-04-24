using System.Collections;
using TMPro;
using UnityEngine;

namespace Search
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

            // InputField�� ���� ����� ������ OnValueChange�� ȣ��
            inputField.onValueChanged.AddListener(OnValueChange);
            // delegate ��� �ϴ� ���� �������� ���, ������ ����, ���ٽ� ǥ��
            inputField.onSelect.AddListener(delegate { HandleSelect(); });
        }

        public void OnValueChange(string searchText)
        {
            // destroy�� null�� �ƴ��� ���� Ȯ��
            if (destroy.placesearch != null)
            {
                // �Էµ� �ؽ�Ʈ�� ���� �� ��� �˻� ����� �ı��մϴ�.
                if (string.IsNullOrEmpty(searchText))
                {
                    destroy.PrefabsDestroy();  // ���� �˻� ����� �ı�
                    LoadDataAndCreateButtons();
                }
                // �ּ� 2���� �̻� �ԷµǾ��� �� �˻��� �����մϴ�.
                else if (searchText.Length >= 2)
                {
                    StartCoroutine(SearchAPI.Instance.SearchPlacesCoroutine(searchText));
                }
            }
            else
            {
                // destroy�� null�̸� ���⼭ ���� �޽����� �α��ϰų� ������ ��ġ�� ���� �� �ֽ��ϴ�.
                Debug.LogWarning("Destroy component is not assigned or missing.");
            }
        }

        private void LoadDataAndCreateButtons()
        {
            DataManager.Instance.LoadPlacesDatas();  // ������ �ε�
            if (DataManager.Instance.jsonDatas != null && DataManager.Instance.jsonDatas.datas != null)
            {
                foreach (var data in DataManager.Instance.jsonDatas.datas)
                {
                    create.CreateButton(data.Name, 1);  // �� ������ �׸� ���� ��ư ����
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
                destroy.PrefabsDestroy();  // ���� �˻� ����� �ı�
                LoadDataAndCreateButtons();  // jsonDatas���� ������ �ε� �� ��ư ����
            }
        }
    }
}

