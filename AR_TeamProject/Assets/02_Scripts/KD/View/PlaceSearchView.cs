using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
namespace AR
{
    public class PlaceSearchView : IUimenu
    {
        [Tooltip("바꾸고 싶은 메뉴2")]
        public UIManager.MenuType TargetMeun2Type;
        [Space]
        [Header("UI")]
        [Tooltip("검색창")]
        public TMP_InputField searchText;
        [Tooltip("뒤로가기 버튼")]
        public Button exitButton;

        /* PlacesSearchController 이벤트 */
        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        public UnityEvent OnInputFieldSelect;
        public UnityEvent OnInputFieldChange;
        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

        private void Start()
        {
        /* 인풋 필드 */
        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            searchText.onValueChanged.AddListener(delegate { SearchListCreate();});
            searchText.onSelect.AddListener(delegate { ReSearchListCreate(); });
        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

        /* 뒤로 가기 버튼 */
        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            exitButton.onClick.AddListener(delegate { Close(); });
        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        }

        public override void Close()
        {
            TargetSwitch2Meun();
            OnEnableStatic();
            UIManager.Instance.LoadingSet = false;
        }

        public override void Open()
        {
            TargetSwitch();
        }

        public override void TargetSwitch()
        {
            UIManager.Instance.Switch(CurrentMenu, TargetMenuType);
        }

        public void TargetSwitch2Meun()
        {
            UIManager.Instance.Switch(CurrentMenu, TargetMeun2Type);
        }

        public void ReSearchListCreate()
        {
            OnInputFieldSelect.Invoke();
        }

        public void SearchListCreate()
        {
            
            OnInputFieldChange.Invoke();
        }

        private void OnEnableStatic()
        {
            StaticMapRenderer staticMapRenderer = GameObject.Find("RawImage - StaticMap").GetComponent<StaticMapRenderer>();

            if(staticMapRenderer != null )
            {
                staticMapRenderer.enabled = true;
            }
        }
    }
}
