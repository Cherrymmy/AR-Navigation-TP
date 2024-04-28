using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailView : IUimenu
{
    [Tooltip("바꾸고 싶은 메뉴2")]
    public UIManager.MenuType TargetMeun2Type;
    [Space]
    [Header("UI")]
    public Button exitButton;

    private void Start()
    {
        exitButton.onClick.AddListener(Close);
    }
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
        UIManager.Instance.Switch(MenuType, TargetMenuType);
    }

    public void TargetSwitch2Meun()
    {
        UIManager.Instance.Switch(MenuType, TargetMeun2Type);
        UIManager.Instance.LoadingSet = false;
    }
}
