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
            // 프리펩을 인스턴스화합니다.
            GameObject newButton = Instantiate(buttonPrefab[index], parentPanel.transform);

            // 인스턴스화된 버튼에서 TextMeshPro 컴포넌트를 찾습니다.
            TMP_Text tmpText = newButton.GetComponentInChildren<TMP_Text>();

            // 텍스트 컴포넌트가 있다면, 그 내용을 변경합니다.
            if (tmpText != null)
            {
                tmpText.text = text;
            }
        }
    }
}