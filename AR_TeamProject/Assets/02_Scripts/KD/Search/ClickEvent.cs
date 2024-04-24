using TMPro;
using UnityEngine;

namespace Search
{
    public class CilckEvent : MonoBehaviour
    {
        public void OnClick()
        {
            TMP_Text tmpText = GetComponentInChildren<TMP_Text>();
            if (tmpText != null)
            {
                Debug.Log(tmpText.text);
                SearchAPI.Instance.Toss(tmpText.text);
            }
            else
            {
                Debug.LogError("TMP_Text component is missing on the button.");
            }
        }

        public void DeletePlace()
        {
            TMP_Text tmpText = GetComponentInChildren<TMP_Text>();
            DataManager.Instance.RemovePlaceIdData(tmpText.text); // JSON에서 데이터 삭제
            Destroy(gameObject); // 프리팹 제거
        }
    }

}
