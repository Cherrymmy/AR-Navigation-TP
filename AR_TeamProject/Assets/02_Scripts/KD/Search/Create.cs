using TMPro;
using UnityEngine;

namespace Search
{
    public class Create : MonoBehaviour
    {
        public GameObject[] buttonPrefab;
        private GameObject parentPanel;

        private void Start()
        {
            parentPanel = GameObject.Find("List");
        }

        public void CreateButton(string text, int index)
        {
            // �������� �ν��Ͻ�ȭ�մϴ�.
            GameObject newButton = Instantiate(buttonPrefab[index], parentPanel.transform);

            // �ν��Ͻ�ȭ�� ��ư���� TextMeshPro ������Ʈ�� ã���ϴ�.
            TMP_Text tmpText = newButton.GetComponentInChildren<TMP_Text>();

            // �ؽ�Ʈ ������Ʈ�� �ִٸ�, �� ������ �����մϴ�.
            if (tmpText != null)
            {
                tmpText.text = text;
            }
        }
    }
}