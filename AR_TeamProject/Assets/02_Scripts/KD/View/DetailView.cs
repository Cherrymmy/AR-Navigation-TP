using AR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetailView : IUimenu
{
    [Tooltip("바꾸고 싶은 메뉴2")]
    public UIManager.MenuType TargetMeun2Type;
    [Space]
    [Header("UI")]
    public Button exitButton;
    [Header("뱃지")]
    public TMP_Text Badgetxet1;
    public TMP_Text Badgetxet2;
    [Header("장소")]
    public TMP_Text placename;
    public TMP_Text placeAddress;
    [Header("사진")]
    public Photo[] photos;
    [Header("리뷰")]
    public int riviews;
    public TMP_Text totalRiviews;


    private void Start()
    {
        /* Event */
        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        exitButton.onClick.AddListener(Close);
        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    }
    #region Swich 
    public override void Close()
    {
        TargetSwitch2Meun();
    }

    public override void Open()
    {
        TargetSwitch();

        // 상대방 준비되면 시작 

        UIManager.Instance.LoadingSet = false;
    }

    public override void TargetSwitch()
    {
        UIManager.Instance.Switch(CurrentMenu, TargetMenuType);
    }

    public void TargetSwitch2Meun()
    {
        UIManager.Instance.Switch(CurrentMenu, TargetMeun2Type);
        UIManager.Instance.LoadingSet = false;
    }
    #endregion


}
