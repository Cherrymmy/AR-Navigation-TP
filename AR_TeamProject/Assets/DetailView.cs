using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailView : IUimenu
{
    [Tooltip("바꾸고 싶은 메뉴2")]
    public UIManager.MenuType TargetMeun2Type;
    public override void Close()
    {

    }

    public override void Open()
    {

    }

    public override void TargetSwitch()
    {
        UIManager.Instance.Switch(MenuType, TargetMenuType);
    }

    public void TargetSwitch2Meun()
    {
        UIManager.Instance.Switch(MenuType, TargetMeun2Type);
    }
}
