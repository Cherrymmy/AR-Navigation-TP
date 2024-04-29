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

        public UnityEvent OnReSearchCreate;
        public UnityEvent OnSearchCreate;


        public PlacesSearchController PSController;


        private void Start()
        {
            searchText.onValueChanged.AddListener(delegate { SearchListCreate(); });
            searchText.onSelect.AddListener(delegate { ReSearchListCreate(); });
            exitButton.onClick.AddListener(delegate { Close(); });
        }

        public override void Close()
        {
            TargetSwitch2Meun();
        }

        public override void Open()
        {
            TargetSwitch();
            UIManager.Instance.LoadingSet = false;
        }

        public override void TargetSwitch()
        {
            UIManager.Instance.Switch(MenuType, TargetMenuType);
        }

        public void TargetSwitch2Meun()
        {
            UIManager.Instance.Switch(MenuType, TargetMeun2Type);
            UIManager.Instance.LoadingSet = false;
        }

        public void ReSearchListCreate()
        {
            OnReSearchCreate.Invoke();
        }

        public void SearchListCreate()
        {
            OnSearchCreate.Invoke();
        }
    }
}

