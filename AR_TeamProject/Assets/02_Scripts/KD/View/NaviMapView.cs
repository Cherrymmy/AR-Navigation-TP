using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NaviMapView : IUimenu
{
    [Tooltip("바꾸고 싶은 메뉴2")]
    public UIManager.MenuType TargetMeun2Type;
    [Space]
    public Button exitButton;
    public Button ARChange;

    #region Swich 
    public override void Close()
    {
        TargetSwitch2Meun();
        UIManager.Instance.LoadingSet = false;
    }

    public override void Open()
    {
        TargetSwitch();
        UIManager.Instance.LoadingSet = false;
        SceneManager.LoadScene(2);
    }

    public override void TargetSwitch()
    {
        UIManager.Instance.Switch(CurrentMenu, TargetMenuType);
    }

    public void TargetSwitch2Meun()
    {
        UIManager.Instance.Switch(CurrentMenu, TargetMeun2Type);
    }
    #endregion

    private void Start()
    {
        exitButton.onClick.AddListener(Close);
        // AR 씬 로드 하는거 여기에 
        ARChange.onClick.AddListener(Open);
    }

}